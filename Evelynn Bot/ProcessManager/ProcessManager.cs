using Evelynn_Bot.Account_Process;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;
using AutoIt;
using bAUTH;
using Evelynn_Bot.Constants;
using Evelynn_Bot.Entities;
using Evelynn_Bot.ExternalCommands;
using Evelynn_Bot.GameAI;
using Evelynn_Bot.League_API.GameData;
using Evelynn_Bot.ProcessManager;
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

            if (CheckInGame(itsInterface))
            {
                return GameAi(itsInterface, true);
            }

            await StartAccountProcess(itsInterface);

            while (IsGameStarted(itsInterface) == false)
            {
                Thread.Sleep(15000);
                IsGameStarted(itsInterface);
            }
            return Task.CompletedTask;
        }

        public async Task<Task> StartAccountProcess(Interface itsInterface, bool isFromGame = false)
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

                //itsInterface.license.LeaguePath = itsInterface.jsonRead.Location() + "LeagueClient.exe";

                itsInterface.license.LeaguePath = itsInterface.jsonRead.Location();


                using (AccountProcess accountProcess = new AccountProcess())
                {
                    if (isFromGame == false)
                    {
                        itsInterface.clientKiller.KillLeagueClientNormally(itsInterface);
                        await Task.Delay(3000);
                        accountProcess.StartLeague(itsInterface);
                        await Task.Delay(20000);
                    }
                    if (processExist("RiotClientUx.exe" , itsInterface))
                    {
                        if (isFromGame == false) { await accountProcess.LoginAccount(itsInterface); }
                        accountProcess.Initialize(itsInterface);
                        await itsInterface.lcuPlugins.GetSetMissions();
                        if (!await accountProcess.GetSetWallet(itsInterface))
                        {
                            itsInterface.clientKiller.KillLeagueClient(itsInterface);
                            await Task.Delay(2000);
                            itsInterface.clientKiller.KillLeagueClient(itsInterface);
                            await Task.Delay(10000);
                            return Start(itsInterface);
                        }

                        Dispose(true);

                        if (itsInterface.license.Lol_maxLevel != 0 && itsInterface.summoner.summonerLevel >= itsInterface.license.Lol_maxLevel)
                        {
                            itsInterface.logger.Log(true, itsInterface.messages.AccountDoneXP);
                            itsInterface.clientKiller.KillLeagueClient(itsInterface);
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
                            itsInterface.clientKiller.KillLeagueClient(itsInterface);
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

                        if (CheckInGame(itsInterface))
                        {
                            Console.WriteLine(itsInterface.messages.GameFound);
                            return GameAi(itsInterface, true);
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

        public async Task<Task> GameAi(Interface itsInterface, bool isFromDetect = false)
        {
            itsInterface.dashboardHelper.UpdateLolStatus("In Game", itsInterface);
            await Task.Delay(15000);

            ExtremeGameTimer.Elapsed += (sender, e) => VoidExtremeGameTime(sender, e, itsInterface);
            ExtremeGameTimer.Interval = 3600000;
            ExtremeGameTimer.Enabled = true;
            ExtremeGameTimer.Start();

            itsInterface.logger.Log(true, itsInterface.messages.GameStarted);


            //Burası çok güzel refactor edilecek...

            while (processExist("League of Legends.exe", itsInterface))
            {
                using (GameAi gameAi = new GameAi())
                {
                    while (gameAi.ImageSearchForGameStart(itsInterface.ImgPaths.game_started, "2", itsInterface.messages.GameStarted, itsInterface))
                    {
                        await gameAi.GoMid();

                        if (gameAi.ImageSearch(itsInterface.ImgPaths.shop, "1", "", itsInterface))
                        {
                            AutoItX.Send("p");
                        }

                        gameAi.CurrentPlayerStats(itsInterface);
                        if (itsInterface.player.CurrentHealth < 250)
                        {
                            await gameAi.GoBase();
                        }

                        if (itsInterface.player.CurrentHealth < 300)
                        {
                            gameAi.Heal();
                        }

                        //Rakip varsa, dost minyon yoksa geri çekil!
                        if (gameAi.ImageSearchOnlyForControl(itsInterface.ImgPaths.enemy_health, "2", "", itsInterface)
                            && !gameAi.ImageSearchOnlyForControl(itsInterface.ImgPaths.minions, "2", "", itsInterface))
                        {
                            await gameAi.GoSafeArea();
                        }


                        gameAi.CurrentPlayerStats(itsInterface);
                        while (gameAi.ImageSearch(itsInterface.ImgPaths.minions, "2", itsInterface.messages.SuccessMinion, itsInterface) && 
                               itsInterface.player.CurrentHealth > 250)
                        {

                            gameAi.CurrentPlayerStats(itsInterface);
                            if (gameAi.ImageSearch(itsInterface.ImgPaths.minions, "2", itsInterface.messages.SuccessMinion, itsInterface) &&
                                itsInterface.player.CurrentHealth > 250)
                            {
                                gameAi.HitMove(gameAi.X, gameAi.Y);
                            }

                            gameAi.CurrentPlayerStats(itsInterface);
                            if (itsInterface.player.CurrentHealth < 250)
                            {
                                await gameAi.GoBase();
                            }

                            gameAi.CurrentPlayerStats(itsInterface);
                            if (itsInterface.player.CurrentHealth < 300)
                            {
                                gameAi.Heal();
                            }

                            gameAi.CurrentPlayerStats(itsInterface);
                            if (gameAi.ImageSearch(itsInterface.ImgPaths.minions, "2", itsInterface.messages.SuccessMinion, itsInterface) &&
                                itsInterface.player.CurrentHealth > 250)
                            {
                                gameAi.HitMove(gameAi.X, gameAi.Y);
                            }

                            gameAi.CurrentPlayerStats(itsInterface);
                            if (gameAi.ImageSearch(itsInterface.ImgPaths.minions, "2", itsInterface.messages.SuccessMinion, itsInterface) &&
                                itsInterface.player.CurrentHealth > 250)
                            {
                                gameAi.HitMove(gameAi.X, gameAi.Y);
                            }

                            //Rakibi Sert Dürtme
                            if (gameAi.ImageSearch(itsInterface.ImgPaths.enemy_health, "2", "", itsInterface) && itsInterface.player.CurrentHealth > 350)
                            {
                                gameAi.CurrentPlayerStats(itsInterface);
                                while (gameAi.ImageSearch(itsInterface.ImgPaths.enemy_health, "2", "", itsInterface) && itsInterface.player.CurrentHealth > 350)
                                {
                                    if (gameAi.TowerCheck(itsInterface))
                                    {
                                        await gameAi.GoMid();
                                    }
                                    gameAi.CurrentPlayerStats(itsInterface);
                                    AutoItX.MouseClick("RIGHT", gameAi.X + 65, gameAi.Y + 75, 1, 0);
                                    AutoItX.MouseClick("RIGHT", gameAi.X + 65, gameAi.Y + 75, 1, 0);
                                    AutoItX.Send("d");
                                    AutoItX.Send("f");
                                    await Task.Delay(500);
                                    AutoItX.Send("q");
                                    await Task.Delay(500);
                                    AutoItX.Send("w");
                                    AutoItX.Send("a");
                                    await Task.Delay(500);
                                    AutoItX.Send("e");
                                    await Task.Delay(500);
                                    AutoItX.Send("r");
                                    await Task.Delay(500);
                                    AutoItX.Send("a");
                                    gameAi.CurrentPlayerStats(itsInterface);
                                    if (gameAi.TowerCheck(itsInterface))
                                    {
                                        AutoItX.Send("a");
                                        await Task.Delay(500);
                                        await gameAi.GoMid();
                                    }
                                    if (itsInterface.player.CurrentHealth < 350)
                                    {
                                        AutoItX.Send("a");
                                        await Task.Delay(500);
                                        await gameAi.GoMid();
                                    }
                                }
                                AutoItX.Send("a");
                                await Task.Delay(500);
                                await gameAi.GoMid();
                            }

                            gameAi.CurrentPlayerStats(itsInterface);
                            //Basit şampiyona 2.3 saniye düz vuruş.
                            if (gameAi.ImageSearch(itsInterface.ImgPaths.enemy_health, "2", "", itsInterface) 
                                && itsInterface.player.CurrentHealth > 350)
                            {
                                //Kule varsa, rakip varsa geri çekil
                                if (gameAi.TowerCheck(itsInterface)
                                    && gameAi.ImageSearchOnlyForControl(itsInterface.ImgPaths.enemy_health, "2", "", itsInterface))
                                {
                                    await gameAi.GoMid();
                                }
                                AutoItX.MouseClick("RIGHT", gameAi.X + 65, gameAi.Y + 75, 1, 0);
                                AutoItX.MouseClick("RIGHT", gameAi.X + 65, gameAi.Y + 75, 1, 0);
                                await Task.Delay(2000);
                                await gameAi.GoMid();
                            }

                            //Kule varsa, dost minyon yoksa geri çekil
                            if (gameAi.TowerCheck(itsInterface)
                                && !gameAi.ImageSearchOnlyForControl(itsInterface.ImgPaths.minions, "2", "", itsInterface))
                            {
                                await gameAi.GoMid();
                            }

                            //Kule varsa, rakip varsa geri çekil
                            if (gameAi.TowerCheck(itsInterface)
                                && gameAi.ImageSearchOnlyForControl(itsInterface.ImgPaths.enemy_health, "2", "", itsInterface))
                            {
                                await gameAi.GoMid();
                            }

                            //Rakip varsa, dost minyon yoksa geri çekil!
                            if (gameAi.ImageSearchOnlyForControl(itsInterface.ImgPaths.enemy_health, "2", "", itsInterface)
                                && !gameAi.ImageSearchOnlyForControl(itsInterface.ImgPaths.minions, "2", "", itsInterface))
                            {
                                AutoItX.Send("a");
                                await gameAi.GoSafeArea();
                            }

                            //Rakip varsa, rakip minyon varsa, dost minyon yoksa güvenli alana çekil.
                            if (gameAi.ImageSearchOnlyForControl(itsInterface.ImgPaths.enemy_health, "2", "", itsInterface)
                                && !gameAi.ImageSearchOnlyForControl(itsInterface.ImgPaths.minions, "2", "", itsInterface)
                                && gameAi.ImageSearchOnlyForControl(itsInterface.ImgPaths.enemy_minions, "2", "", itsInterface))
                            {
                                AutoItX.Send("a");
                                await gameAi.GoSafeArea();
                            }

                            gameAi.CurrentPlayerStats(itsInterface);
                            //Düşman minyona oto atak.
                            if (gameAi.ImageSearch(itsInterface.ImgPaths.enemy_minions, "2", itsInterface.messages.SuccessEnemyMinion, itsInterface) 
                                && itsInterface.player.CurrentHealth > 250)
                            {
                                gameAi.HitMove(gameAi.X, gameAi.Y);
                                AutoItX.MouseClick("RIGHT", gameAi.X + 27, gameAi.Y + 20, 1, 0);
                                await Task.Delay(500);
                                AutoItX.Send("q");

                                //Kule varsa, rakip varsa geri çekil.
                                if (gameAi.TowerCheck(itsInterface)
                                    && gameAi.ImageSearchOnlyForControl(itsInterface.ImgPaths.enemy_health, "2", itsInterface.messages.SuccessEnemyMinion, itsInterface))
                                {
                                    await gameAi.GoMid();
                                }

                                //Kule yoksa, rakip varsa atak.
                                if (gameAi.ImageSearch(itsInterface.ImgPaths.enemy_health, "2", itsInterface.messages.SuccessEnemyMinion, itsInterface) 
                                    && !gameAi.TowerCheck(itsInterface))
                                {
                                    AutoItX.Send("a");
                                    await gameAi.Combo(gameAi.X, gameAi.Y);
                                }
                            }

                            gameAi.CurrentPlayerStats(itsInterface);
                            //Kule yoksa, can yüksekse, dost minyon varsa -> All In
                            if (gameAi.ImageSearch(itsInterface.ImgPaths.enemy_health, "3", itsInterface.messages.SuccessEnemyChampion, itsInterface) 
                                && itsInterface.player.CurrentHealth > 350 
                                && !gameAi.TowerCheck(itsInterface)
                                && gameAi.ImageSearchOnlyForControl(itsInterface.ImgPaths.minions, "3", "", itsInterface))
                            {
                                //Kule varsa, rakip varsa geri çekil
                                if (gameAi.TowerCheck(itsInterface)
                                    && gameAi.ImageSearchOnlyForControl(itsInterface.ImgPaths.enemy_health, "2", "", itsInterface))
                                {
                                    await gameAi.GoMid();
                                }

                                AutoItX.MouseClick("RIGHT", gameAi.X + 65, gameAi.Y + 75, 1, 0);

                                gameAi.HitMove(gameAi.X, gameAi.Y);

                                if (gameAi.ImageSearch(itsInterface.ImgPaths.enemy_health,"3", "", itsInterface) 
                                    && !gameAi.TowerCheck(itsInterface))
                                {
                                    AutoItX.MouseClick("LEFT", gameAi.X + 65, gameAi.Y + 75, 1, 0);
                                    AutoItX.Send("q");
                                    AutoItX.Send("q");
                                    AutoItX.Send("a");

                                    gameAi.CurrentPlayerStats(itsInterface);
                                    if (itsInterface.player.CurrentHealth < 300)
                                    {
                                        await gameAi.GoMid();
                                    }

                                    if (gameAi.TowerCheck(itsInterface))
                                    {
                                        AutoItX.Send("a");
                                        await gameAi.GoMid();
                                    }
                                }
                                
                                await Task.Delay(1000);
                                if (gameAi.ImageSearch(itsInterface.ImgPaths.enemy_health, "3", "", itsInterface)
                                    && !gameAi.TowerCheck(itsInterface))
                                {
                                    AutoItX.MouseClick("LEFT", gameAi.X + 65, gameAi.Y + 75, 1, 0);
                                    AutoItX.Send("w");
                                    AutoItX.Send("w");
                                    AutoItX.Send("a");

                                    gameAi.CurrentPlayerStats(itsInterface);
                                    if (itsInterface.player.CurrentHealth < 300)
                                    {
                                        await gameAi.GoMid();
                                    }
                                    if (gameAi.TowerCheck(itsInterface))
                                    {
                                        AutoItX.Send("a");
                                        await gameAi.GoMid();
                                    }
                                }
                                await Task.Delay(1000);
                                if (gameAi.ImageSearch(itsInterface.ImgPaths.enemy_health, "3", "", itsInterface)
                                    && !gameAi.TowerCheck(itsInterface))
                                {
                                    AutoItX.MouseClick("LEFT", gameAi.X + 65, gameAi.Y + 75, 1, 0);
                                    AutoItX.Send("e");
                                    AutoItX.Send("e");
                                    AutoItX.Send("a");

                                    gameAi.CurrentPlayerStats(itsInterface);
                                    if (itsInterface.player.CurrentHealth < 300)
                                    {
                                        await gameAi.GoMid();
                                    }
                                    if (gameAi.TowerCheck(itsInterface))
                                    {
                                        AutoItX.Send("a");
                                        await gameAi.GoMid();
                                    }
                                }
                                await Task.Delay(1000);
                                if (gameAi.ImageSearch(itsInterface.ImgPaths.enemy_health, "3", "", itsInterface)
                                    && !gameAi.TowerCheck(itsInterface))
                                {
                                    AutoItX.MouseClick("LEFT", gameAi.X + 65, gameAi.Y + 75, 1, 0);
                                    AutoItX.Send("r");
                                    AutoItX.Send("r");
                                    AutoItX.Send("a");

                                    gameAi.CurrentPlayerStats(itsInterface);
                                    if (itsInterface.player.CurrentHealth < 300)
                                    {
                                        await gameAi.GoMid();
                                    }
                                    if (gameAi.TowerCheck(itsInterface))
                                    {
                                        AutoItX.Send("a");
                                        await gameAi.GoMid();
                                    }
                                }

                                gameAi.CurrentPlayerStats(itsInterface);
                                if (itsInterface.player.CurrentHealth < 300)
                                {
                                    gameAi.Heal();
                                }

                                gameAi.CurrentPlayerStats(itsInterface);
                                while (gameAi.ImageSearch(itsInterface.ImgPaths.enemy_health, "3", "", itsInterface) && 
                                       itsInterface.player.CurrentHealth > 300 && 
                                       !gameAi.TowerCheck(itsInterface))
                                {
                                    gameAi.CurrentPlayerStats(itsInterface);
                                    if (!gameAi.TowerCheck(itsInterface) &&
                                        itsInterface.player.CurrentHealth > 250)
                                    {
                                        gameAi.CurrentPlayerStats(itsInterface);
                                        gameAi.HitMove(gameAi.X, gameAi.Y);
                                        AutoItX.Send("d");
                                        AutoItX.Send("f");
                                        await Task.Delay(500);
                                        AutoItX.Send("q");
                                        await Task.Delay(500);
                                        AutoItX.Send("w");
                                        AutoItX.Send("a");
                                        await Task.Delay(500);
                                        AutoItX.Send("e");
                                        await Task.Delay(500);
                                        AutoItX.Send("r");
                                        await Task.Delay(500);
                                        AutoItX.Send("a");
                                        await gameAi.Combo(gameAi.X, gameAi.Y);
                                    }
                                }
                            }

                            //Kule varsa, rakip varsa geri çekil
                            if (gameAi.TowerCheck(itsInterface)
                                && gameAi.ImageSearchOnlyForControl(itsInterface.ImgPaths.enemy_health, "2", "", itsInterface))
                            {
                                await gameAi.GoMid();
                            }

                            if (gameAi.ImageSearch(itsInterface.ImgPaths.minions, "2", itsInterface.messages.SuccessMinion, itsInterface) &&
                                itsInterface.player.CurrentHealth > 250)
                            {
                                gameAi.HitMove(gameAi.X, gameAi.Y);
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

                            if (gameAi.ImageSearch(itsInterface.ImgPaths.minions, "2", itsInterface.messages.SuccessMinion, itsInterface) &&
                                itsInterface.player.CurrentHealth > 250)
                            {
                                gameAi.HitMove(gameAi.X, gameAi.Y);
                            }

                            gameAi.CurrentPlayerStats(itsInterface);
                            //Basit şampiyona 2.3 saniye düz vuruş.
                            if (gameAi.ImageSearch(itsInterface.ImgPaths.enemy_health, "2", "", itsInterface) 
                                && itsInterface.player.CurrentHealth > 380)
                            {
                                AutoItX.MouseClick("RIGHT", gameAi.X + 65, gameAi.Y + 75, 1, 0);
                                AutoItX.MouseClick("RIGHT", gameAi.X + 65, gameAi.Y + 75, 1, 0);
                                await Task.Delay(2300);
                                await gameAi.GoMid();
                            }
                        }
                        await gameAi.GoMid();
                    }
                }
            }

            Console.Clear();
            ExtremeGameTime = 0;
            ExtremeGameTimer.Stop();
            Dispose(true);

            if (isFromDetect)
            {
                return StartAccountProcess(itsInterface, true);
            }
            return Task.CompletedTask;
        }

        private void VoidExtremeGameTime(object source, ElapsedEventArgs e, Interface itsInterface)
        {
            ExtremeGameTime++;
            if (ExtremeGameTime >= 2)
            {
                itsInterface.logger.Log(false, "Extreme Game Time! Restarting...");
                ExtremeGameTime = 0;
                var licenseBase64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(itsInterface.license)));
                var exeDir = System.Reflection.Assembly.GetExecutingAssembly().Location;
                Process eBot = new Process();
                eBot.StartInfo.FileName = exeDir;
                eBot.StartInfo.WorkingDirectory = Path.GetDirectoryName(exeDir);
                eBot.StartInfo.Arguments = licenseBase64String;
                eBot.StartInfo.Verb = "runas";
                eBot.Start();
                Environment.Exit(0);
            }
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
                    itsInterface.clientKiller.KillLeagueClient(itsInterface);
                    return Start(itsInterface);
                }

                Console.WriteLine(itsInterface.summoner.summonerLevel);

                accountProcess.PatchCheck(itsInterface);
                itsInterface.lcuPlugins.KillUXAsync();

                Dispose(true);

                if (itsInterface.license.Lol_maxLevel != 0 && itsInterface.summoner.summonerLevel >= itsInterface.license.Lol_maxLevel)
                {
                    itsInterface.logger.Log(true, itsInterface.messages.AccountDoneXP);
                    itsInterface.clientKiller.KillLeagueClient(itsInterface);
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
                    itsInterface.clientKiller.KillLeagueClient(itsInterface);
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
                    await itsInterface.lcuPlugins.DisenchantSummonerCapsules();
                }

                NewQueue.CreateLobby();

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
