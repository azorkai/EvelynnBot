using AutoIt;
using Evelynn_Bot.Results;
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
                return data;
            }
            catch
            {
                return null;
            }

        }
        public IResult ImageSearchForGameStart(string path, string tolerance, string message)
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
                    return new Result(true, message);
                }
                return new Result(false, "Oyun başlamadı!");
            }
            catch (Exception e)
            {
                return new Result(false, "ERROR!");
            }
        }
        public IResult ImageSearch(string path, string tolerance, string message)
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
                    return new Result(true, message);
                }
                return new Result(false, "Görsel bulunamadı");
            }
            catch (Exception e)
            {
                return new Result(false, "ERROR!");
            }
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
            AutoItX.MouseClick("RIGHT", x - 20, y + 40, 1, 1);
            AutoItX.Send("a");
            Thread.Sleep(700);
            AutoItX.MouseClick("LEFT", x - 20, y + 40, 1, 1);
            AutoItX.Send("a");
            AutoItX.Send("a");
        }

        public void Combo(int x, int y)
        {
            AutoItX.MouseClick("LEFT", x + 65, y + 75, 1, 0);
            Thread.Sleep(200);
            AutoItX.Send("q");
            AutoItX.Send("w");
            AutoItX.Send("e");
            Thread.Sleep(200);
            AutoItX.Send("r");
            Thread.Sleep(500);
            AutoItX.Send("a");
        }

        public void GoMid()
        {
            AutoItX.MouseClick("RIGHT", game_X + 43, game_Y - 33, 1, 0);
            AutoItX.MouseClick("RIGHT", game_X + 43, game_Y - 33, 1, 0);
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

        public void GoBase()
        {
            Thread.Sleep(1500);
            AutoItX.MouseClick("RIGHT", game_X + 31, game_Y - 19, 1, 0);
            AutoItX.MouseClick("RIGHT", game_X + 31, game_Y - 19, 1, 0);
            Thread.Sleep(13000);
            AutoItX.Send("b");
            Thread.Sleep(19000);
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
            AutoItX.MouseClick("LEFT", game_X - 158, game_Y - 172, 1, 0);
            Thread.Sleep(1500);
            AutoItX.MouseClick("RIGHT", game_X - 158, game_Y - 172, 1, 0);
            AutoItX.MouseClick("LEFT", game_X - 13, game_Y - 48, 1, 0);
            AutoItX.MouseClick("LEFT", game_X - 13, game_Y - 48, 5,1);
            Thread.Sleep(200);
            AutoItX.MouseClick("LEFT", game_X - 13, game_Y - 48, 5, 1);
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


        #region TutorialAI
        public void TutorialAI_1()
        {
            while (IsGameStarted() == false)
            {
                Thread.Sleep(15000);
                IsGameStarted();
            }

            while (ImageSearchForGameStart(ImagePaths.game_started, "2", Messages.GameStarted).Success)
            {
                GoMid();

                if (ImageSearch(ImagePaths.enemy_health, "2", Messages.SuccessEnemyChampion).Success)
                {
                    AutoItX.MouseClick("RIGHT", X + 65, Y + 75, 1, 0);
                    HitMove(X, Y);
                    Combo(X, Y);
                    AutoItX.Send("d");
                    AutoItX.Send("f");
                    Thread.Sleep(1500);
                }

                while (ImageSearch(ImagePaths.minions_tutorial, "3", Messages.SuccessMinion).Success)
                {
                    HitMove(X, Y);
                    Thread.Sleep(500);

                    if (ImageSearch(ImagePaths.enemy_minions, "3", Messages.SuccessEnemyChampion).Success)
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

                    if (ImageSearch(ImagePaths.enemy_health, "2", Messages.SuccessEnemyChampion).Success)
                    {
                        AutoItX.MouseClick("RIGHT", X + 65, Y + 75, 1, 0);
                        HitMove(X, Y);
                        Combo(X, Y);
                        AutoItX.Send("d");
                        AutoItX.Send("f");
                        Thread.Sleep(1500);
                    }

                    if (ImageSearch(ImagePaths.tower, "2", Messages.SuccessEnemyChampion).Success)
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

        public void TutorialAI_2()
        {
            while (IsGameStarted() == false)
            {
                Thread.Sleep(15000);
                IsGameStarted();
            }

            Thread.Sleep(5000);

            SkillUp("q", "j");

            Thread.Sleep(5000);

            BuyItem();

            while (ImageSearchForGameStart(ImagePaths.game_started, "2", Messages.GameStarted).Success)
            {
                GoMid();
                Thread.Sleep(4000);

                if (ImageSearch(ImagePaths.enemy_health, "2", Messages.SuccessEnemyChampion).Success)
                {
                    AutoItX.MouseClick("RIGHT", X + 65, Y + 75, 1, 0);
                    HitMove(X, Y);
                    Combo(X, Y);
                    AutoItX.Send("d");
                    AutoItX.Send("f");
                    Thread.Sleep(1500);
                }

                while (ImageSearch(ImagePaths.minions, "3", Messages.SuccessMinion).Success)
                {
                    SkillUp("q", "j");
                    SkillUp("w", "k");
                    SkillUp("e", "m");
                    SkillUp("r", "l");
                    HitMove(X, Y);
                    Thread.Sleep(500);

                    if (ImageSearch(ImagePaths.enemy_minions, "3", Messages.SuccessEnemyChampion).Success)
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

                    if (ImageSearch(ImagePaths.enemy_health, "2", Messages.SuccessEnemyChampion).Success)
                    {
                        AutoItX.MouseClick("RIGHT", X + 65, Y + 75, 1, 0);
                        HitMove(X, Y);
                        Combo(X, Y);
                        AutoItX.Send("d");
                        AutoItX.Send("f");
                        Thread.Sleep(1500);
                    }

                    if (ImageSearch(ImagePaths.tower, "2", Messages.SuccessEnemyChampion).Success)
                    {
                        AutoItX.MouseClick("RIGHT", X + 65, Y + 75, 1, 0);
                        AutoItX.MouseClick("RIGHT", X + 65, Y + 75, 1, 0);
                        AutoItX.MouseClick("RIGHT", X + 65, Y + 75, 1, 0);
                        Thread.Sleep(3000);
                    }

                }
            }
        }

        public bool IsGameStarted()
        {
            if (ImageSearchForGameStart(ImagePaths.game_started, "2", Messages.GameStarted).Success)
            {
                return true;
            }
            return false;
        }


        #endregion

        public void CurrentPlayerStats(Player player)
        {
            using (ApiCalls apiCalls = new ApiCalls())
            {
                var liveData = apiCalls.GetLiveGameData();
                player.MaxHealth = liveData.activePlayer.championStats.maxHealth;
                player.CurrentHealth = liveData.activePlayer.championStats.currentHealth;
                player.CurrentGold = liveData.activePlayer.currentGold;
                player.Level = liveData.activePlayer.level;
            }
        }

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
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
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
