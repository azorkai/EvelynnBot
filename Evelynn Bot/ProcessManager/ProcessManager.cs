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
using EvelynnLCU.Plugins.LoL;
using Newtonsoft.Json;
using Timer = System.Threading.Timer;

namespace Evelynn_Bot.ProcessManager
{
    public class ProcessManager : IProcessManager
    {
        private Random random = new Random();
        
        private bool danger;
        private float lastHealth;
        private float currentHealth;

        public static System.Timers.Timer DamageCheckerTimer = new System.Timers.Timer();
        public static System.Timers.Timer ExtremeGameTimer = new System.Timers.Timer();

        private int ExtremeGameTime;
        private bool randomController;

        public async Task<Task> Start(Interface itsInterface)
        {

            //await itsInterface.clientKiller.ExecuteBypass();

            await StartAccountProcess(itsInterface);

            //itsInterface.gameAi.YeniAIBaslat(itsInterface);

            //while (IsGameStarted(itsInterface) == false)
            //{
            //    Thread.Sleep(15000);
            //    IsGameStarted(itsInterface);
            //}
            return Task.CompletedTask;
        }

        public async Task<Task> StartAccountProcess(Interface itsInterface, bool isFromGame = false)
        {
            itsInterface.newQueue.bugTimer.Stop();
            itsInterface.clientKiller.KillAllLeague();
            // BURASI YENI AI'E GÖRE GÜZELCE Bİ REFACTOR EDİLECEK (isFromGame true)
            try
            {
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
                        Thread.Sleep(15000);
                        itsInterface.lcuApi.BeginTryInitRiotClient();
                    }
                    if (processExist("RiotClientServices", itsInterface))
                    {
                        if (isFromGame == false) { await accountProcess.LoginAccount(itsInterface); }
                        Thread.Sleep(5000);
                        accountProcess.Initialize(itsInterface);
                        itsInterface.lcuPlugins.KillUXAsync();

                        try
                        {
                            string session = await accountProcess.VerifySession(itsInterface);
                            itsInterface.logger.Log(false, session);

                            switch (session)
                            {
                                case "banned_account":
                                    // hesabı panele gönder ve yenisini al.
                                    Console.WriteLine("BANNED ACCOUNT, GETTING NEW ACCOUNT");
                                    break;
                                case "new_player_set_account":
                                    await itsInterface.lcuPlugins.CompleteNewAccountAsync();
                                    Console.WriteLine("NEW PLAYER, SETTING UP NEW PLAYER");
                                    if (itsInterface.license.Lol_isEmptyNick == false)
                                    {
                                        Dispose(true);
                                        await accountProcess.CheckNewAccount(itsInterface);
                                    }
                                    break;
                                case "invalid_credentials":
                                    Console.WriteLine("INVALID CREDENTIALS, GETTING NEW ACCOUNT");
                                    break;
                                case "restart_client_error":
                                    Console.WriteLine("CLIENT ERROR! RESTART");

                                    return itsInterface.processManager.StartAccountProcess(itsInterface);
                                case "invalid_summoner_name":
                                    Console.WriteLine("We have a invalid summoner name!");
                                    break;
                                case "logged_in_from_another":
                                    Console.WriteLine("This account has logged in from somewhere else already!");
                                    break;
                                default:
                                    Console.WriteLine($"LOGIN SESSION: {session}");
                                    break;
                            }
                        }

                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        
                        Thread.Sleep(3500);

                        try
                        {
                            var eula = await itsInterface.lcuPlugins.GetEula("read");
                            if (eula.Equals("\"AcceptanceRequired\""))
                            {
                                await itsInterface.lcuPlugins.GetEula("accept");
                            }
                        }
                        catch
                        {
                            //ignored
                        }

                        await accountProcess.CheckLeagueBan(itsInterface);

                        itsInterface.newQueue.itsInterface2 = itsInterface;
                        itsInterface.newQueue.UxEventAsync();
                        accountProcess.PatchCheck(itsInterface); //websocket subscribe olunacak _work işi done koyulacak \\ Gereksiz belki ıh degil ihh yani

                        try
                        {
                            await itsInterface.lcuPlugins.RemoveNotificationsAsync();
                        }
                        catch (Exception e)
                        {
                            
                        }

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
                            itsInterface.dashboardHelper.UpdateLolStatus("Finished", itsInterface);
                            var licenseBase64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(itsInterface.license)));
                            var exeDir = System.Reflection.Assembly.GetExecutingAssembly().Location;
                            Process eBot = new Process();
                            eBot.StartInfo.FileName = exeDir;
                            eBot.StartInfo.WorkingDirectory = Path.GetDirectoryName(exeDir);
                            eBot.StartInfo.Arguments = licenseBase64String;
                            eBot.StartInfo.Verb = "runas";
                            eBot.Start();
                            Environment.Exit(0);
                            return Start(itsInterface);
                        }

                        if (itsInterface.license.Lol_maxBlueEssences != 0 && itsInterface.wallet.ip >= itsInterface.license.Lol_maxBlueEssences)
                        {
                            itsInterface.logger.Log(true, itsInterface.messages.AccountDoneBE);
                            itsInterface.clientKiller.KillAllLeague();
                            itsInterface.dashboardHelper.UpdateLolStatus("Finished", itsInterface);
                            var licenseBase64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(itsInterface.license)));
                            var exeDir = System.Reflection.Assembly.GetExecutingAssembly().Location;
                            Process eBot = new Process();
                            eBot.StartInfo.FileName = exeDir;
                            eBot.StartInfo.WorkingDirectory = Path.GetDirectoryName(exeDir);
                            eBot.StartInfo.Arguments = licenseBase64String;
                            eBot.StartInfo.Verb = "runas";
                            eBot.Start();
                            Environment.Exit(0);
                            return Start(itsInterface);
                        }

                        if ((bool)await CheckInGame(itsInterface))
                        {
                            Console.WriteLine(itsInterface.messages.GameFound);
                            itsInterface.gameAi.YeniAIBaslat(itsInterface);
                        }

                        if (itsInterface.license.Lol_isEmptyNick == false) // Eğer ! olursa true değeri false, false değeri true döner./
                        {
                            Dispose(true);
                            await accountProcess.CheckNewAccount(itsInterface);
                        }

                        if (itsInterface.license.Lol_disenchant)
                        {
                            try
                            {
                                DisenchantAgain:
                                await itsInterface.lcuPlugins.CraftKeysAsync();
                                bool isMoreChest = await itsInterface.lcuPlugins.OpenChestsAsync();
                                await itsInterface.lcuPlugins.CraftChampionShardAsync();
                                await itsInterface.lcuPlugins.DisenchantChampionsAsync();
                                if (isMoreChest)
                                {
                                    Thread.Sleep(5000);
                                    goto DisenchantAgain;
                                }
                            }
                            catch (Exception e)
                            {
                                
                            }
                        }

                        if (itsInterface.license.Lol_doTutorial)
                        {
                            //accountProcess.TutorialMissions(itsInterface);
                        }

                        itsInterface.lcuPlugins.KillUXAsync();

                        Dispose(true);
                        return itsInterface.newQueue.Test(itsInterface);
                    }
                    else if (processExist("LeagueClient", itsInterface))
                    {
                        // Started Old League Client instead of RiotClient, login from LCU!
                        if (isFromGame == false) { await accountProcess.OldClientLoginAccount(itsInterface); }
                        await accountProcess.CheckLeagueBan(itsInterface);
                        itsInterface.newQueue.itsInterface2 = itsInterface;
                        itsInterface.newQueue.UxEventAsync();
                        accountProcess.PatchCheck(itsInterface); //websocket subscribe olunacak _work işi done koyulacak \\ Gereksiz belki ıh yani
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
                            itsInterface.dashboardHelper.UpdateLolStatus("Finished", itsInterface);
                            var licenseBase64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(itsInterface.license)));
                            var exeDir = System.Reflection.Assembly.GetExecutingAssembly().Location;
                            Process eBot = new Process();
                            eBot.StartInfo.FileName = exeDir;
                            eBot.StartInfo.WorkingDirectory = Path.GetDirectoryName(exeDir);
                            eBot.StartInfo.Arguments = licenseBase64String;
                            eBot.StartInfo.Verb = "runas";
                            eBot.Start();
                            Environment.Exit(0);
                            return Start(itsInterface);
                        }

                        if (itsInterface.license.Lol_maxBlueEssences != 0 && itsInterface.wallet.ip >= itsInterface.license.Lol_maxBlueEssences)
                        {
                            itsInterface.logger.Log(true, itsInterface.messages.AccountDoneBE);
                            itsInterface.clientKiller.KillAllLeague();
                            itsInterface.dashboardHelper.UpdateLolStatus("Finished", itsInterface);
                            var licenseBase64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(itsInterface.license)));
                            var exeDir = System.Reflection.Assembly.GetExecutingAssembly().Location;
                            Process eBot = new Process();
                            eBot.StartInfo.FileName = exeDir;
                            eBot.StartInfo.WorkingDirectory = Path.GetDirectoryName(exeDir);
                            eBot.StartInfo.Arguments = licenseBase64String;
                            eBot.StartInfo.Verb = "runas";
                            eBot.Start();
                            Environment.Exit(0);
                            return Start(itsInterface);
                        }

                        if ((bool)await CheckInGame(itsInterface))
                        {
                            Console.WriteLine(itsInterface.messages.GameFound);
                            itsInterface.gameAi.YeniAIBaslat(itsInterface);
                        }

                        if (itsInterface.license.Lol_isEmptyNick == false) // Eğer ! olursa true değeri false, false değeri true döner./
                        {
                            Dispose(true);
                            await accountProcess.CheckNewAccount(itsInterface);
                        }

                        if (itsInterface.license.Lol_disenchant)
                        {
                            DisenchantAgain:
                            await itsInterface.lcuPlugins.CraftKeysAsync();
                            bool isMoreChest = await itsInterface.lcuPlugins.OpenChestsAsync();
                            await itsInterface.lcuPlugins.CraftChampionShardAsync();
                            await itsInterface.lcuPlugins.DisenchantChampionsAsync();
                            if (isMoreChest)
                            {
                                Thread.Sleep(5000);
                                goto DisenchantAgain;
                            }
                        }

                        if (itsInterface.license.Lol_doTutorial)
                        {
                            //accountProcess.TutorialMissions(itsInterface);
                        }

                        itsInterface.lcuPlugins.KillUXAsync();

                        await Task.Delay(15000);

                        itsInterface.lcuPlugins.KillUXAsync();

                        Dispose(true);
                        return itsInterface.newQueue.Test(itsInterface);
                    }
                    else
                    {
                        return StartAccountProcess(itsInterface);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"START ACCOUNT PROCESS HATA {e}");
                Dispose(true);
                return StartAccountProcess(itsInterface);
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
                    // LOBBY AÇTIRT
                    return (bool)false;
                }

                gameFlowPhase = await itsInterface.lcuPlugins.GetGameFlowSessionAsync();
                var queueId = JsonConvert.DeserializeObject<EvelynnLCU.API_Models.LolGameFlowSession>(gameFlowPhase).gameData.queue.id.Value;
                if (queueId != 830)
                {
                    Thread.Sleep(30000);
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

        //private void DamageChecker(object source, ElapsedEventArgs e, Interface itsInterface, GameAi gameAi)
        //{
        //    danger = false;
        //    gameAi.CurrentPlayerStats(itsInterface);
        //    lastHealth = itsInterface.player.CurrentHealth;

        //    Thread.Sleep(2500);

        //    gameAi.CurrentPlayerStats(itsInterface);
        //    currentHealth = itsInterface.player.CurrentHealth;

        //    if (lastHealth - currentHealth > 180)
        //    {
        //        Console.WriteLine("Tehlikeli Durum! Büyük Can Kaybı Yaşandı!");
        //        danger = true;
        //        Dispose(true);
        //        Thread.Sleep(1000);
        //    }
        //    else
        //    {
        //        danger = false;
        //    }
        //}

        public async Task<Task> PlayAgain(Interface itsInterface)
        {
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
                var licenseBase64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(itsInterface.license)));
                var exeDir = System.Reflection.Assembly.GetExecutingAssembly().Location;
                Process eBot = new Process();
                eBot.StartInfo.FileName = exeDir;
                eBot.StartInfo.WorkingDirectory = Path.GetDirectoryName(exeDir);
                eBot.StartInfo.Arguments = licenseBase64String;
                eBot.StartInfo.Verb = "runas";
                eBot.Start();
                Environment.Exit(0);
                return Task.CompletedTask;
            }

            if (itsInterface.dashboard.IsStop) // Dashboard Action Stop
            {
                if(pnC==0) { Console.WriteLine("Panelden Stop Geldi!") ; }
                itsInterface.isBotStarted = false;
                pnC++;
                Thread.Sleep(10000);
                goto CHECKACTIONS;
            }

            Dispose(true);

            using (AccountProcess accountProcess = new AccountProcess())
            {
                itsInterface.logger.Log(true, itsInterface.messages.InfoStartingAgain);
                //accountProcess.Initialize(itsInterface);
                await itsInterface.lcuPlugins.RemoveNotificationsAsync();
                await itsInterface.lcuPlugins.GetSetMissions();

                //GetSetWallet Riot yüzünden patladığı oluyor (Client Yarım Yükleniyor)
                if (!await accountProcess.GetSetWallet(itsInterface))
                {
                    itsInterface.clientKiller.KillAllLeague();
                    return Start(itsInterface);
                }

                Console.WriteLine(itsInterface.summoner.summonerLevel);

                itsInterface.lcuPlugins.KillUXAsync();

                Dispose(true);

                if (itsInterface.license.Lol_maxLevel != 0 && itsInterface.summoner.summonerLevel >= itsInterface.license.Lol_maxLevel)
                {
                    itsInterface.logger.Log(true, itsInterface.messages.AccountDoneXP);
                    
                    itsInterface.clientKiller.KillAllLeague();
                    itsInterface.dashboardHelper.UpdateLolStatus("Finished", itsInterface);
                    var licenseBase64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(itsInterface.license)));
                    var exeDir = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    Process eBot = new Process();
                    eBot.StartInfo.FileName = exeDir;
                    eBot.StartInfo.WorkingDirectory = Path.GetDirectoryName(exeDir);
                    eBot.StartInfo.Arguments = licenseBase64String;
                    eBot.StartInfo.Verb = "runas";
                    eBot.Start();
                    Environment.Exit(0);
                    return Start(itsInterface);
                }

                if (itsInterface.license.Lol_maxBlueEssences != 0 && itsInterface.wallet.ip >= itsInterface.license.Lol_maxBlueEssences)
                {
                    itsInterface.logger.Log(true, itsInterface.messages.AccountDoneBE);
                    itsInterface.clientKiller.KillAllLeague();
                    itsInterface.dashboardHelper.UpdateLolStatus("Finished", itsInterface);
                    var licenseBase64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(itsInterface.license)));
                    var exeDir = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    Process eBot = new Process();
                    eBot.StartInfo.FileName = exeDir;
                    eBot.StartInfo.WorkingDirectory = Path.GetDirectoryName(exeDir);
                    eBot.StartInfo.Arguments = licenseBase64String;
                    eBot.StartInfo.Verb = "runas";
                    eBot.Start();
                    Environment.Exit(0);
                    return Start(itsInterface);
                }

                if (itsInterface.license.Lol_disenchant)
                {
                    DisenchantAgain:
                    await itsInterface.lcuPlugins.CraftKeysAsync();
                    bool isMoreChest = await itsInterface.lcuPlugins.OpenChestsAsync();
                    await itsInterface.lcuPlugins.CraftChampionShardAsync();
                    await itsInterface.lcuPlugins.DisenchantChampionsAsync();
                    if (isMoreChest)
                    {
                        Thread.Sleep(5000);
                        goto DisenchantAgain;
                    }
                }
                
                itsInterface.newQueue.CreateLobby();

                Dispose(true);
                return Task.CompletedTask;
            }
        }

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                GC.Collect();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
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
