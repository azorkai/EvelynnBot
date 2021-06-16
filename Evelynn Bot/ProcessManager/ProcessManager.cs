using Evelynn_Bot.Account_Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
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

        public bool Start(Interface itsInterface)
        {
            if (CheckInGame(itsInterface))
            {
                return this.GameAi(itsInterface);
            }

            StartAccountProcess(itsInterface);

            while (IsGameStarted(itsInterface) == false)
            {
                Thread.Sleep(15000);
                IsGameStarted(itsInterface);
            }
            return this.GameAi(itsInterface);
        }

        public bool StartAccountProcess(Interface itsInterface)
        {
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
                    accountProcess.StartLeague(itsInterface);
                    accountProcess.LoginAccount(itsInterface);
                    accountProcess.Initialize(itsInterface);
                    accountProcess.KillUxRender(itsInterface);
                    accountProcess.SelectChampion(itsInterface);
                    accountProcess.GetSetWallet(itsInterface);
                    //ClientKiller.SuspendLeagueClient();
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
                        return accountProcess.CheckNewAccount(itsInterface);
                    }

                    accountProcess.PatchCheck(itsInterface);

                    if (itsInterface.license.Lol_disenchant)
                    {
                        accountProcess.Disenchant(itsInterface);
                    }

                    if (itsInterface.license.Lol_doTutorial)
                    {
                        accountProcess.TutorialMissions(itsInterface);
                    }

                    Thread.Sleep(15000);

                    if (CheckInGame(itsInterface))
                    {
                        return itsInterface.Result(true, "");
                    }

                    accountProcess.CreateGame(itsInterface);
                    accountProcess.KillUxRender(itsInterface);
                    Dispose(true);
                    return accountProcess.StartQueue(itsInterface);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"START ACCOUNT PROCESS HATA {e}");
                Dispose(true);
                return itsInterface.Result(false, "");
            }
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
            while (winExist("League Of Legends.exe", itsInterface))
            {
                return itsInterface.Result(true, "");
            }
            return itsInterface.Result(false, "");
        }

        public bool winExist(string win, Interface itsInterface)
        {
            int a = AutoItX.ProcessExists(win);
            return itsInterface.Result(Convert.ToBoolean(a), "");
        }

        public bool GameAi(Interface itsInterface)
        {
            Thread aiThread = new Thread(() => GameAi2(itsInterface));
            aiThread.Start();
            itsInterface.dashboardHelper.UpdateLolStatus("In Game", itsInterface);
            Thread.Sleep(15000);
            randomController = true;

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

                    while (gameAi.ImageSearch(itsInterface.ImgPaths.minions, "2", itsInterface.messages.SuccessMinion, itsInterface))
                    {
                        gameAi.CurrentPlayerStats(itsInterface);
                        Console.WriteLine("Can: " + itsInterface.player.CurrentHealth);
                        Console.WriteLine("Altın: " + itsInterface.player.CurrentGold);
                        Console.WriteLine("Level: " + itsInterface.player.Level);


                        gameAi.HitMove(gameAi.X, gameAi.Y);
                        Thread.Sleep(500);

                        if (gameAi.ImageSearch(itsInterface.ImgPaths.enemy_minions, "2", itsInterface.messages.SuccessEnemyMinion, itsInterface))
                        {
                            AutoItX.MouseClick("RIGHT", gameAi.X + 27, gameAi.Y + 20, 1, 0);
                            AutoItX.Send("q");
                        }

                        if (gameAi.ImageSearch(itsInterface.ImgPaths.enemy_health, "2", itsInterface.messages.SuccessEnemyChampion, itsInterface))
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
                                gameAi.SkillUp("e", "m");
                                break;
                            case 6:
                                gameAi.SkillUp("r", "l");
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

            Console.WriteLine("Oyun bitti!");
            aiThread.Abort();
            Dispose(true);
            using (AccountProcess accountProcess = new AccountProcess())
            {
                accountProcess.Initialize(itsInterface);
                Thread.Sleep(3500);
                accountProcess.KillUxRender(itsInterface);
            }

            Thread.Sleep(70000);
            
            CHECKACTIONS:
            if (itsInterface.dashboard.IsStart) // Dashboard Action Start
            {
                /*
                 * Burda olmasının sebebi eğer Stop olduktan sonra Start gelirse
                 * İlk start gelmiş mi diye kontrol edecek. Sonra Stop u false edecek.
                 */
                itsInterface.dashboard.IsStop = false;
                itsInterface.dashboard.IsStart = false;
                Console.WriteLine("Panelden Start Geldi!");
                Dispose(true);
                return PlayAgain(itsInterface);
            }

            if (itsInterface.dashboard.IsStop) // Dashboard Action Stop
            {
                Console.WriteLine("Panelden Stop Geldi!");
                goto CHECKACTIONS;
            }

            if (itsInterface.dashboard.IsRestart) // Dashboard Action Restart
            {
                itsInterface.dashboard.IsRestart = false;
                Console.WriteLine("Panelden Restart Geldi!");
                itsInterface.clientKiller.KillLeagueClient();
                Dispose(true);
                return Start(itsInterface);
            }

            Dispose(true);
            return PlayAgain(itsInterface);


        }

        public bool GameAi2(Interface itsInterface)
        {
            Console.WriteLine("Thread 2 Test!");
            using (GameAi gameAi = new GameAi())
            {
                float attackPercentage = ((itsInterface.player.MaxHealth - itsInterface.player.CurrentHealth) * 100) / itsInterface.player.CurrentHealth;
                if ((int)attackPercentage >= 55 && itsInterface.player.CurrentHealth != 0) // Eğer gelen saldırıdaki can yüzde 30 dan fazla olursa base'e git.
                {
                    Console.WriteLine("Can çok azaldı, bir tık geri çekilme zamanı!");
                    AutoItX.Send("f");
                    AutoItX.MouseClick("RIGHT", gameAi.game_X + 31, gameAi.game_Y - 19, 1, 0);
                    AutoItX.MouseClick("RIGHT", gameAi.game_X + 31, gameAi.game_Y - 19, 1, 0);
                    AutoItX.Send("d");
                    AutoItX.MouseClick("RIGHT", gameAi.game_X + 31, gameAi.game_Y - 19, 1, 0);
                    AutoItX.MouseClick("RIGHT", gameAi.game_X + 31, gameAi.game_Y - 19, 1, 0);
                    Thread.Sleep(6000);
                }

                var maxHealth = itsInterface.player.MaxHealth;
                var baseHealth = maxHealth / 2.7f;
                var currentHealth = itsInterface.player.CurrentHealth;

                if (itsInterface.player.CurrentGold > 3000)
                {
                    Console.WriteLine("Gold sınırı, base!");
                    gameAi.GoBase();
                }

                if (currentHealth <= baseHealth)
                {
                    Console.WriteLine("Can sınırı, base!");
                    gameAi.GoBase();
                }
            }

            Thread.Sleep(1000);

            return itsInterface.Result(true, "");
        }

        public bool PlayAgain(Interface itsInterface)
        {
            using (AccountProcess accountProcess = new AccountProcess())
            {
                Console.WriteLine("Yeni oyun başlatılıyor!");
                accountProcess.Initialize(itsInterface);
                accountProcess.SelectChampion(itsInterface);
                accountProcess.GetSetWallet(itsInterface);
                accountProcess.PatchCheck(itsInterface);
                accountProcess.KillUxRender(itsInterface);
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
                    accountProcess.Disenchant(itsInterface);
                }

                accountProcess.CreateGame(itsInterface);
                accountProcess.StartQueue(itsInterface);

                while (IsGameStarted(itsInterface) == false)
                {
                    Thread.Sleep(15000);
                    IsGameStarted(itsInterface);
                }
                
                Dispose(true);
                return this.GameAi(itsInterface);

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
            GC.SuppressFinalize(this);

        }
        #endregion


    }
}
