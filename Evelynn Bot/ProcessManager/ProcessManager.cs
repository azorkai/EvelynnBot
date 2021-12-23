using Evelynn_Bot.Account_Process;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;
using bAUTH;
using Evelynn_Bot.Constants;
using Evelynn_Bot.Entities;
using Evelynn_Bot.ExternalCommands;
using Evelynn_Bot.GameAI;
using Evelynn_Bot.League_API.GameData;
using Evelynn_Bot.ProcessManager;
using EvelynnLCU.API_Models;
using EvelynnLCU.Plugins.LoL;
using Newtonsoft.Json;

namespace Evelynn_Bot.ProcessManager
{
    public class ProcessManager : IProcessManager
    {
        public async Task<Task> Start(Interface itsInterface)
        {
            await StartAccountProcess(itsInterface);

            //itsInterface.gameAi.YeniAIBaslat(itsInterface);

            //while (IsGameStarted(itsInterface) == false)
            //{
            //    Thread.Sleep(15000);
            //    IsGameStarted(itsInterface);
            //}
            return Task.CompletedTask;
        }

        public async Task<bool> RiotUpdateCheck(Interface itsInterface)
        {
            try
            {
                int tryNum = 1;
                bool isPatchDone = false;
                while (await RiotIsUpdateAvailable(itsInterface))
                {
                    isPatchDone = true;
                    itsInterface.logger.Log(true, itsInterface.messages.Patch);
                    await Task.Delay(60000);
                    Dispose(true);
                    tryNum++;
                    if (tryNum >= 60)
                    {
                        break;
                    }
                }
                itsInterface.logger.Log(true, "Patch done for RiotClient.");
                if (isPatchDone)
                {
                    itsInterface.clientKiller.KillAllLeague();
                    itsInterface.clientKiller.RestartAndExit(itsInterface);
                }

                return itsInterface.Result(true, "");
            }
            catch (Exception e)
            {
                Console.WriteLine($"PATCH CHECK HATASI {e}");
                return itsInterface.Result(false, "");
            }
        }

        public async Task<bool> RiotIsUpdateAvailable(Interface itsInterface)
        {
            RiotUpdateResponse riotUpdateNew = await itsInterface.lcuPlugins.GetRiotClientUpdateAsync();
            try
            {
                if (riotUpdateNew.PatchRequested != null && !riotUpdateNew.PatchRequested.Value)
                {
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                //TODO: BELKI HATA CIKARABILIR | RESTART GEREKEBILIR
                return false;
            }
        }


        public async Task<Task> StartAccountProcess(Interface itsInterface, bool isFromGame = false)
        {
            await itsInterface.clientKiller.StartFPSLimiter();
            
            itsInterface.newQueue.bugTimer.Stop();
            //itsInterface.clientKiller.KillAllLeague();

            await Task.Delay(10000);

            try
            {
                CHECKACTIONS:
                if (itsInterface.dashboard.IsStop && itsInterface.dashboard.IsNoAccount) // No Account Found
                {
                    itsInterface.isBotStarted = false;
                    await Task.Delay(20000);
                    goto CHECKACTIONS;
                }

                itsInterface.logger.Log(true, "ID: " + itsInterface.license.Lol_username);
                itsInterface.logger.Log(true, "Password: " + itsInterface.license.Lol_password);
                itsInterface.logger.Log(true, "Max Level: " + itsInterface.license.Lol_maxLevel.ToString());
                itsInterface.logger.Log(true, "Max BE: " + itsInterface.license.Lol_maxBlueEssences);
                itsInterface.logger.Log(true, "Tutorial: " + itsInterface.license.Lol_doTutorial);
                itsInterface.logger.Log(true, "Disenchant: " + itsInterface.license.Lol_disenchant.ToString());
                itsInterface.logger.Log(true, "Empty Nick: " + itsInterface.license.Lol_isEmptyNick);

                using (AccountProcess accountProcess = new AccountProcess())
                {
                    if (isFromGame == false)
                    {
                        accountProcess.StartLeague(itsInterface, StartEnums.RiotClient);
                        await Task.Delay(8000);
                        itsInterface.lcuApi.BeginTryInitRiotClient();
                    }
                    if (processExist("RiotClientServices", itsInterface))
                    {
                        if (isFromGame == false) { await accountProcess.LoginAccount(itsInterface); }
                        if (!accountProcess.Initialize(itsInterface))
                        {
                            itsInterface.logger.Log(false, "Restarting...");
                            await Task.Delay(7000);
                            await Restart(itsInterface);
                        }

                        try
                        {
                            await Task.Delay(1000);
                            string session = await accountProcess.VerifySession(itsInterface);

                            switch (session)
                            {
                                case "banned_account":
                                    itsInterface.clientKiller.KillAllLeague();
                                    await Task.Delay(10000);
                                    itsInterface.dashboardHelper.UpdateLolStatus("Banned", itsInterface);
                                    return Start(itsInterface);
                                    break;
                                case "new_player_set_account":
                                    itsInterface.logger.Log(true ,"That's a new account");

                                    if (itsInterface.license.Lol_isEmptyNick == false)
                                    {
                                        Dispose(true);
                                        await accountProcess.CheckNewAccount(itsInterface);
                                    }
                                    break;
                                case "invalid_credentials":
                                    itsInterface.clientKiller.KillAllLeague();
                                    await Task.Delay(10000);
                                    itsInterface.dashboardHelper.UpdateLolStatus("Wrong", itsInterface);
                                    return Start(itsInterface);
                                    break;
                                case "restart_client_error":
                                    itsInterface.logger.Log(false, "Client Error.");
                                    return itsInterface.processManager.StartAccountProcess(itsInterface);
                                case "invalid_summoner_name":
                                    itsInterface.logger.Log(false,"Invalid summoner name!");
                                    break;
                                //case "logged_in_from_another":
                                //    Console.WriteLine("This account has logged in from somewhere else already!");
                                //    break;
                                default:
                                    itsInterface.logger.Log(true,$"LOGIN SESSION: {session}");
                                    break;
                            }
                        }

                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        
                        await Task.Delay(3500);

                        if (!await accountProcess.GetSetWallet(itsInterface))
                        {
                            itsInterface.clientKiller.KillAllLeague();
                            await Task.Delay(10000);
                            await Restart(itsInterface);
                        }

                        accountProcess.CopyConfig(itsInterface);
                        //InjectConfigPersist(itsInterface);

                        itsInterface.ProcessController.SuspendLeagueUx();
                        await accountProcess.CheckLeagueBan(itsInterface);
                        itsInterface.newQueue.itsInterface2 = itsInterface;
                        itsInterface.newQueue.UxEventAsync();
                        await accountProcess.PatchCheck(itsInterface);

                        try { await itsInterface.lcuPlugins.RemoveNotificationsAsync(); } catch (Exception e) { }

                        await itsInterface.lcuPlugins.GetSetMissions();

                        try { await itsInterface.lcuPlugins.ChangeSummonerIconAsync(); } catch (Exception e) { }

                        Dispose(true);

                        if (itsInterface.license.Lol_maxLevel != 0 && itsInterface.summoner.summonerLevel >= itsInterface.license.Lol_maxLevel)
                        {
                            itsInterface.logger.Log(true, itsInterface.messages.AccountDoneXP);
                            itsInterface.clientKiller.KillAllLeague();
                            await Task.Delay(10000);
                            itsInterface.dashboardHelper.UpdateLolStatus("Finished", itsInterface);
                            return Start(itsInterface);
                        }

                        if (itsInterface.license.Lol_maxBlueEssences != 0 && itsInterface.wallet.ip >= itsInterface.license.Lol_maxBlueEssences)
                        {
                            itsInterface.logger.Log(true, itsInterface.messages.AccountDoneBE);
                            itsInterface.clientKiller.KillAllLeague();
                            await Task.Delay(10000);
                            itsInterface.dashboardHelper.UpdateLolStatus("Finished", itsInterface);
                            return Start(itsInterface);
                        }

                        if ((bool)await CheckInGame(itsInterface))
                        {
                            Console.WriteLine(itsInterface.messages.GameFound);
                            itsInterface.gameAi.YeniAIBaslat(itsInterface);
                        }

                        //if (itsInterface.license.Lol_isEmptyNick == false) // Eğer ! olursa true değeri false, false değeri true döner./
                        //{
                        //    Dispose(true);
                        //    await accountProcess.CheckNewAccount(itsInterface);
                        //}

                        itsInterface.clientKiller.KillRiotUx();

                        itsInterface.ProcessController.SuspendLeagueUx();

                        if (itsInterface.license.Lol_disenchant)
                        {
                            try
                            {
                                DisenchantAgain:
                                await itsInterface.lcuPlugins.CraftKeysAsync();
                                bool isMoreChest = await itsInterface.lcuPlugins.OpenChestsAsync();
                                await itsInterface.lcuPlugins.CraftEventTokenAsync();
                                await itsInterface.lcuPlugins.DisenchantChampionsAsync();
                                if (isMoreChest)
                                {
                                    await Task.Delay(5000);
                                    goto DisenchantAgain;
                                }
                            }
                            catch (Exception e)
                            {
                                itsInterface.logger.Log(true, itsInterface.messages.ErrorDisenchant);
                            }
                        }

                        if (itsInterface.license.Lol_doTutorial)
                        {
                            var summonerTutorials = await itsInterface.lcuPlugins.GetTutorials();
                            foreach (EvelynnLCU.API_Models.Tutorial item in summonerTutorials)
                            {
                                if (item.type == "CARD" && item.status != "COMPLETED")
                                {

                                    await Task.Delay(15000);

                                    itsInterface.newQueue.GameAiBool = true;
                                    itsInterface.gameAi.pickedTutoChamp = false;
                                    itsInterface.queueId = int.Parse(item.queueId);
                                    itsInterface.logger.Log(true, $"Playing Tutorial: {item.stepNumber}");
                                    itsInterface.dashboardHelper.UpdateLolStatus("Playing Tutorial", itsInterface);

                                    await Task.Delay(1500);

                                    await itsInterface.newQueue.DoTutorials(itsInterface);

                                    //Son tutorial oyunu oynandıysa restart
                                    if (itsInterface.queueId == 2020)
                                    {
                                        itsInterface.logger.Log(true, "Tutorial Games are Completed");
                                        await Task.Delay(10000);
                                        await Restart(itsInterface);
                                    }
                                }
                            }
                        }

                        itsInterface.lcuPlugins.KillUXAsync();

                        Dispose(true);
                        return itsInterface.newQueue.Test(itsInterface, false);
                    }
                    else if (processExist("LeagueClient", itsInterface))
                    {
                        // Started Old League Client instead of RiotClient, login from LCU!
                        if (isFromGame == false) { await accountProcess.OldClientLoginAccount(itsInterface); }
                        await accountProcess.CheckLeagueBan(itsInterface);
                        itsInterface.newQueue.itsInterface2 = itsInterface;
                        itsInterface.newQueue.UxEventAsync();
                        await accountProcess.PatchCheck(itsInterface);
                        await itsInterface.lcuPlugins.RemoveNotificationsAsync();
                        await itsInterface.lcuPlugins.GetSetMissions();
                        if (!await accountProcess.GetSetWallet(itsInterface))
                        {
                            itsInterface.clientKiller.KillAllLeague();
                            await Task.Delay(10000);
                            return Start(itsInterface);
                        }

                        await itsInterface.lcuPlugins.ChangeSummonerIconAsync();

                        Dispose(true);

                        if (itsInterface.license.Lol_maxLevel != 0 && itsInterface.summoner.summonerLevel >= itsInterface.license.Lol_maxLevel)
                        {
                            itsInterface.logger.Log(true, itsInterface.messages.AccountDoneXP);
                            itsInterface.clientKiller.KillAllLeague();
                            await Task.Delay(10000);
                            itsInterface.dashboardHelper.UpdateLolStatus("Finished", itsInterface);
                            return Start(itsInterface);
                        }

                        if (itsInterface.license.Lol_maxBlueEssences != 0 && itsInterface.wallet.ip >= itsInterface.license.Lol_maxBlueEssences)
                        {
                            itsInterface.logger.Log(true, itsInterface.messages.AccountDoneBE);
                            itsInterface.clientKiller.KillAllLeague();
                            await Task.Delay(10000);
                            itsInterface.dashboardHelper.UpdateLolStatus("Finished", itsInterface);
                            return Start(itsInterface);
                        }

                        if ((bool)await CheckInGame(itsInterface))
                        {
                            Console.WriteLine(itsInterface.messages.GameFound);
                            itsInterface.gameAi.YeniAIBaslat(itsInterface);
                        }

                        //if (itsInterface.license.Lol_isEmptyNick == false) // Eğer ! olursa true değeri false, false değeri true döner./
                        //{
                        //    Dispose(true);
                        //    await accountProcess.CheckNewAccount(itsInterface);
                        //}

                        if (itsInterface.license.Lol_disenchant)
                        {
                            try
                            {
                                DisenchantAgain:
                                await itsInterface.lcuPlugins.CraftKeysAsync();
                                bool isMoreChest = await itsInterface.lcuPlugins.OpenChestsAsync();
                                await itsInterface.lcuPlugins.CraftEventTokenAsync();
                                await itsInterface.lcuPlugins.DisenchantChampionsAsync();
                                if (isMoreChest)
                                {
                                    await Task.Delay(5000);
                                    goto DisenchantAgain;
                                }
                            }
                            catch (Exception e)
                            {
                                itsInterface.logger.Log(true, itsInterface.messages.ErrorDisenchant);
                            }
                        }

                        if (itsInterface.license.Lol_doTutorial)
                        {
                            var summonerTutorials = await itsInterface.lcuPlugins.GetTutorials();
                            foreach (EvelynnLCU.API_Models.Tutorial item in summonerTutorials)
                            {
                                if (item.type == "CARD" && item.status != "COMPLETED")
                                {
                                    Console.WriteLine(item.queueId);
                                    itsInterface.queueId = int.Parse(item.queueId);
                                    itsInterface.logger.Log(true, $"Playing Tutorial: {item.stepNumber}");
                                    await itsInterface.newQueue.Test(itsInterface, true);
                                }
                            }
                            //accountProcess.TutorialMissions(itsInterface);
                        }

                        itsInterface.lcuPlugins.KillUXAsync();

                        await Task.Delay(15000);

                        itsInterface.lcuPlugins.KillUXAsync();

                        Dispose(true);
                        return itsInterface.newQueue.Test(itsInterface, false);
                    }
                    else
                    {
                        await Restart(itsInterface);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"START ACCOUNT PROCESS HATA {e}");
                Dispose(true);
                await Restart(itsInterface);
            }

            return Task.CompletedTask;
        }

        public async Task<Task> Restart(Interface itsInterface)
        {
            try
            {
                itsInterface.clientKiller.KillAllLeague();
                await Task.Delay(10000);
                itsInterface.clientKiller.RestartAndExit(itsInterface);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return Task.CompletedTask;
        }

        public async Task<object> CheckInGame(Interface itsInterface)
        {
            string gameFlowPhase = await itsInterface.lcuPlugins.GetGameFlowPhaseAsync();
            if (!gameFlowPhase.Contains("InProgress"))
            {
                if (!gameFlowPhase.Contains("Reconnect"))
                {
                    
                    return (bool)false;
                }

                gameFlowPhase = await itsInterface.lcuPlugins.GetGameFlowSessionAsync();
                var queueId = JsonConvert.DeserializeObject<EvelynnLCU.API_Models.LolGameFlowSession>(gameFlowPhase).gameData.queue.id.Value;
                if (queueId != 830)
                {
                    await Task.Delay(30000);
                    return (Task<Task>)itsInterface.processManager.StartAccountProcess(itsInterface);
                }
                await itsInterface.lcuPlugins.ReconnectGameAsync();
                return (bool)true;
            }
            else
            {
                return (bool)true;
            }
        }

        public bool processExist(string win, Interface itsInterface)
        {
            return itsInterface.Result(Convert.ToBoolean(Process.GetProcessesByName(win).Length != 0), "");
        }

        //public void InjectConfigPersist(Interface itsInterface)
        //{
        //    try
        //    {
        //        string text = itsInterface.clientKiller.GetLeaguePath() + "Config\\";
        //        File.Delete($"{text}PersistedSettings.json");
        //        Thread.Sleep(1500);
        //        File.Copy(Directory.GetCurrentDirectory() + "\\Config\\PersistedSettings.json", $"{text}PersistedSettings.json", overwrite: true);
        //        itsInterface.logger.Log(true, "Config Updated");
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //    }
        //}

        public async Task<Task> PlayAgain(Interface itsInterface)
        {
            itsInterface.newQueue.GameAiBool = true;
            int pnC = 0;
            CHECKACTIONS:
            if (itsInterface.dashboard.IsStart) // Dashboard Action Start
            {

                /*
                 * Burda olmasının sebebi eğer Stop olduktan sonra Start gelirse
                 * İlk start gelmiş mi diye kontrol edecek. Sonra Stop u false edecek.
                 */
                itsInterface.dashboard.IsStop = false;
                itsInterface.dashboard.IsStart = false;
                itsInterface.dashboard.IsRestart = false;
                if (pnC == 0){ Console.WriteLine("Panelden Start Geldi!");}
                Dispose(true);
            }

            if (itsInterface.dashboard.IsRestart) // Dashboard Action Restart
            {
                itsInterface.dashboard.IsRestart = false;
                itsInterface.dashboard.IsStop = false;
                itsInterface.dashboard.IsStart = true;
                itsInterface.clientKiller.KillAllLeague();
                if (pnC == 0) { Console.WriteLine("Panelden Restart Geldi!"); }
                itsInterface.clientKiller.RestartAndExit(itsInterface);
                return Task.CompletedTask;
            }

            if (itsInterface.dashboard.IsStop) // Dashboard Action Stop
            {
                if(pnC==0) { Console.WriteLine("Panelden Stop Geldi!") ; }
                itsInterface.isBotStarted = false;
                pnC++;
                await Task.Delay(10000);
                goto CHECKACTIONS;
            }

            Dispose(true);

            using (AccountProcess accountProcess = new AccountProcess())
            {
                itsInterface.ProcessController.SuspendLeagueUx();
                itsInterface.logger.Log(true, itsInterface.messages.InfoStartingAgain);
                //accountProcess.Initialize(itsInterface);
                await itsInterface.lcuPlugins.RemoveNotificationsAsync();
                await itsInterface.lcuPlugins.GetSetMissions();

                //GetSetWallet Riot yüzünden patladığı oluyor (Client Yarım Yükleniyor)
                if (!await accountProcess.GetSetWallet(itsInterface))
                {
                    itsInterface.clientKiller.KillAllLeague();
                    await Restart(itsInterface);
                }

                itsInterface.logger.Log(true,"Level: " + itsInterface.summoner.summonerLevel);

                itsInterface.lcuPlugins.KillUXAsync();

                Dispose(true);

                if (itsInterface.license.Lol_maxLevel != 0 && itsInterface.summoner.summonerLevel >= itsInterface.license.Lol_maxLevel)
                {
                    itsInterface.logger.Log(true, itsInterface.messages.AccountDoneXP);
                    itsInterface.clientKiller.KillAllLeague();
                    await Task.Delay(10000);
                    itsInterface.dashboardHelper.UpdateLolStatus("Finished", itsInterface);
                    return Start(itsInterface);
                }

                if (itsInterface.license.Lol_maxBlueEssences != 0 && itsInterface.wallet.ip >= itsInterface.license.Lol_maxBlueEssences)
                {
                    itsInterface.logger.Log(true, itsInterface.messages.AccountDoneBE);
                    itsInterface.clientKiller.KillAllLeague();
                    await Task.Delay(10000);
                    itsInterface.dashboardHelper.UpdateLolStatus("Finished", itsInterface);
                    return Start(itsInterface);
                }

                if (itsInterface.license.Lol_disenchant)
                {
                    try
                    {
                        DisenchantAgain:
                        await itsInterface.lcuPlugins.CraftKeysAsync();
                        bool isMoreChest = await itsInterface.lcuPlugins.OpenChestsAsync();
                        await itsInterface.lcuPlugins.CraftEventTokenAsync();
                        await itsInterface.lcuPlugins.DisenchantChampionsAsync();
                        if (isMoreChest)
                        {
                            await Task.Delay(5000);
                            goto DisenchantAgain;
                        }
                    }
                    catch (Exception e)
                    {
                        itsInterface.logger.Log(true, itsInterface.messages.ErrorDisenchant);
                    }
                }
                
                itsInterface.newQueue.CreateLobby(itsInterface);
                itsInterface.newQueue.StartMatchmaking();
                Dispose(true);
                return Task.CompletedTask;
            }
        }

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.Collect();
            }
        }

        ~ProcessManager()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            //GC.SuppressFinalize(this);

        }
        #endregion
    }
}
