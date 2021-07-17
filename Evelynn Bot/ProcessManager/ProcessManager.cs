using Evelynn_Bot.Account_Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using AutoIt;
using bAUTH;
using Evelynn_Bot.Constants;
using Evelynn_Bot.Entities;
using Evelynn_Bot.ExternalCommands;
using Evelynn_Bot.GameAI;
using Evelynn_Bot.League_API.GameData;
using Evelynn_Bot.ProcessManager;

namespace Evelynn_Bot.ProcessManager
{
    public class ProcessManager : IProcessManager
    {
        
        private bool randomController;

        public async Task<Task> Start(Interface itsInterface)
        {
            if (CheckInGame(itsInterface))
            {
                return GameAi(itsInterface);
            }

            await StartAccountProcess(itsInterface);

            while (IsGameStarted(itsInterface) == false)
            {
                Thread.Sleep(15000);
                IsGameStarted(itsInterface);
            }
            return Task.CompletedTask;
        }

        public async Task<Task> StartAccountProcess(Interface itsInterface)
        {
            NewQueue.bugTimer.Stop();

            try
            {
                itsInterface.logger.Log(true, "ID: " + itsInterface.license.Lol_username);
                itsInterface.logger.Log(true, "Password: " + itsInterface.license.Lol_password);
                itsInterface.logger.Log(true, "Max Level: " + itsInterface.license.Lol_maxLevel.ToString());
                itsInterface.logger.Log(true, "Max BE: " + itsInterface.license.Lol_maxBlueEssences);
                itsInterface.logger.Log(true, "Tutorial: " + itsInterface.license.Lol_doTutorial);
                itsInterface.logger.Log(true, "Disenchant: " + itsInterface.license.Lol_disenchant.ToString());
                itsInterface.logger.Log(true, "Empty Nick: " + itsInterface.license.Lol_isEmptyNick);
                itsInterface.license.LeaguePath = itsInterface.jsonRead.Location() + "LeagueClient.exe";

                using (AccountProcess accountProcess = new AccountProcess())
                {
                    itsInterface.clientKiller.KillLeagueClient();
                    accountProcess.StartLeague(itsInterface);
                    await accountProcess.LoginAccount(itsInterface);
                    accountProcess.Initialize(itsInterface);

                    itsInterface.lcuPlugins.KillUXAsync();

                    if (!await accountProcess.GetSetWallet(itsInterface))
                    {
                        return StartAccountProcess(itsInterface);
                    }

                    Dispose(true);

                    if (itsInterface.license.Lol_maxLevel != 0 && itsInterface.summoner.summonerLevel >= itsInterface.license.Lol_maxLevel)
                    {
                        itsInterface.logger.Log(true, itsInterface.messages.AccountDoneXP);
                        itsInterface.clientKiller.KillLeagueClient();
                        itsInterface.dashboardHelper.UpdateLolStatus("Finished", itsInterface);
                        Thread.Sleep(15000);
                        Dispose(true);
                        return Start(itsInterface);
                    }

                    if (itsInterface.license.Lol_maxBlueEssences != 0 && itsInterface.wallet.ip >= itsInterface.license.Lol_maxBlueEssences)
                    {
                        itsInterface.logger.Log(true, itsInterface.messages.AccountDoneBE);
                        itsInterface.clientKiller.KillLeagueClient();
                        itsInterface.dashboardHelper.UpdateLolStatus("Finished", itsInterface);
                        Thread.Sleep(15000);
                        Dispose(true);
                        return Start(itsInterface);
                    }

                    if (itsInterface.license.Lol_isEmptyNick == false) // Eğer ! olursa true değeri false, false değeri true döner./
                    {
                        Dispose(true);
                        await accountProcess.CheckNewAccount(itsInterface);
                    }

                    accountProcess.PatchCheck(itsInterface); //websocket subscribe olunacak _work işi done koyulacak

                    if (itsInterface.license.Lol_disenchant)
                    {
                        await itsInterface.lcuPlugins.DisenchantSummonerCapsules();
                    }

                    if (itsInterface.license.Lol_doTutorial)
                    {
                        accountProcess.TutorialMissions(itsInterface);
                    }

                    if (CheckInGame(itsInterface))
                    {
                        Console.WriteLine(itsInterface.messages.GameFound);
                        return GameAi(itsInterface);
                    }

                    await Task.Delay(15000);

                    itsInterface.lcuPlugins.KillUXAsync();

                    Dispose(true);
                    return itsInterface.newQueue.Test(itsInterface);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"START ACCOUNT PROCESS HATA {e}");
                Dispose(true);
                itsInterface.Result(false, "");
            }
            return Task.CompletedTask;
        }

        public bool IsGameStarted(Interface itsInterface)
        {
            using (GameAi gameAi = new GameAi())
            {
                if (gameAi.ImageSearchForGameStart(itsInterface.ImgPaths.game_started, "7", itsInterface.messages.GameStarted, itsInterface))
                {
                    Dispose(true);
                    return itsInterface.Result(true, "");
                }
                Dispose(true);
                return itsInterface.Result(false, "");
            }
        }

        public bool CheckInGame(Interface itsInterface)
        {
            while (processExist("League of Legends.exe", itsInterface))
            {
                return itsInterface.Result(true, "");
            }
            return itsInterface.Result(false, "");
        }

        public bool processExist(string win, Interface itsInterface)
        {
            int a = AutoItX.ProcessExists(win);
            return itsInterface.Result(Convert.ToBoolean(a), "");
        }
        public bool winExist(string win, Interface itsInterface)
        {
            int a = AutoItX.WinExists(win);
            return itsInterface.Result(Convert.ToBoolean(a), "");
        }

        public async Task<Task> GameAi(Interface itsInterface)
        {
            itsInterface.dashboardHelper.UpdateLolStatus("In Game", itsInterface);
            Thread.Sleep(15000);
            randomController = true;

            while(processExist("League of Legends.exe", itsInterface))
            {
                using (GameAi gameAi = new GameAi())
                {
                    while (gameAi.ImageSearchForGameStart(itsInterface.ImgPaths.game_started, "2", itsInterface.messages.GameStarted, itsInterface))
                    {
                        if (randomController)
                        {
                            gameAi.RandomLaner();

                            Thread.Sleep(9000);
                            randomController = false;
                        }
                        else
                        {
                            gameAi.GoMid();
                        }

                        gameAi.CurrentPlayerStats(itsInterface);

                        if (itsInterface.player.CurrentHealth < 15)
                        {
                            Dispose(true);
                        }

                        while (gameAi.ImageSearch(itsInterface.ImgPaths.minions, "2", itsInterface.messages.SuccessMinion, itsInterface) && itsInterface.player.CurrentHealth > 0)
                        {
                            if (itsInterface.player.CurrentHealth < 15)
                            {
                                Dispose(true);
                            }
                            gameAi.CurrentPlayerStats(itsInterface);
                            gameAi.HitMove(gameAi.X, gameAi.Y);
                            Thread.Sleep(500);

                            if (gameAi.ImageSearch(itsInterface.ImgPaths.enemy_minions, "2", itsInterface.messages.SuccessEnemyMinion, itsInterface) && itsInterface.player.CurrentHealth > 0)
                            {
                                AutoItX.MouseClick("RIGHT", gameAi.X + 27, gameAi.Y + 20, 1, 0);
                                AutoItX.Send("q");
                            }

                            if (gameAi.ImageSearch(itsInterface.ImgPaths.enemy_health, "3", itsInterface.messages.SuccessEnemyChampion, itsInterface) && itsInterface.player.CurrentHealth > 0)
                            {
                                AutoItX.MouseClick("RIGHT", gameAi.X + 65, gameAi.Y + 75, 1, 0);
                                gameAi.HitMove(gameAi.X, gameAi.Y);
                                gameAi.Combo(gameAi.X, gameAi.Y);
                                Thread.Sleep(1500);
                            }

                            switch (itsInterface.player.Level)
                            {
                                case 1:
                                    gameAi.SkillUp("q", "j");
                                    break;
                                case 2:
                                    gameAi.SkillUp("w", "k");
                                    break;
                                case 3:
                                    gameAi.SkillUp("e", "m");
                                    break;
                                case 4:
                                    gameAi.SkillUp("q", "j");
                                    break;
                                case 5:
                                    gameAi.SkillUp("q", "j");
                                    break;
                                case 6:
                                    gameAi.SkillUp("r", "l");
                                    break;
                                case 7:
                                    gameAi.SkillUp("q", "j");
                                    break;
                                case 8:
                                    gameAi.SkillUp("w", "k");
                                    break;
                                case 9:
                                    gameAi.SkillUp("q", "j");
                                    break;
                                case 10:
                                    gameAi.SkillUp("w", "k");
                                    break;
                                case 11:
                                    gameAi.SkillUp("r", "l");
                                    break;
                                case 12:
                                    gameAi.SkillUp("w", "k");
                                    break;
                                case 13:
                                    gameAi.SkillUp("w", "k");
                                    break;
                                case 14:
                                    gameAi.SkillUp("e", "m");
                                    break;
                                case 15:
                                    gameAi.SkillUp("e", "m");
                                    break;
                                case 16:
                                    gameAi.SkillUp("r", "l");
                                    break;
                                case 17:
                                    gameAi.SkillUp("e", "m");
                                    break;
                                case 18:
                                    gameAi.SkillUp("e", "m");
                                    break;

                                default:
                                    gameAi.SkillUp("q", "j");
                                    gameAi.SkillUp("w", "k");
                                    gameAi.SkillUp("e", "m");
                                    gameAi.SkillUp("r", "l");
                                    break;
                            }

                            Thread.Sleep(1000);
                        }

                        gameAi.GoMid();
                        Thread.Sleep(1500);
                    }
                }
            }
            Dispose(true);
            return Task.CompletedTask;
        }

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
                if (pnC == 0) { Console.WriteLine("Panelden Restart Geldi!"); }
                itsInterface.clientKiller.KillLeagueClient();
                Dispose(true);
                return Start(itsInterface);
            }

            if (itsInterface.dashboard.IsStop) // Dashboard Action Stop
            {
                if(pnC==0) { Console.WriteLine("Panelden Stop Geldi!") ; }
                pnC++;
                goto CHECKACTIONS;
            }

            Dispose(true);

            using (AccountProcess accountProcess = new AccountProcess())
            {
                itsInterface.logger.Log(true, itsInterface.messages.InfoStartingAgain);
                //accountProcess.Initialize(itsInterface);
                await itsInterface.lcuPlugins.GetSetMissions();

                //GetSetWallet Riot yüzünden patladığı oluyor (Client Yarım Yükleniyor)
                if (!await accountProcess.GetSetWallet(itsInterface))
                {
                    //RestartBot
                    return StartAccountProcess(itsInterface);
                }

                accountProcess.PatchCheck(itsInterface);
                itsInterface.lcuPlugins.KillUXAsync();

                Dispose(true);

                if (itsInterface.license.Lol_maxLevel != 0 && itsInterface.summoner.summonerLevel >= itsInterface.license.Lol_maxLevel)
                {
                    itsInterface.logger.Log(true, itsInterface.messages.AccountDoneXP);
                    itsInterface.clientKiller.KillLeagueClient();
                    itsInterface.dashboardHelper.UpdateLolStatus("Finished", itsInterface);
                    Thread.Sleep(15000);
                    Dispose(true);
                    return Start(itsInterface);
                }

                if (itsInterface.license.Lol_maxBlueEssences != 0 && itsInterface.wallet.ip >= itsInterface.license.Lol_maxBlueEssences)
                {
                    itsInterface.logger.Log(true, itsInterface.messages.AccountDoneBE);
                    itsInterface.clientKiller.KillLeagueClient();
                    itsInterface.dashboardHelper.UpdateLolStatus("Finished", itsInterface);
                    Thread.Sleep(15000);
                    Dispose(true);
                    return Start(itsInterface);
                }

                if (itsInterface.license.Lol_disenchant)
                {
                    await itsInterface.lcuPlugins.DisenchantSummonerCapsules();
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
