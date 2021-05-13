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

namespace Evelynn_Bot.ProcessManager
{
    public class ProcessManager
    {
        JsonRead jsonRead = new JsonRead();
        private bool randomController;

        public void Start(License license)
        {
            StartAccountProcess(license);

            while (IsGameStarted() == false)
            {
                Thread.Sleep(15000);
                IsGameStarted();
            }

            Player player = new Player();
            GameAi(player, license);
        }

        public void StartAccountProcess(License license)
        {

            Logger.Log(true, "ID: " + license.Lol_username);
            Logger.Log(true, "Password: " + license.Lol_password);
            Logger.Log(true, "Max Level: " + license.Lol_maxLevel.ToString());
            Logger.Log(true, "Max BE: " + license.Lol_maxBlueEssences);
            Logger.Log(true, "Tutorial: " + license.Lol_doTutorial);
            Logger.Log(true, "Disenchant: " + license.Lol_disenchant.ToString());
            Logger.Log(true, "Empty Nick: " + license.Lol_isEmptyNick);

            license.LeaguePath = jsonRead.Location() + "LeagueClient.exe";

            using (AccountProcess accountProcess = new AccountProcess())
            {
                accountProcess.StartLeague(license);
                accountProcess.LoginAccount(license);
                accountProcess.Initialize();
                accountProcess.GetSetWallet();
                ClientKiller.SuspendLeagueClient();

                if (license.Lol_maxLevel != 0 && AccountProcess.summoner.summonerLevel >= license.Lol_maxLevel)
                {
                    Logger.Log(true, Messages.AccountDoneXP);
                    ClientKiller.KillLeagueClient();
                    DashboardHelper.UpdateLolStatus("Finished", license);
                    Thread.Sleep(15000);
                    Start(license);
                }

                if (license.Lol_maxBlueEssences != 0 && AccountProcess.wallet.ip >= license.Lol_maxBlueEssences)
                {
                    Logger.Log(true, Messages.AccountDoneBE);
                    ClientKiller.KillLeagueClient();
                    DashboardHelper.UpdateLolStatus("Finished", license);
                    Thread.Sleep(15000);
                    Start(license);
                }

                else
                {
                    if (!license.Lol_isEmptyNick)
                    {
                        accountProcess.CheckNewAccount(license);
                    }

                    accountProcess.PatchCheck();

                    if (license.Lol_disenchant)
                    {
                        accountProcess.Disenchant();
                    }

                    if (license.Lol_doTutorial)
                    {
                        accountProcess.TutorialMissions(license);
                    }

                    accountProcess.CreateGame(license);
                    accountProcess.StartQueue(license);
                }
            }
        }

        public bool IsGameStarted()
        {
            using (GameAi gameAi = new GameAi())
            {
                if (gameAi.ImageSearchForGameStart(ImagePaths.game_started, "7", Messages.GameStarted).Success)
                {
                    return true;
                }

                return false;
            }
        }
        public void GameAi(Player player, License license)
        {
            DashboardHelper.UpdateLolStatus("In Game", license);
            Thread.Sleep(15000);
            randomController = true;

            using (GameAi gameAi = new GameAi())
            {
                while (gameAi.ImageSearch(ImagePaths.game_started, "2", Messages.GameStarted).Success)
                {
                    if (randomController)
                    {
                        gameAi.RandomLaner();

                        //TEST
                        AutoItX.Send("{CTRLDOWN}");
                        AutoItX.Send("q");
                        AutoItX.Send("{CTRLUP}");
                        //TEST

                        Thread.Sleep(15000);
                        randomController = false;
                    }

                    gameAi.GoMid();

                    while (gameAi.ImageSearch(ImagePaths.minions, "2", Messages.SuccessMinion).Success)
                    {
                        AutoItX.MouseClick("RIGHT", gameAi.X - 20, gameAi.Y + 40, 1, 1);
                        AutoItX.Send("a");
                        Thread.Sleep(500);
                        AutoItX.Send("a");
                        AutoItX.MouseClick("LEFT", gameAi.X - 23, gameAi.Y + 40, 1, 1);
                        Thread.Sleep(2000);

                        if (gameAi.ImageSearch(ImagePaths.enemy_minions, "2", Messages.SuccessEnemyMinion).Success)
                        {
                            AutoItX.MouseClick("LEFT", gameAi.X + 27, gameAi.Y + 20, 1, 0);
                            AutoItX.Send("q");
                        }

                        if (gameAi.ImageSearch(ImagePaths.enemy_health, "2", Messages.SuccessEnemyChampion).Success)
                        {
                            AutoItX.MouseClick("LEFT", gameAi.X + 65, gameAi.Y + 75, 1, 0);
                            AutoItX.MouseClick("RIGHT", gameAi.X + 65, gameAi.Y + 75, 1, 0);
                            AutoItX.Send("w");
                            AutoItX.Send("a");
                            Thread.Sleep(550);
                            AutoItX.Send("r");
                            AutoItX.Send("a");
                            Thread.Sleep(550);
                            AutoItX.Send("q");
                            AutoItX.Send("a");
                            AutoItX.Send("e");
                            AutoItX.Send("f");
                            AutoItX.Send("t");
                            Thread.Sleep(550);
                            AutoItX.Send("a");
                            gameAi.GoMid();
                            Thread.Sleep(4000);
                        }
                        
                        gameAi.CurrentPlayerStats(player);
                        Console.WriteLine("Can: " + player.CurrentHealth);
                        Console.WriteLine("Altın: " + player.CurrentGold);

                        gameAi.GoMid();
                        Thread.Sleep(2000);

                    }

                    gameAi.GoMid();
                    Thread.Sleep(15000);
                }
            }

            Console.WriteLine("Oyun bitti!");
            Thread.Sleep(60000);
            CHECKACTIONS:
            if (DashboardHelper.req.dashboardActions.IsStop) // Dashboard Action Stop
            {
                goto CHECKACTIONS;
            } 
            if (DashboardHelper.req.dashboardActions.IsRestart) // Dashboard Action Restart
            {
                ClientKiller.KillLeagueClient();
                Start(license);
            }
            else
            {
                PlayAgain(license);
            }
        }
        public void PlayAgain(License license)
        {
            using (AccountProcess accountProcess = new AccountProcess())
            {
                accountProcess.Initialize();
                accountProcess.DoMission();
                accountProcess.GetSetWallet();
                accountProcess.PatchCheck();
                ClientKiller.SuspendLeagueClient();
                if (license.Lol_maxLevel != 0 && AccountProcess.summoner.summonerLevel >= license.Lol_maxLevel)
                {
                    Logger.Log(true, Messages.AccountDoneXP);
                    ClientKiller.KillLeagueClient();
                    DashboardHelper.UpdateLolStatus("Finished", license);
                    Thread.Sleep(15000);
                    Start(license);
                }

                if (license.Lol_maxBlueEssences != 0 && AccountProcess.wallet.ip >= license.Lol_maxBlueEssences)
                {
                    Logger.Log(true, Messages.AccountDoneBE);
                    ClientKiller.KillLeagueClient();
                    DashboardHelper.UpdateLolStatus("Finished", license);
                    Thread.Sleep(15000);
                    Start(license);
                }
                else
                {
                    if (license.Lol_disenchant)
                    {
                        accountProcess.Disenchant();
                    }

                    accountProcess.CreateGame(license);
                    accountProcess.StartQueue(license);

                    while (IsGameStarted() == false)
                    {
                        Thread.Sleep(15000);
                        IsGameStarted();
                    }

                    Player player = new Player();
                    GameAi(player, license);
                }

            }
        }

    }
}
