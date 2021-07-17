using Evelynn_Bot.Constants;
using Evelynn_Bot.Entities;
using Evelynn_Bot.League_API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Evelynn_Bot.ExternalCommands;
using Leaf.xNet;
using AutoIt;
using bAUTH;
using Evelynn_Bot.GameAI;
using EvelynnLCU.API_Models;
using EvelynnLCU.Plugins.LoL;
using LCU.NET;
using LCU.NET.API_Models;

namespace Evelynn_Bot.Account_Process
{
    public class AccountProcess : IAccountProcess
    {


        private int caps = 0;

        public int SW_HIDE = 0;

        public Encoding HttpRequestEncoding = Encoding.UTF8;



        [DllImport("User32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("User32.dll")]
        public static extern bool EnableWindow(IntPtr hwnd, bool enabled);

        public bool StartLeague(Interface itsInterface)
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = itsInterface.license.LeaguePath;
                info.UseShellExecute = true;
                info.CreateNoWindow = true;
                info.WindowStyle = ProcessWindowStyle.Hidden;

                Process lol = Process.Start(info);
                lol.PriorityClass = ProcessPriorityClass.AboveNormal;
                lol.WaitForInputIdle();
                return itsInterface.Result(true, itsInterface.messages.SuccessStartLeague);


            }
            catch (Exception ex6)
            {
                return itsInterface.Result(false, itsInterface.messages.ErrorStartLeague);
            }
        }
        public async Task<bool> LoginAccount(Interface itsInterface)
        {
            try
            {
                if (itsInterface.license.Lol_username == "")
                {
                    return itsInterface.Result(false, itsInterface.messages.ErrorNullUsername);
                }

                if (itsInterface.license.Lol_password == "")
                {
                    return itsInterface.Result(false, itsInterface.messages.ErrorNullPassword);
                }

                Thread.Sleep(20000);

                itsInterface.lcuApi.InitRiotClient();
                itsInterface.lcuPlugins = new Plugins(itsInterface.lcuApi);
                await itsInterface.lcuPlugins.Login(itsInterface.license.Lol_username, itsInterface.license.Lol_password);
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

                Thread.Sleep(10000);

                Process[] processesByName = Process.GetProcessesByName("RiotClientUx");
                if (processesByName.Length >= 1 && processesByName[0].MainWindowHandle != IntPtr.Zero)
                {
                    AutoItX.ControlClick("Riot Client", "Chrome Legacy Window", "[CLASS:Chrome_RenderWidgetHostHWND; INSTANCE:1]", "left", 1, 647, 355);
                    Thread.Sleep(25000);
                }

                Thread.Sleep(15000);
                //KillUxRender(itsInterface);
                Thread.Sleep(15000);
                Dispose(true);
                itsInterface.lcuPlugins = null;
                return itsInterface.Result(true, itsInterface.messages.SuccessLogin);
            }
            catch (Exception e)
            {
                itsInterface.clientKiller.KillLeagueClient();
                Dispose(true);
                return itsInterface.Result(false, itsInterface.messages.ErrorLogin);
            }
        }
        public bool Initialize(Interface itsInterface)
        {
            try
            {
                itsInterface.lcuApi.Init(InitializeMethod.Lockfile);
                itsInterface.lcuApi.Socket.DumpToDebug = true;
                itsInterface.lcuPlugins = new Plugins(itsInterface.lcuApi);
            }
            catch (Exception e)
            {
                return itsInterface.Result(false, itsInterface.messages.ErrorInitialize);
            }
            return itsInterface.Result(true, itsInterface.messages.SuccessInitialize);
        }
        public async Task<bool> GetSetWallet(Interface itsInterface)
        {
            try
            {
                itsInterface.summoner = await itsInterface.lcuPlugins.GetCurrentSummoner();
                itsInterface.wallet = await itsInterface.lcuPlugins.GetWalletDetails();
                itsInterface.dashboardHelper.UpdateLolWallet(itsInterface.summoner.summonerLevel.ToString(), itsInterface.wallet.ip.ToString(), itsInterface);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"GET SET WALLET HATA | SRC: {e.Source} | SATIR {e.StackTrace}");
                return false;
            }
        }
        public async Task<Task> CheckNewAccount(Interface itsInterface)
        {
            if (string.IsNullOrEmpty(itsInterface.summoner.displayName))
            {
                itsInterface.logger.Log(true, "New account!");
                Thread.Sleep(5000);
                AutoItX.ControlClick("Riot Client", "Chrome Legacy Window", "[CLASS:Chrome_RenderWidgetHostHWND; INSTANCE:1]", "left", 1, 647, 355);
                Thread.Sleep(25000);
                AutoItX.ControlClick("League of Legends", "Chrome Legacy Window", "[CLASS:Chrome_RenderWidgetHostHWND; INSTANCE:1]", "left", 1, 865, 219);
                Thread.Sleep(1250);
                AutoItX.ControlClick("League of Legends", "Chrome Legacy Window", "[CLASS:Chrome_RenderWidgetHostHWND; INSTANCE:1]", "left", 1, 862, 316);
                Thread.Sleep(1250);
                AutoItX.ControlClick("League of Legends", "Chrome Legacy Window", "[CLASS:Chrome_RenderWidgetHostHWND; INSTANCE:1]", "left", 1, 819, 430);
                Thread.Sleep(1250);
                AutoItX.ControlClick("League of Legends", "Chrome Legacy Window", "[CLASS:Chrome_RenderWidgetHostHWND; INSTANCE:1]", "left", 1, 834, 551);
                Thread.Sleep(1250);
                AutoItX.ControlClick("League of Legends", "Chrome Legacy Window", "[CLASS:Chrome_RenderWidgetHostHWND; INSTANCE:1]", "left", 1, 639, 664);

                AutoItX.ControlClick("League of Legends", "Chrome Legacy Window", "[CLASS:Chrome_RenderWidgetHostHWND; INSTANCE:1]", "left", 1, 640, 400); //This is for config bug.

                itsInterface.newLeaguePlayer.name = RandomNameGenerator();

                itsInterface.lcuPlugins.SetSummonerName(itsInterface.newLeaguePlayer.name);
                itsInterface.logger.Log(true, "Successfully used name!");
                itsInterface.clientKiller.KillLeagueClient();
                Thread.Sleep(7000);
                Dispose(true);
                return itsInterface.processManager.Start(itsInterface);

            }

            return Task.CompletedTask;
        }
        public string RandomName(int len, bool two)
        {

                Random r = new Random();

                string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "t", "v", "w", "x"};
                string[] vowels = { "a", "e", "i", "o", "u", "y"};
                string FirstName = "";
                string SecondName = "";
                string FinalName = "";

                FirstName += consonants[r.Next(consonants.Length)].ToUpper();
                FirstName += vowels[r.Next(vowels.Length)];
                SecondName += consonants[r.Next(consonants.Length)].ToUpper();
                SecondName += vowels[r.Next(vowels.Length)];

                int b = 3;
                while (b < len)
                {
                    FirstName += consonants[r.Next(consonants.Length)];
                    SecondName += consonants[r.Next(consonants.Length)];
                    b++;
                    FirstName += vowels[r.Next(vowels.Length)];
                    SecondName += vowels[r.Next(vowels.Length)];
                    b++;
                }

                if (two)
                {
                    FinalName += FirstName + " " + SecondName;
                }
                else
                {
                    FinalName = FirstName;
                }

                return FinalName;
        }
        public string RandomNameGenerator()
        {
            try
            {
                Thread.Sleep(2000);

                Random random = new Random();
                bool two;

                if (random.Next(100) < 40)
                {
                    two = true;
                }
                else
                {
                    two = false;
                }

                return RandomName(6, two);
            }
            catch
            {
                return RandomName(6, false);
            }
        }
        public bool TutorialMissions(Interface itsInterface)
        {
            itsInterface.dashboardHelper.UpdateLolStatus("Playing Tutorial", itsInterface);
            itsInterface.summoner = itsInterface.lcuPlugins.GetCurrentSummoner().Result;
            itsInterface.logger.Log(true, itsInterface.summoner.summonerLevel.ToString());
            if (itsInterface.summoner.summonerLevel < 11)
            {

                Tutorial[] objectArray = itsInterface.lcuPlugins.GetTutorials().Result;
                try
                {
                    if (!objectArray[0].isViewed)
                    {
                        itsInterface.logger.Log(true, "TUTORIAL 1");
                        itsInterface.lcuPlugins.CreateLobbyAsync(new LolLobbyLobbyChangeGameDto { queueId = 2000 });
                        Thread.Sleep(10000);
                        itsInterface.lcuPlugins.PostMatchmakingSearch();

                        itsInterface.gameAi.TutorialAI_1(itsInterface);

                        itsInterface.logger.Log(true, "TUTORIAL 1 ENDED");
                        Thread.Sleep(15000);
                        itsInterface.lcuPlugins.GetSetMissions();
                        itsInterface.lcuPlugins.KillUXAsync();
                        Thread.Sleep(5000);
                    }
                }
                catch
                {
                    // ignored
                }

                Thread.Sleep(5000);
                try
                {
                    if (!objectArray[1].isViewed)
                    {
                        itsInterface.logger.Log(true, "TUTORIAL 2");
                        itsInterface.lcuPlugins.CreateLobbyAsync(new LolLobbyLobbyChangeGameDto { queueId = 2010 });
                        Thread.Sleep(10000);
                        itsInterface.lcuPlugins.PostMatchmakingSearch();

                        itsInterface.gameAi.TutorialAI_2(itsInterface);

                        itsInterface.logger.Log(true, "TUTORIAL 2 ENDED");
                        Thread.Sleep(15000);
                        itsInterface.lcuPlugins.GetSetMissions();
                        itsInterface.lcuPlugins.KillUXAsync();
                        Thread.Sleep(5000);
                    }
                }
                catch
                {
                    // ignored
                }

                Thread.Sleep(5000);
                try
                {
                    if (!objectArray[2].isViewed)
                    {
                        itsInterface.logger.Log(true, "TUTORIAL 3"); itsInterface.lcuPlugins.CreateLobbyAsync(new LolLobbyLobbyChangeGameDto { queueId = 2020 });
                        Thread.Sleep(10000);
                        itsInterface.lcuPlugins.PostMatchmakingSearch();

                        itsInterface.gameAi.TutorialAI_2(itsInterface); //Bilerek "2" olarak bırakılmıştır, aynı AI!

                        itsInterface.logger.Log(true, "TUTORIAL 3 ENDED");
                        Thread.Sleep(15000);
                        itsInterface.lcuPlugins.GetSetMissions();
                        itsInterface.lcuPlugins.KillUXAsync();
                        Thread.Sleep(5000);
                        Dispose(true);
                        return itsInterface.Result(true, "Tutorial games are finished!");
                    }
                }
                catch
                {
                    // ignored
                }
            }
            return itsInterface.Result(false, "");
        }
        public bool PatchCheck(Interface itsInterface)
        {
            try
            {
                int tryNum = 1;
                while (LeagueIsPatchAvailable(itsInterface))
                {
                    itsInterface.logger.Log(true, "Patch bulundu!");
                    Thread.Sleep(60000);
                    Dispose(true);
                    tryNum++;
                    if (tryNum >= 15)
                    {
                        break;
                    }
                }
                return itsInterface.Result(true, "");
            }
            catch (Exception e)
            {
                Console.WriteLine("PATCH CHECK HATASI");
                return itsInterface.Result(false, "");
            }

        }
        public bool LeagueIsPatchAvailable(Interface itsInterface)
        {
            LeaguePatch lolPatchNew = itsInterface.lcuPlugins.GetLeaguePatchAsync().Result;
            //Logger.Status($@"Is there a new league patch: {lolPatchNew.isUpdateAvailable}");
            return itsInterface.Result(lolPatchNew.isCorrupted || lolPatchNew.isUpdateAvailable || !lolPatchNew.isUpToDate, "");
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
        ~AccountProcess()
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
