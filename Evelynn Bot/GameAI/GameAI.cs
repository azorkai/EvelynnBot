using AutoIt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Evelynn_Bot.Account_Process;
using Evelynn_Bot.Constants;
using Evelynn_Bot.Entities;
using Evelynn_Bot.ExternalCommands;
using Evelynn_Bot.League_API;
using Evelynn_Bot.League_API.GameData;

namespace Evelynn_Bot.GameAI
{
    public class GameAi : IGameAI
    {
        [DllImport("ImageSearchDLL.dll")]
        public static extern IntPtr ImageSearch(int x, int y, int right, int bottom, [MarshalAs(UnmanagedType.LPStr)] string imagePath);

        private readonly Random _random = new Random();

        public int game_X;
        public int game_Y;
        public int X;
        public int Y;

        public string[] UseImageSearch(string imgPath, string tolerance)
        {
            try
            {
                imgPath = "*" + tolerance + " " + imgPath;
                var lol = AutoItX.WinGetPos("League of Legends (TM) Client");
                IntPtr result = ImageSearch(lol.X, lol.Y, lol.Right, lol.Bottom, imgPath);
                string res = Marshal.PtrToStringAnsi(result);
                if (res[0] == '0') return null;
                string[] data = res.Split('|');
                int x; int y;
                int.TryParse(data[1], out x);
                int.TryParse(data[2], out y);
                Dispose(true);
                return data;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

        }
        public bool ImageSearchForGameStart(string path, string tolerance, string message, Interface itsInterface)
        {
            int x;
            int y;
            try
            {
                string[] results = UseImageSearch(path, tolerance);
                if (results != null)
                {
                    Int32.TryParse(results[1], out x);
                    Int32.TryParse(results[2], out y);
                    game_X = x;
                    game_Y = y;
                    return itsInterface.Result(true, message);
                }
                return itsInterface.Result(false, "");
            }
            catch (Exception e)
            {
                return itsInterface.Result(false, "ERROR!");
            }
        }
        public bool ImageSearch(string path, string tolerance, string message, Interface itsInterface)
        {
            int x;
            int y;
            try
            {
                string[] results = UseImageSearch(path, tolerance);
                if (results != null)
                {
                    Int32.TryParse(results[1], out x);
                    Int32.TryParse(results[2], out y);
                    X = x;
                    Y = y;
                    return itsInterface.Result(true, message);
                }
                return itsInterface.Result(false, "");
            }
            catch (Exception e)
            {
                return itsInterface.Result(false, "ERROR!");
            }
        }

        public bool ImageSearchOnlyForControl(string path, string tolerance, string message, Interface itsInterface)
        {
            try
            {
                string[] results = UseImageSearch(path, tolerance);
                if (results != null)
                {
                    return itsInterface.Result(true, message);
                }
                return itsInterface.Result(false, "");
            }
            catch (Exception e)
            {
                return itsInterface.Result(false, "ERROR!");
            }
        }

        public bool TowerCheck(Interface itsInterface)
        {
            if (ImageSearchOnlyForControl(itsInterface.ImgPaths.tower, "2", "", itsInterface) || 
                ImageSearchOnlyForControl(itsInterface.ImgPaths.tower2, "2", "", itsInterface))
            {
                return true;
            }
            return false;
        }

        public void SkillUp(string skill, string skill2)
        {
            AutoItX.Send("{CTRLDOWN}");
            AutoItX.Send(skill);
            AutoItX.Send("{CTRLUP}");
            AutoItX.Send(skill2);
        }

        public void HitMove(int x, int y)
        {
            AutoItX.MouseClick("RIGHT", x - 17, y + 35, 1, 1);
            AutoItX.Send("a");
            Thread.Sleep(700);
            AutoItX.MouseClick("LEFT", x - 17, y + 35, 1, 1);
            AutoItX.Send("a");
            AutoItX.Send("a");
            AutoItX.MouseClick("RIGHT", x - 17, y + 35, 1, 1);
            AutoItX.MouseClick("RIGHT", x - 17, y + 35, 1, 1);
            AutoItX.MouseClick("RIGHT", x - 17, y + 35, 1, 1);

        }

        public async Task Combo(int x, int y)
        {
            AutoItX.MouseClick("LEFT", x + 65, y + 75, 1, 0);
            await Task.Delay(200);
            AutoItX.Send("q");
            AutoItX.Send("w");
            AutoItX.Send("e");
            await Task.Delay(200);
            AutoItX.Send("r");
            await Task.Delay(1000);
            AutoItX.Send("a");
        }

        public void Heal()
        {
            AutoItX.Send("f");
        }

        public async Task GoSafeArea()
        {
            AutoItX.MouseClick("RIGHT", game_X + 30, game_Y - 20, 1, 0);
            AutoItX.MouseClick("RIGHT", game_X + 30, game_Y - 20, 1, 0);
            await Task.Delay(1500);
        }

        public async Task GoMid()
        {
            AutoItX.MouseClick("RIGHT", game_X + 42, game_Y - 30, 1, 0);
            AutoItX.MouseClick("RIGHT", game_X + 42, game_Y - 30, 1, 0);
            await Task.Delay(1900);
        }

        public void GoTop()
        {
            AutoItX.MouseClick("RIGHT", game_X + 25, game_Y - 55, 1, 0);
            AutoItX.MouseClick("RIGHT", game_X + 25, game_Y - 55, 1, 0);
        }

        public void GoBot()
        {
            AutoItX.MouseClick("RIGHT", game_X + 73, game_Y, 1, 0);
            AutoItX.MouseClick("RIGHT", game_X + 73, game_Y, 1, 0);
        }

        public async Task GoBase()
        {
            AutoItX.MouseClick("RIGHT", game_X + 31, game_Y - 19, 1, 0);
            AutoItX.MouseClick("RIGHT", game_X + 31, game_Y - 19, 1, 0);
            await Task.Delay(14000);
            AutoItX.Send("b");
            AutoItX.Send("b");
            AutoItX.Send("b");
            await Task.Delay(14000);
            AutoItX.MouseClick("RIGHT", game_X + 31, game_Y - 19, 1, 0);
            AutoItX.MouseClick("RIGHT", game_X + 31, game_Y - 19, 1, 0);
        }

        public void PickTutorialChampionAI()
        {
            AutoItX.MouseClick("RIGHT", game_X - 199, game_Y -218, 1, 0);
            AutoItX.MouseClick("RIGHT", game_X - 199, game_Y - 218, 1, 0);
            AutoItX.MouseClick("RIGHT", game_X - 199, game_Y - 218, 1, 0);
        }

        public void BuyItem()
        {
            AutoItX.Send("p");
            Thread.Sleep(2000);
            AutoItX.MouseClick("LEFT", game_X - 158, game_Y - 172, 1, 0);
            AutoItX.MouseDown();
            Thread.Sleep(500);
            AutoItX.MouseUp();
            Thread.Sleep(700);
            for (int i = 0; i < 10; i++)
            {
                AutoItX.MouseClick("LEFT", game_X - 158, game_Y - 172, 1, 0);
                Thread.Sleep(1500);
                AutoItX.MouseClick("RIGHT", game_X - 158, game_Y - 172, 1, 0);
                Thread.Sleep(500);
                AutoItX.MouseClick("RIGHT", game_X - 158, game_Y - 172, 1, 0);
                Thread.Sleep(200);
                AutoItX.MouseClick("LEFT", game_X - 158, game_Y - 172, 1, 0);
                Thread.Sleep(200);
                AutoItX.MouseClick("RIGHT", game_X - 158, game_Y - 172, 1, 0);
                Thread.Sleep(200);
                AutoItX.MouseClick("RIGHT", game_X - 158, game_Y - 172, 1, 0);
                AutoItX.MouseClick("LEFT", game_X - 13, game_Y - 48, 1, 0);
                AutoItX.MouseClick("LEFT", game_X - 13, game_Y - 48, 5, 1);
                Thread.Sleep(200);
                AutoItX.MouseClick("LEFT", game_X - 13, game_Y - 48, 5, 1);
            }
            AutoItX.Send("p");
        }

        public void RandomLaner()
        {
            int random =_random.Next(1, 4);
            if (random == 1)
            {
                GoTop();
                Console.WriteLine("Going to Top!");
            }

            if (random == 2)
            {
                GoMid();
                Console.WriteLine("Going to Mid");
            }

            if (random == 3)
            {
                GoBot();
                Console.WriteLine("Going to Bot!");
            }
        }

        public bool processExist(string win, Interface itsInterface)
        {
            int a = AutoItX.ProcessExists(win);
            return itsInterface.Result(Convert.ToBoolean(a), "");
        }

        #region TutorialAI
        public void TutorialAI_1(Interface itsInterface)
        {
            while (IsGameStarted(itsInterface) == false)
            {
                Thread.Sleep(15000);
                IsGameStarted(itsInterface);
            }

            while (processExist("League of Legends.exe", itsInterface))
            {
                while (ImageSearchForGameStart(itsInterface.ImgPaths.game_started, "2", itsInterface.messages.GameStarted, itsInterface))
                {
                    GoMid();

                    if (ImageSearch(itsInterface.ImgPaths.shop, "1", "", itsInterface))
                    {
                        AutoItX.Send("p");
                    }

                    if (ImageSearch(itsInterface.ImgPaths.enemy_health, "2", itsInterface.messages.SuccessEnemyChampion, itsInterface))
                    {
                        AutoItX.MouseClick("RIGHT", X + 65, Y + 75, 1, 0);
                        HitMove(X, Y);
                        Combo(X, Y);
                        AutoItX.Send("d");
                        AutoItX.Send("f");
                        Thread.Sleep(1500);
                    }

                    while (ImageSearch(itsInterface.ImgPaths.minions_tutorial, "3", itsInterface.messages.SuccessMinion, itsInterface))
                    {
                        HitMove(X, Y);
                        Thread.Sleep(500);

                        if (ImageSearch(itsInterface.ImgPaths.enemy_minions, "3", itsInterface.messages.SuccessEnemyChampion, itsInterface))
                        {
                            AutoItX.MouseClick("RIGHT", X + 27, Y + 20, 1, 0);
                            AutoItX.Send("q");
                            Thread.Sleep(1500);
                            AutoItX.Send("w");
                            Thread.Sleep(1500);
                            AutoItX.Send("e");
                            Thread.Sleep(1500);
                            AutoItX.Send("r");
                            AutoItX.Send("d");
                            AutoItX.Send("f");
                            Thread.Sleep(1500);
                        }

                        if (ImageSearch(itsInterface.ImgPaths.enemy_health, "2", itsInterface.messages.SuccessEnemyChampion, itsInterface))
                        {
                            AutoItX.MouseClick("RIGHT", X + 65, Y + 75, 1, 0);
                            HitMove(X, Y);
                            Combo(X, Y);
                            AutoItX.Send("d");
                            AutoItX.Send("f");
                            Thread.Sleep(1500);
                        }

                        if (ImageSearch(itsInterface.ImgPaths.tower, "2", itsInterface.messages.SuccessEnemyChampion, itsInterface))
                        {
                            AutoItX.MouseClick("RIGHT", X + 65, Y + 75, 1, 0);
                            AutoItX.MouseClick("RIGHT", X + 65, Y + 75, 1, 0);
                            AutoItX.MouseClick("RIGHT", X + 65, Y + 75, 1, 0);
                            Thread.Sleep(3000);
                        }
                        GoMid();
                    }

                    Thread.Sleep(6000);
                    PickTutorialChampionAI();
                }

            }

        }

        public void TutorialAI_2(Interface itsInterface)
        {
            while (IsGameStarted(itsInterface) == false)
            {
                Thread.Sleep(15000);
                IsGameStarted(itsInterface);
            }

            Thread.Sleep(5000);
            SkillUp("q", "j");
            Thread.Sleep(5000);
            for (int i = 0; i < 2; i++)
            {
                BuyItem();
            }

            while (processExist("League of Legends.exe", itsInterface))
            {
                while (ImageSearchForGameStart(itsInterface.ImgPaths.game_started, "2", itsInterface.messages.GameStarted, itsInterface))
                {
                    SkillUp("q", "j");
                    GoMid();
                    Thread.Sleep(4000);

                    if (ImageSearch(itsInterface.ImgPaths.enemy_health, "2", itsInterface.messages.SuccessEnemyChampion, itsInterface))
                    {
                        AutoItX.MouseClick("RIGHT", X + 65, Y + 75, 1, 0);
                        HitMove(X, Y);
                        Combo(X, Y);
                        AutoItX.Send("d");
                        AutoItX.Send("f");
                        Thread.Sleep(1500);
                    }

                    while (ImageSearch(itsInterface.ImgPaths.minions, "3", itsInterface.messages.SuccessMinion, itsInterface))
                    {
                        SkillUp("q", "j");
                        SkillUp("w", "k");
                        SkillUp("e", "m");
                        SkillUp("r", "l");
                        HitMove(X, Y);
                        Thread.Sleep(500);

                        if (ImageSearch(itsInterface.ImgPaths.enemy_minions, "3", itsInterface.messages.SuccessEnemyChampion, itsInterface))
                        {
                            AutoItX.MouseClick("RIGHT", X + 27, Y + 20, 1, 0);
                            AutoItX.Send("q");
                            Thread.Sleep(1500);
                            AutoItX.Send("w");
                            Thread.Sleep(1500);
                            AutoItX.Send("e");
                            Thread.Sleep(1500);
                            AutoItX.Send("r");
                            AutoItX.Send("d");
                            AutoItX.Send("f");
                            Thread.Sleep(1500);
                        }

                        if (ImageSearch(itsInterface.ImgPaths.enemy_health, "2", itsInterface.messages.SuccessEnemyChampion, itsInterface))
                        {
                            AutoItX.MouseClick("RIGHT", X + 65, Y + 75, 1, 0);
                            HitMove(X, Y);
                            Combo(X, Y);
                            AutoItX.Send("d");
                            AutoItX.Send("f");
                            Thread.Sleep(1500);
                        }

                        if (ImageSearch(itsInterface.ImgPaths.tower, "2", itsInterface.messages.SuccessEnemyChampion, itsInterface))
                        {
                            AutoItX.MouseClick("RIGHT", X + 65, Y + 75, 1, 0);
                            AutoItX.MouseClick("RIGHT", X + 65, Y + 75, 1, 0);
                            AutoItX.MouseClick("RIGHT", X + 65, Y + 75, 1, 0);
                            Thread.Sleep(3000);
                        }
                    }
                }
            }
            Thread.Sleep(10000);
        }

        public bool IsGameStarted(Interface itsInterface)
        {
            if (ImageSearchForGameStart(itsInterface.ImgPaths.game_started, "2", itsInterface.messages.GameStarted, itsInterface))
            {
                return true;
            }
            return false;
        }


        #endregion

        public void CurrentPlayerStats(Interface itsInterface)
        {
            var liveData = itsInterface.lcuPlugins.GetLiveGameData();
            itsInterface.player.MaxHealth = liveData.Result.activePlayer.championStats.maxHealth;
            itsInterface.player.CurrentHealth = liveData.Result.activePlayer.championStats.currentHealth;
            itsInterface.player.CurrentGold = liveData.Result.activePlayer.currentGold;
            itsInterface.player.Level = liveData.Result.activePlayer.level;
        }

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.Collect();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~GameAi()
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
