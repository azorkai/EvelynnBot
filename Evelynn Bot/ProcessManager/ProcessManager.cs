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
                accountProcess.DoMission();
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
                while (gameAi.ImageSearchForGameStart(ImagePaths.game_started, "2", Messages.GameStarted).Success)
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

                    while (gameAi.ImageSearch(ImagePaths.minions, "2", Messages.SuccessMinion).Success)
                    {
                        gameAi.SkillUp("q", "j");
                        gameAi.SkillUp("w", "k");
                        gameAi.SkillUp("e", "m");
                        gameAi.SkillUp("r", "l");

                        gameAi.HitMove(gameAi.X, gameAi.Y);
                        Thread.Sleep(500);

                        if (gameAi.ImageSearch(ImagePaths.enemy_minions, "2", Messages.SuccessEnemyMinion).Success)
                        {
                            AutoItX.MouseClick("RIGHT", gameAi.X + 27, gameAi.Y + 20, 1, 0);
                            AutoItX.Send("q");
                        }

                        if (gameAi.ImageSearch(ImagePaths.enemy_health, "2", Messages.SuccessEnemyChampion).Success)
                        {
                            AutoItX.MouseClick("RIGHT", gameAi.X + 65, gameAi.Y + 75, 1, 0);
                            gameAi.HitMove(gameAi.X, gameAi.Y);
                            gameAi.Combo(gameAi.X, gameAi.Y);
                            Thread.Sleep(1500);
                        }

                        gameAi.CurrentPlayerStats(player);
                        Console.WriteLine("Can: " + player.CurrentHealth);
                        Console.WriteLine("Altın: " + player.CurrentGold);

                        if (player.CurrentGold > 3000)
                        {
                            gameAi.GoBase();
                        }

                        var maxHealth = player.MaxHealth;
                        var baseHealth = maxHealth / 2.7f;
                        var currentHealth = player.CurrentHealth;

                        if (currentHealth <= baseHealth)
                        {
                            gameAi.GoBase();
                        }

                        Thread.Sleep(1000);
                    }

                    gameAi.GoMid();
                    Thread.Sleep(1500);
                }

            }

            Console.WriteLine("Oyun bitti!");

            Thread.Sleep(70000);

            CHECKACTIONS:
            if (DashboardHelper.req.dashboardActions.IsStart) // Dashboard Action Start
            {
                /*
                 * Burda olmasının sebebi eğer Stop olduktan sonra Start gelirse
                 * İlk start gelmiş mi diye kontrol edecek. Sonra Stop u false edecek.
                 */
                DashboardHelper.req.dashboardActions.IsStop = false;
                DashboardHelper.req.dashboardActions.IsStart = false;
                Console.WriteLine("Panelden Start Geldi!");

                /* Else kaldırdık çünkü restarta geldiği zaman altına play again runlanmaması lazım
                 * Stoptan sonra start gelirse zaten oynamaya devam etmesini isteyeceğiz.
                 */
                PlayAgain(license);
            }

            if (DashboardHelper.req.dashboardActions.IsStop) // Dashboard Action Stop
            {
                Console.WriteLine("Panelden Stop Geldi!");
                goto CHECKACTIONS;
            } 

            if (DashboardHelper.req.dashboardActions.IsRestart) // Dashboard Action Restart
            {
                DashboardHelper.req.dashboardActions.IsRestart = false;
                Console.WriteLine("Panelde Restart Geldi!");
                ClientKiller.KillLeagueClient();
                Start(license);
            }

        }
        public void PlayAgain(License license)
        {
            using (AccountProcess accountProcess = new AccountProcess())
            {
                Console.WriteLine("Yeni oyun başlatılıyor!");
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
