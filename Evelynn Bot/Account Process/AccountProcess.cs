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
using Evelynn_Bot.League_API.GameData;
using Leaf.xNet;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using AutoIt;
using bAUTH;
using Evelynn_Bot.GameAI;

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
                lol.PriorityClass = ProcessPriorityClass.BelowNormal;
                lol.WaitForInputIdle();
                return itsInterface.Result(true, itsInterface.messages.SuccessStartLeague);


            }
            catch (Exception ex6)
            {
                return itsInterface.Result(false, itsInterface.messages.ErrorStartLeague);
            }
        }
        public bool LoginAccount(Interface itsInterface)
        {
            try
            {
                using (ApiCalls apiCalls = new ApiCalls())
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

                    if (File.Exists(Application.StartupPath + "\\lockfile"))
                    {
                        File.Delete(Application.StartupPath + "\\lockfile");
                    }

                    string sourceFileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Riot Games\\Riot Client\\Config\\lockfile";
                    File.Copy(sourceFileName, Application.StartupPath + "\\lockfile");
                    string text = File.ReadAllText(Application.StartupPath + "\\lockfile");
                    string[] separator = new string[]
                    {":"};
                    string[] array = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    string string_ = array[2];
                    string string_2 = array[3];

                    try
                    {
                        bool sertifica_metot(object object_0, X509Certificate x509Certificate_0, X509Chain x509Chain_0, SslPolicyErrors sslPolicyErrors_0)
                        {
                            return true;
                        }
                        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(sertifica_metot);
                    }
                    catch (Exception ex)
                    {

                    }

                    string string_3 = $"{{ \"username\": \"{itsInterface.license.Lol_username}\", \"password\": \"{itsInterface.license.Lol_password}\", \"persistLogin\": false }}";

                    apiCalls.requestWeb("/rso-auth/v1/session/credentials", string_3, "PUT", string_2, string_);

                    Thread.Sleep(3500);
                    try
                    {
                        var eula = apiCalls.requestWeb("/eula/v1/agreement/acceptance", "", "GET", string_2, string_)
                            .Equals("\"AcceptanceRequired\"");
                        if (eula)
                        {
                            apiCalls.requestWeb("/eula/v1/agreement/acceptance", "", "PUT", string_2, string_);
                            Thread.Sleep(1000);
                            apiCalls.requestWeb("/v1/products/league_of_legends/patchlines/live", "", "POST", string_2, string_);
                        }
                    }
                    catch
                    {
                    }

                    //Console.WriteLine(requestWeb("/product-session/v1/sessions", "", "GET", string_2, string_));
                    //PATCH DETECT
                    //Console.WriteLine();

                    Thread.Sleep(10000);

                    Process[] processesByName = Process.GetProcessesByName("RiotClientUx");
                    if (processesByName.Length >= 1 && processesByName[0].MainWindowHandle != IntPtr.Zero)
                    {
                        AutoItX.ControlClick("Riot Client", "Chrome Legacy Window", "[CLASS:Chrome_RenderWidgetHostHWND; INSTANCE:1]", "left", 1, 647, 355);
                        Thread.Sleep(25000);
                    }

                    Thread.Sleep(15000);
                    KillUxRender(itsInterface);
                    Thread.Sleep(15000);
                    Dispose(true);
                    return itsInterface.Result(true, itsInterface.messages.SuccessLogin);
                }

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
                using (var fileStream = new FileStream(itsInterface.@jsonRead.Location() + "lockfile", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var streamReader = new StreamReader(fileStream, Encoding.Default))
                    {
                        string line;
                        while ((line = streamReader.ReadLine()) != null)
                        {
                            string[] lines = line.Split(':');
                            itsInterface.apiVariables.IPort = int.Parse(lines[2]);
                            string riot_pass = lines[3];
                            itsInterface.apiVariables.IAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes("riot:" + riot_pass));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return itsInterface.Result(false, itsInterface.messages.ErrorInitialize);
            }
            return itsInterface.Result(true, itsInterface.messages.SuccessInitialize);
        }
        public bool GetSetWallet(Interface itsInterface)
        {
            try
            {
                using (ApiCalls apiCalls = new ApiCalls())
                {
                    itsInterface.summoner = apiCalls.GetObject<Summoner>("/lol-summoner/v1/current-summoner", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                    itsInterface.wallet = apiCalls.GetObject<Wallet>("/lol-store/v1/wallet", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                    itsInterface.dashboardHelper.UpdateLolWallet(itsInterface.summoner.summonerLevel.ToString(), itsInterface.wallet.ip.ToString(), itsInterface);
                    return itsInterface.Result(true, "");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"GET SET WALLET HATA | SRC: {e.Source} | SATIR {e.StackTrace}");
                return itsInterface.Result(false, "");
            }
        }

        public bool KillUxRender(Interface itsInterface)
        {
            using (ApiCalls apiCalls = new ApiCalls())
            {
                apiCalls.PostObject<string>("", "/riotclient/kill-ux", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                return itsInterface.Result(true, "");
            }
        }

        public bool ShowUxRender(Interface itsInterface)
        {
            using (ApiCalls apiCalls = new ApiCalls())
            {
                apiCalls.PostObject<string>("", "/riotclient/ux-show", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                return itsInterface.Result(true, "");
            }
        }
        public bool CheckNewAccount(Interface itsInterface)
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

                using (ApiCalls apiCalls = new ApiCalls())
                {
                    if (apiCalls.PostObject<NewLeaguePlayer>(itsInterface.newLeaguePlayer, "/lol-summoner/v1/summoners", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort))
                    {
                        itsInterface.logger.Log(true, "Successfully used name!");
                        itsInterface.clientKiller.KillLeagueClient();
                        Thread.Sleep(7000);
                        Dispose(true);
                        return itsInterface.processManager.Start(itsInterface);
                    }

                    itsInterface.logger.Log(true, "Need another name..");
                    itsInterface.newLeaguePlayer.name = RandomNameGenerator();
                    apiCalls.PostObject<NewLeaguePlayer>(itsInterface.newLeaguePlayer, "/lol-summoner/v1/summoners", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                    itsInterface.logger.Log(true, "Successfully used name!");
                    itsInterface.clientKiller.KillLeagueClient();
                    Thread.Sleep(7000);
                    Dispose(true);
                    return itsInterface.processManager.Start(itsInterface);
                }
            }
            return itsInterface.Result(false, "");
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
            using (ApiCalls apiCalls = new ApiCalls())
            {
                itsInterface.dashboardHelper.UpdateLolStatus("Playing Tutorial", itsInterface);
                itsInterface.summoner = apiCalls.GetObject<Summoner>("/lol-summoner/v1/current-summoner", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                itsInterface.logger.Log(true, itsInterface.summoner.summonerLevel.ToString());
                if (itsInterface.summoner.summonerLevel < 11)
                {

                    Tutorial[] objectArray = apiCalls.GetObjectArray<Tutorial>("/lol-npe-tutorial-path/v1/tutorials", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                    try
                    {
                        if (!objectArray[0].isViewed)
                        {
                            itsInterface.logger.Log(true,"TUTORIAL 1");
                            apiCalls.PostObject<Lobby>(new Lobby
                            {
                                gameMode = "TUTORIAL",
                                queueId = 2000
                            }, "/lol-lobby/v2/lobby", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                            Thread.Sleep(10000);
                            apiCalls.PostObject<string>("", "/lol-lobby/v2/lobby/matchmaking/search", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);


                            itsInterface.gameAi.TutorialAI_1(itsInterface);

                            itsInterface.logger.Log(true,"TUTORIAL 1 ENDED");
                            Thread.Sleep(15000);
                            SelectChampion(itsInterface);
                            KillUxRender(itsInterface);
                            Thread.Sleep(5000);
                        }
                        else
                        {

                        }
                    }
                    catch
                    {
                    }
                    Thread.Sleep(5000);
                    try
                    {
                        if (!objectArray[1].isViewed)
                        {
                            itsInterface.logger.Log(true,"TUTORIAL 2");
                            apiCalls.PostObject<Lobby>(new Lobby
                            {
                                gameMode = "TUTORIAL",
                                queueId = 2010
                            }, "/lol-lobby/v2/lobby", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                            Thread.Sleep(10000);
                            apiCalls.PostObject<string>("", "/lol-lobby/v2/lobby/matchmaking/search", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);


                            itsInterface.gameAi.TutorialAI_2(itsInterface);

                            itsInterface.logger.Log(true,"TUTORIAL 2 ENDED");
                            Thread.Sleep(15000);
                            SelectChampion(itsInterface);
                            KillUxRender(itsInterface);
                            Thread.Sleep(5000);
                        }
                    }
                    catch
                    {
                    }
                    Thread.Sleep(5000);
                    try
                    {
                        if (!objectArray[2].isViewed)
                        {
                            itsInterface.logger.Log(true,"TUTORIAL 3");
                            apiCalls.PostObject<Lobby>(new Lobby
                            {
                                gameMode = "TUTORIAL",
                                queueId = 2020
                            }, "/lol-lobby/v2/lobby", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                            Thread.Sleep(10000);
                            apiCalls.PostObject<string>("", "/lol-lobby/v2/lobby/matchmaking/search", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);


                            itsInterface.gameAi.TutorialAI_2(itsInterface); //Bilerek "2" olarak bırakılmıştır, aynı AI!
                            

                            itsInterface.logger.Log(true,"TUTORIAL 3 ENDED");
                            Thread.Sleep(15000);
                            SelectChampion(itsInterface);
                            KillUxRender(itsInterface);
                            Thread.Sleep(5000);
                            Dispose(true);
                            return itsInterface.Result(true, "Tutorial games are finished!");
                        }
                    }
                    catch
                    {
                    }
                }
                return itsInterface.Result(false, "");
            }
            
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
            using (ApiCalls apiCalls = new ApiCalls())
            {
                LeaguePatch lolPatchNew = apiCalls.GetObject<LeaguePatch>("/lol-patch/v1/products/league_of_legends/state", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                //Logger.Status($@"Is there a new league patch: {lolPatchNew.isUpdateAvailable}");
                return itsInterface.Result(lolPatchNew.isCorrupted || lolPatchNew.isUpdateAvailable || !lolPatchNew.isUpToDate, "");

            }
        }
        public bool Disenchant(Interface itsInterface)
        {
            try
            {
                using (ApiCalls apiCalls = new ApiCalls())
                {
                    if (caps < 50)
                    {
                        caps++;
                        string playerLoot = apiCalls.GetObject("/lol-loot/v1/player-loot-map", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                        bool flag = false;
                        string[] array = playerLoot.Split(new char[]
                        {
                        '"'
                        });
                        for (int i = 0; i < array.Length; i++)
                        {
                            if (array[i].Contains("lootId") && i + 3 < array.Length)
                            {
                                string text = array[i + 2];
                                if (text.StartsWith("CHAMPION_RENTAL_"))
                                {
                                    apiCalls.PostObject<string[]>(new string[]
                                    {
                                    text
                                    }, "/lol-loot/v1/recipes/CHAMPION_RENTAL_disenchant/craft", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                    Thread.Sleep(500);
                                }
                                else if (text.StartsWith("CHEST_") && !text.StartsWith("CHEST_generic"))
                                {
                                    flag = true;
                                    apiCalls.PostObject<string[]>(new string[]
                                    {
                                    text
                                    }, "/lol-loot/v1/recipes/" + text + "_OPEN/craft", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                    Thread.Sleep(500);
                                }
                                else if (text.StartsWith("CHAMPION_"))
                                {
                                    apiCalls.PostObject<string[]>(new string[]
                                    {
                                    text
                                    }, "/lol-loot/v1/recipes/CHAMPION_disenchant/craft", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                    Thread.Sleep(500);
                                }
                            }
                        }
                        if (flag)
                        {

                            Dispose(true);
                            return Disenchant(itsInterface);
                        }
                    }
                    return itsInterface.Result(false, "");
                }
            }
            catch (Exception e)
            {

                Dispose(true);
                return itsInterface.Result(false, itsInterface.messages.ErrorDisenchant);

            }
        }
        public bool CreateGame(Interface itsInterface)
        { 
            try
            {
               
                using (var apiCalls = new ApiCalls())
                {
                    Lobby lobby = new Lobby();
                    lobby.gameMode = "CLASSIC";
                    lobby.queueId = 830;
                    var success = apiCalls.PostObject<Lobby>(lobby, "/lol-lobby/v2/lobby", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                    KillUxRender(itsInterface);
                    if (success == true)
                    {
                        itsInterface.dashboardHelper.UpdateLolStatus("In Lobby", itsInterface);
                        return itsInterface.Result(success, itsInterface.messages.SuccessCreateGame);
                    }
                    itsInterface.clientKiller.KillLeagueClient();
                    Thread.Sleep(7000);
                    Dispose(true);
                    return itsInterface.processManager.Start(itsInterface);
                }
            }
            catch (Exception e)
            {
                itsInterface.clientKiller.KillLeagueClient();
                Thread.Sleep(7000);
                Dispose(true);
                return itsInterface.processManager.Start(itsInterface);
            }
        }
        public bool StartQueue(Interface itsInterface)
        {
            using (var apiCalls = new ApiCalls())
            {
                itsInterface.dashboardHelper.UpdateLolStatus("In Queue", itsInterface);
                try
                {

                    try
                    {
                        /*
                         * if (API.leaverbuster()){ Logger.Status("LQP detected."); } else {API.StartQueue2();}
                        */

                        apiCalls.PostObject<string>("", "/lol-lobby/v2/lobby/matchmaking/search", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);

                        //IF RESPONSE IS EMPTY, THAT MEANS THE CHAMPION IS IN LQP
                        //TODO: CHECK THE RESPONSE, IF ITS EMPTY RUN THE LQP PRIOITY PROCESS

                    }
                    catch
                    {
                        LQP_HATASI:
                        Thread.Sleep(300000);
                        Dispose(true);
                        return StartQueue2(itsInterface);
                    }
                    Thread.Sleep(500);
                    Matchmaking matchmaking = apiCalls.GetObject<Matchmaking>("/lol-matchmaking/v1/search", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                    if (matchmaking.Equals(null) == false)
                    {
                        if (matchmaking.searchState.ToUpper() != Matchmaking.SearchStateEnum.SEARCHING.ToString() && matchmaking.searchState.ToUpper() != Matchmaking.SearchStateEnum.FOUND.ToString())
                        {
                            itsInterface.clientKiller.KillLeagueClient();
                            Thread.Sleep(7000);
                            Dispose(true);
                            return itsInterface.processManager.Start(itsInterface);
                        }
                        else
                        {
                            DateTime now = DateTime.Now;
                            for (; ; )
                            {
                                DateTime now2 = DateTime.Now;
                                TimeSpan timeSpan = now - now2;
                                if (timeSpan.TotalMinutes > 30.0 || timeSpan.TotalMinutes < -30.0)
                                {
                                    goto ARAMAHATASIUZUNSURE;
                                }
                                matchmaking = apiCalls.GetObject<Matchmaking>("/lol-matchmaking/v1/search", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                try
                                {
                                    itsInterface.gameflowSession = apiCalls.GetObject<GameflowSession>("/lol-gameflow/v1/session", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                    goto MACHATASI;
                                }
                                catch
                                {
                                    goto MACHATASI;
                                }
                                MACBULUNDUSTATE:
                                if (matchmaking.searchState.ToUpper() == "FOUND")
                                {
                                    KillUxRender(itsInterface);
                                    try
                                    {
                                        Thread.Sleep(50);
                                        if (apiCalls.PostObject<string>("", "/lol-matchmaking/v1/ready-check/accept", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort))
                                        {

                                        }
                                        if (apiCalls.PostObject<string>("", "/lol-lobby-team-builder/v1/ready-check/accept", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort))
                                        {

                                        }
                                    }
                                    catch
                                    {
                                    }
                                    if (itsInterface.gameflowSession != null && ((itsInterface.gameflowSession != null) ? itsInterface.gameflowSession.phase.ToUpper() : null) == GameflowSession.GameflowSessionEnum.CHAMPSELECT.ToString())
                                    {
                                        break;
                                    }
                                }
                                if (itsInterface.gameflowSession != null && ((itsInterface.gameflowSession != null) ? itsInterface.gameflowSession.phase.ToUpper() : null) == GameflowSession.GameflowSessionEnum.CHAMPSELECT.ToString())
                                {
                                    break;
                                }
                                continue;
                            MACHATASI:
                                if (matchmaking.Equals(null) == false)
                                {
                                    goto MACBULUNDUSTATE;
                                }
                                else
                                {
                                    itsInterface.clientKiller.KillLeagueClient();
                                    Thread.Sleep(7000);
                                    Dispose(true);
                                    return itsInterface.processManager.Start(itsInterface);
                                }
                            }
                            goto IL_28C;
                        ARAMAHATASIUZUNSURE:
                            itsInterface.clientKiller.KillLeagueClient();
                            Thread.Sleep(7000);
                            Dispose(true);
                            return itsInterface.processManager.Start(itsInterface);
                        IL_28C:
                            Thread.Sleep(50);
                            itsInterface.gameflowSession = apiCalls.GetObject<GameflowSession>("/lol-gameflow/v1/session", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                            while (itsInterface.gameflowSession == null && itsInterface.gameflowSession.phase.ToUpper() != GameflowSession.GameflowSessionEnum.CHAMPSELECT.ToString())
                            {
                                itsInterface.gameflowSession = apiCalls.GetObject<GameflowSession>("/lol-gameflow/v1/session", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                            }
                            while (itsInterface.gameflowSession.phase.ToUpper() != GameflowSession.GameflowSessionEnum.CHAMPSELECT.ToString())
                            {
                                itsInterface.gameflowSession = apiCalls.GetObject<GameflowSession>("/lol-gameflow/v1/session", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                if (itsInterface.gameflowSession.phase.ToUpper() == GameflowSession.GameflowSessionEnum.LOBBY.ToString() || itsInterface.gameflowSession.phase.ToUpper() == GameflowSession.GameflowSessionEnum.NONE.ToString())
                                {
                                    Dispose(true);
                                    return StartQueue2(itsInterface);
                                }
                            }
                            if (itsInterface.gameflowSession.phase.ToUpper() == GameflowSession.GameflowSessionEnum.CHAMPSELECT.ToString())
                            {
                                KillUxRender(itsInterface);
                                PickRandomAvailableChampion(itsInterface);
                                SetSpell(itsInterface);
                                KillUxRender(itsInterface);
                                itsInterface.gameflowSession = apiCalls.GetObject<GameflowSession>("/lol-gameflow/v1/session", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                while (itsInterface.gameflowSession.phase.ToUpper() != GameflowSession.GameflowSessionEnum.GAMESTART.ToString() || itsInterface.gameflowSession.phase.ToUpper() != GameflowSession.GameflowSessionEnum.INPROGRESS.ToString())
                                {
                                    if (itsInterface.gameflowSession.phase.ToUpper() == GameflowSession.GameflowSessionEnum.LOBBY.ToString() || itsInterface.gameflowSession.phase.ToUpper() == GameflowSession.GameflowSessionEnum.MATCHMAKING.ToString() || itsInterface.gameflowSession.phase.ToUpper() == GameflowSession.GameflowSessionEnum.NONE.ToString())
                                    {
                                        Dispose(true);
                                        return StartQueue2(itsInterface);
                                    }
                                    if (itsInterface.gameflowSession.phase.ToUpper() == GameflowSession.GameflowSessionEnum.INPROGRESS.ToString())
                                    {
                                        break;
                                    }
                                    itsInterface.gameflowSession = apiCalls.GetObject<GameflowSession>("/lol-gameflow/v1/session", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                }
                            }
                            else
                            {
                                itsInterface.clientKiller.KillLeagueClient();
                                Thread.Sleep(7000);
                                Dispose(true);
                                return itsInterface.processManager.Start(itsInterface);
                            }
                        }
                    }
                    else
                    {
                        Dispose(true);
                        return StartQueue2(itsInterface);
                    }
                }
                catch (Exception ex)
                {
                    itsInterface.clientKiller.KillLeagueClient();
                    Thread.Sleep(7000);
                    Dispose(true);
                    return itsInterface.processManager.Start(itsInterface);
                }
            }
            return itsInterface.Result(true, "");
        }

        public bool StartQueue2(Interface itsInterface)
        {
            using (var apiCalls = new ApiCalls())
            {
                itsInterface.dashboardHelper.UpdateLolStatus("In Queue", itsInterface);
                try
                {

                    try
                    {
                        /*
                         * if (API.leaverbuster()){ Logger.Status("LQP detected."); } else {API.StartQueue();}
                        */

                        apiCalls.PostObject<string>("", "/lol-lobby/v2/lobby/matchmaking/search", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);

                        //IF RESPONSE IS EMPTY, THAT MEANS THE CHAMPION IS IN LQP
                        //TODO: CHECK THE RESPONSE, IF ITS EMPTY RUN THE LQP PRIOITY PROCESS

                    }
                    catch
                    {
                        LQP_HATASI:
                        Thread.Sleep(300000);
                        Dispose(true);
                        return StartQueue(itsInterface);
                    }
                    Thread.Sleep(500);
                    Matchmaking matchmaking = new Matchmaking();
                    matchmaking = apiCalls.GetObject<Matchmaking>("/lol-matchmaking/v1/search", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                    if (matchmaking.Equals(null) == false)
                    {
                        if (matchmaking.searchState.ToUpper() != Matchmaking.SearchStateEnum.SEARCHING.ToString() && matchmaking.searchState.ToUpper() != Matchmaking.SearchStateEnum.FOUND.ToString())
                        {
                            itsInterface.clientKiller.KillLeagueClient();
                            Thread.Sleep(7000);
                            Dispose(true);
                            return itsInterface.processManager.Start(itsInterface);
                        }
                        else
                        {
                            DateTime now = DateTime.Now;
                            for (;;)
                            {
                                DateTime now2 = DateTime.Now;
                                TimeSpan timeSpan = now - now2;
                                if (timeSpan.TotalMinutes > 30.0 || timeSpan.TotalMinutes < -30.0)
                                {
                                    goto ARAMAHATASIUZUNSURE;
                                }
                                matchmaking = apiCalls.GetObject<Matchmaking>("/lol-matchmaking/v1/search", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                try
                                {
                                    itsInterface.gameflowSession = apiCalls.GetObject<GameflowSession>("/lol-gameflow/v1/session", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                    goto MACHATASI;
                                }
                                catch
                                {
                                    goto MACHATASI;
                                }
                                MACBULUNDUSTATE:
                                if (matchmaking.searchState.ToUpper() == "FOUND")
                                {
                                    KillUxRender(itsInterface);
                                    try
                                    {
                                        Thread.Sleep(50);
                                        if (apiCalls.PostObject<string>("", "/lol-matchmaking/v1/ready-check/accept", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort))
                                        {

                                        }
                                        if (apiCalls.PostObject<string>("", "/lol-lobby-team-builder/v1/ready-check/accept", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort))
                                        {

                                        }
                                    }
                                    catch
                                    {
                                    }
                                    if (itsInterface.gameflowSession != null && ((itsInterface.gameflowSession != null) ? itsInterface.gameflowSession.phase.ToUpper() : null) == GameflowSession.GameflowSessionEnum.CHAMPSELECT.ToString())
                                    {
                                        break;
                                    }
                                }
                                if (itsInterface.gameflowSession != null && ((itsInterface.gameflowSession != null) ? itsInterface.gameflowSession.phase.ToUpper() : null) == GameflowSession.GameflowSessionEnum.CHAMPSELECT.ToString())
                                {
                                    break;
                                }
                                continue;
                            MACHATASI:
                                if (matchmaking.Equals(null) == false)
                                {
                                    goto MACBULUNDUSTATE;
                                }
                                else
                                {
                                    Dispose(true);
                                    itsInterface.clientKiller.KillLeagueClient();
                                    Thread.Sleep(7000);
                                    return itsInterface.processManager.Start(itsInterface);
                                }
                            }
                            goto IL_28C;
                        ARAMAHATASIUZUNSURE:
                            Dispose(true);
                            itsInterface.clientKiller.KillLeagueClient();
                            Thread.Sleep(7000);
                            return itsInterface.processManager.Start(itsInterface);
                        IL_28C:
                            Thread.Sleep(50);
                            itsInterface.gameflowSession = apiCalls.GetObject<GameflowSession>("/lol-gameflow/v1/session", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                            while (itsInterface.gameflowSession == null && itsInterface.gameflowSession.phase.ToUpper() != GameflowSession.GameflowSessionEnum.CHAMPSELECT.ToString())
                            {
                                itsInterface.gameflowSession = apiCalls.GetObject<GameflowSession>("/lol-gameflow/v1/session", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                            }
                            while (itsInterface.gameflowSession.phase.ToUpper() != GameflowSession.GameflowSessionEnum.CHAMPSELECT.ToString())
                            {
                                itsInterface.gameflowSession = apiCalls.GetObject<GameflowSession>("/lol-gameflow/v1/session", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                if (itsInterface.gameflowSession.phase.ToUpper() == GameflowSession.GameflowSessionEnum.LOBBY.ToString() || itsInterface.gameflowSession.phase.ToUpper() == GameflowSession.GameflowSessionEnum.NONE.ToString())
                                {
                                    Dispose(true);
                                    return StartQueue(itsInterface);
                                }
                            }
                            if (itsInterface.gameflowSession.phase.ToUpper() == GameflowSession.GameflowSessionEnum.CHAMPSELECT.ToString())
                            {
                                KillUxRender(itsInterface);
                                PickRandomAvailableChampion(itsInterface);
                                SetSpell(itsInterface);
                                KillUxRender(itsInterface);
                                itsInterface.gameflowSession = apiCalls.GetObject<GameflowSession>("/lol-gameflow/v1/session", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                while (itsInterface.gameflowSession.phase.ToUpper() != GameflowSession.GameflowSessionEnum.GAMESTART.ToString() || itsInterface.gameflowSession.phase.ToUpper() != GameflowSession.GameflowSessionEnum.INPROGRESS.ToString())
                                {
                                    if (itsInterface.gameflowSession.phase.ToUpper() == GameflowSession.GameflowSessionEnum.LOBBY.ToString() || itsInterface.gameflowSession.phase.ToUpper() == GameflowSession.GameflowSessionEnum.MATCHMAKING.ToString() || itsInterface.gameflowSession.phase.ToUpper() == GameflowSession.GameflowSessionEnum.NONE.ToString())
                                    {
                                        Dispose(true);
                                        return StartQueue(itsInterface);
                                    }
                                    if (itsInterface.gameflowSession.phase.ToUpper() == GameflowSession.GameflowSessionEnum.INPROGRESS.ToString())
                                    {
                                        break;
                                    }
                                    itsInterface.gameflowSession = apiCalls.GetObject<GameflowSession>("/lol-gameflow/v1/session", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                }
                            }
                            else
                            {
                                Dispose(true);
                                itsInterface.clientKiller.KillLeagueClient();
                                Thread.Sleep(7000);
                                return itsInterface.processManager.Start(itsInterface);
                            }
                        }
                    }
                    else
                    {
                        Dispose(true);
                        return StartQueue(itsInterface);
                    }
                    Dispose(true);
                }
                catch (Exception ex)
                {
                    Dispose(true);
                    itsInterface.clientKiller.KillLeagueClient();
                    Thread.Sleep(7000);
                    return itsInterface.processManager.Start(itsInterface);
                }
            }
            return itsInterface.Result(true, "");
        }

        public bool SearchQueue(Interface itsInterface)
        {
            using (var apiCalls = new ApiCalls())
            {
                try
                {
                    // Set dashboard status
                    itsInterface.dashboardHelper.UpdateLolStatus("In Queue", itsInterface);

                    try
                    {
                        // Start matchmaking
                        itsInterface.matchmaking =  apiCalls.GetObject<Matchmaking>("/lol-lobby/v2/lobby/matchmaking/search-state", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                        //if (itsInterface.matchmaking.lowPriorityData.penaltyTime > 0)
                        //{
                        //    Thread.Sleep(300000);
                        //    itsInterface.logger.Log(false, "LPQ Detected!");
                        //    Dispose(true);
                        //    return CallQueue(itsInterface);
                        //}
                    }
                    catch (Exception e)
                    {
                        Thread.Sleep(300000);
                        itsInterface.logger.Log(false, "LPQ Detected!");
                        Dispose(true);
                        return CallQueue(itsInterface);
                    }
                    Thread.Sleep(500);

                    // Start queue search
                    itsInterface.matchmaking = apiCalls.GetObject<Matchmaking>("/lol-matchmaking/v1/search", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                    if (itsInterface.matchmaking.Equals(null) == false)
                    {
                        string searchState = itsInterface.matchmaking.searchState.ToUpper();
                        if (searchState != Matchmaking.SearchStateEnum.SEARCHING.ToString() && searchState != Matchmaking.SearchStateEnum.FOUND.ToString())
                        {
                            itsInterface.clientKiller.KillLeagueClient();
                            Thread.Sleep(7000);
                            Dispose(true);
                            return itsInterface.processManager.Start(itsInterface);
                        }
                        else
                        {
                            DateTime now = DateTime.Now;
                            while (true)
                            {
                                DateTime now2 = DateTime.Now;
                                TimeSpan timeSpan = now - now2;

                                // Check for searching error
                                if (timeSpan.TotalMinutes > 30.0 || timeSpan.TotalMinutes < -30.0)
                                {
                                    itsInterface.clientKiller.KillLeagueClient();
                                    Thread.Sleep(7000);
                                    Dispose(true);
                                    return itsInterface.processManager.Start(itsInterface);
                                }

                                // Search game
                                itsInterface.matchmaking = apiCalls.GetObject<Matchmaking>("/lol-matchmaking/v1/search", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                try
                                {
                                    // Get current game session
                                    itsInterface.gameflowSession = apiCalls.GetObject<GameflowSession>("/lol-gameflow/v1/session", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                }
                                catch { }

                                string gFS = itsInterface.gameflowSession?.phase.ToUpper();

                                if (searchState == Matchmaking.SearchStateEnum.FOUND.ToString())
                                {
                                    KillUxRender(itsInterface);
                                    try
                                    {
                                        Thread.Sleep(50);
                                        //Accept Match
                                        apiCalls.PostObject<string>("", "/lol-matchmaking/v1/ready-check/accept", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);

                                        // Accept Match
                                        apiCalls.PostObject<string>("", "/lol-lobby-team-builder/v1/ready-check/accept", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                    }
                                    catch { }

                                    if (itsInterface.gameflowSession != null && gFS == GameflowSession.GameflowSessionEnum.CHAMPSELECT.ToString())
                                    {
                                        // Champ Select now
                                        return ChampSelect(itsInterface);

                                    }
                                }
                                if (itsInterface.gameflowSession != null && gFS == GameflowSession.GameflowSessionEnum.CHAMPSELECT.ToString())
                                {
                                    // Champ Select now
                                    return ChampSelect(itsInterface);
                                }
                            }


                        }
                    }
                    else
                    {
                        Dispose(true);
                        return CallQueue(itsInterface);
                    }

                }
                catch
                {
                    itsInterface.clientKiller.KillLeagueClient();
                    Thread.Sleep(7000);
                    Dispose(true);
                    return itsInterface.processManager.Start(itsInterface);
                }
            }
        }

        public bool ChampSelect(Interface itsInterface)
        {
            Thread.Sleep(50);
            using (var apiCalls = new ApiCalls())
            {
                itsInterface.gameflowSession = apiCalls.GetObject<GameflowSession>("/lol-gameflow/v1/session", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                string gFS = itsInterface.gameflowSession.phase.ToUpper();
                if (gFS == GameflowSession.GameflowSessionEnum.LOBBY.ToString() || gFS == GameflowSession.GameflowSessionEnum.NONE.ToString())
                {
                    Dispose(true);
                    return SearchQueue(itsInterface);
                }

                if (gFS == GameflowSession.GameflowSessionEnum.CHAMPSELECT.ToString())
                {
                    KillUxRender(itsInterface);
                    PickRandomAvailableChampion(itsInterface);
                    SetSpell(itsInterface);
                    KillUxRender(itsInterface);
                    itsInterface.gameflowSession = apiCalls.GetObject<GameflowSession>("/lol-gameflow/v1/session", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                    while (gFS != GameflowSession.GameflowSessionEnum.GAMESTART.ToString() || gFS != GameflowSession.GameflowSessionEnum.INPROGRESS.ToString())
                    {
                        itsInterface.gameflowSession = apiCalls.GetObject<GameflowSession>("/lol-gameflow/v1/session", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                        if (gFS == GameflowSession.GameflowSessionEnum.LOBBY.ToString() || gFS != GameflowSession.GameflowSessionEnum.MATCHMAKING.ToString() || gFS == GameflowSession.GameflowSessionEnum.NONE.ToString())
                        {
                            Dispose(true);
                            return CallQueue(itsInterface);
                        }

                        if (gFS == GameflowSession.GameflowSessionEnum.INPROGRESS.ToString())
                        {
                            break;
                        }
                    }
                }
                else
                {
                    itsInterface.clientKiller.KillLeagueClient();
                    Thread.Sleep(7000);
                    Dispose(true);
                    return itsInterface.processManager.Start(itsInterface);
                }
            }
            
            return itsInterface.Result(true, "");
        }

        public bool CallQueue(Interface itsInterface)
        {
            return SearchQueue(itsInterface);
        }

        public bool SetSpell(Interface itsInterface)
        {
            try
            {
                //TODO Dashboard'a bağla! veya Spelleri randomlaştır.
                itsInterface.mySelection.wardSkinId = 0;
                itsInterface.mySelection.selectedSkinId = 0;
                itsInterface.mySelection.spell1Id = 4;
                itsInterface.mySelection.spell2Id = 7;
                using (var apiCalls = new ApiCalls())
                {
                    apiCalls.PatchObject<MySelection>(itsInterface.mySelection, "/lol-champ-select/v1/session/my-selection", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                }
            }
            catch
            {
                return itsInterface.Result(true, itsInterface.messages.ErrorSpell);
            }
            return itsInterface.Result(true, itsInterface.messages.SuccessSpell);
        }
        public bool PickRandomAvailableChampion(Interface itsInterface)
        {
            KillUxRender(itsInterface);
            int champion = 0;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            int[] objectArray = GetPickableChampions(itsInterface);
            List<int> champList = objectArray != null ? ((IEnumerable<int>)objectArray).ToList<int>() : (List<int>)null;
            champList?.Remove(34);
            champList?.Remove(136);
            champList?.Remove(68);
            champList?.Remove(777);
            champList?.Remove(54);
            champList?.Remove(147);
            champList?.Remove(777);
            champList?.Remove(360);
            champList?.Remove(526);
            champList?.Remove(234);

            List<int> champList2 = new List<int>();
            for (int i1 = 0; i1 < champList.Count; ++i1)
            {
                for (int i2 = 0; i2 < itsInterface.championDatas.ADCChampions.Count; ++i2)
                {
                    if (champList.Contains(itsInterface.championDatas.ADCChampions[i2]))
                        champList2.Add(itsInterface.championDatas.ADCChampions[i2]);
                }
            }
            if (champList2.Count > 0)
            {
                int index = new Random().Next(0, champList2.Count);
                champion = champList2[index];
            }
            else
            {
                int index = new Random().Next(0, champList.Count);
                champion = champList[index];
            }


            itsInterface.champSelectInfos.actorCellId = 0;
            itsInterface.champSelectInfos.championId = champion;
            itsInterface.champSelectInfos.completed = true;
            itsInterface.champSelectInfos.id = 0;
            itsInterface.champSelectInfos.type = "pick";

            using (var apiCalls = new ApiCalls())
            {
                for (int k = 0; k < 6; k++)
                {
                    for (int l = 0; l < 6; l++)
                    {
                        try
                        {
                            itsInterface.champSelectInfos.actorCellId = k;
                            itsInterface.champSelectInfos.id = l;
                            apiCalls.PatchObject<ChampionSelectInformation>(itsInterface.champSelectInfos, "/lol-champ-select/v1/session/actions/" + k.ToString(), itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                            apiCalls.PostObject<int>(k, "/lol-champ-select/v1/session/actions/" + k.ToString() + "/complete", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                            goto IL_KIRMANOKTASI;
                        }
                        catch
                        {
                            goto IL_KIRMANOKTASI;
                        }
                        break;
                        IL_KIRMANOKTASI:;
                    }
                }

                itsInterface.gameflowSession = apiCalls.GetObject<GameflowSession>("/lol-gameflow/v1/session", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                if (!(itsInterface.gameflowSession.phase.ToUpper() != GameflowSession.GameflowSessionEnum.CHAMPSELECT.ToString()))
                {
                    int num3 = 0;
                    num3 = apiCalls.GetObject<int>("/lol-champ-select/v1/current-champion", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                    if (num3 != 0)
                    {
                        List<int> list3 = itsInterface.championDatas.LeagueChampions;
                        list3.Sort();
                        for (int m = 0; m < list3.Count; m++)
                        {
                            try
                            {
                                var leagueChampions = 266;
                                Enum.TryParse<int>(list3[m].ToString(), out leagueChampions);
                                int num4 = (int)leagueChampions;
                                if (num4 == champion)
                                {
                                    break;
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                    if (num3 == 0)
                    {
                        PickRandomAvailableChampion(itsInterface);
                    }
                }
            }
            return itsInterface.Result(true, itsInterface.messages.SuccessChampionPick);
        }

        public int[] GetPickableChampions(Interface itsInterface)
        {
            try
            {
                using (var request = CreateRequest(itsInterface))
                {
                    var result = request.Get("https://127.0.0.1:"+ itsInterface.apiVariables.IPort +"/lol-champ-select/v1/pickable-champion-ids").ToString();
                    result = Regex.Match(result, @"\[(.*)\]").Groups[1].Value;
                    return result.Split(',').Select(Int32.Parse).ToArray();
                }
            }
            catch
            {
                return GetPickableChampions(itsInterface);
            }

        }

        public bool SelectChampion(Interface itsInterface)
        {
            try
            {
                using (ApiCalls apiCalls = new ApiCalls())
                {
                    Missions[] missionArray = apiCalls.GetObject<Missions[]>("/lol-missions/v1/missions", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                    for (int index1 = 0; index1 < missionArray.Length; ++index1)
                    {
                        try
                        {
                            if ((!(missionArray[index1].missionType.ToUpper() == "ONETIME")
                                ? 0
                                : (missionArray[index1].status != "COMPLETED" ? 1 : 0)) != 0)
                            {
                                for (int index2 = 0; index2 < missionArray[index1].requirements.Length; ++index2)
                                {
                                    if (missionArray[index1].requirements[index2].ToUpper().Contains("LEVEL_UP:IN:[2]"))
                                    {
                                        apiCalls.PutObject<PutMission>(new PutMission()
                                        {
                                            rewardGroups = new string[1]
                                            {
                                                "ahri_group"
                                            }
                                        }, "/lol-missions/v1/player/" + missionArray[index1].id, itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/lol-missions/v1/force", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/riotclient/kill-and-restart-ux", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                    }
                                    else if (missionArray[index1].requirements[index2]
                                        .Contains("MISSION:COMPLETED:npe_rewards_login_v1_step4:AFTER_DELAY:PT17H"))
                                    {
                                        apiCalls.PutObject<PutMission>(new PutMission()
                                        {
                                            rewardGroups = new string[1]
                                            {
                                                "ekko_group"
                                            }
                                        }, "/lol-missions/v1/player/" + missionArray[index1].id, itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/lol-missions/v1/force", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/riotclient/kill-and-restart-ux", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                    }
                                    else if (missionArray[index1].requirements[index2]
                                        .Contains("MISSION:COMPLETED:npe_rewards_login_v1_step1:AFTER_DELAY:PT17H"))
                                    {
                                        apiCalls.PutObject<PutMission>(new PutMission()
                                        {
                                            rewardGroups = new string[1]
                                            {
                                                "illaoi_group"
                                            }
                                        }, "/lol-missions/v1/player/" + missionArray[index1].id, itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/lol-missions/v1/force", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/riotclient/kill-and-restart-ux", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                    }
                                    else if (missionArray[index1].requirements[index2]
                                        .Contains("MISSION:COMPLETED:npe_rewards_login_v1_step3:AFTER_DELAY:PT17H"))
                                    {
                                        apiCalls.PutObject<PutMission>(new PutMission()
                                        {
                                            rewardGroups = new string[1]
                                            {
                                                "nami_group"
                                            }
                                        }, "/lol-missions/v1/player/" + missionArray[index1].id, itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/lol-missions/v1/force", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/riotclient/kill-and-restart-ux", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                    }
                                    else if (missionArray[index1].requirements[index2]
                                        .Contains("SERIES:OPT_IN:npe_rewards_login_v1_series"))
                                    {
                                        apiCalls.PutObject<PutMission>(new PutMission()
                                        {
                                            rewardGroups = new string[1]
                                            {
                                                "caitlyn_group"
                                            }
                                        }, "/lol-missions/v1/player/" + missionArray[index1].id, itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/lol-missions/v1/force", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/riotclient/kill-and-restart-ux", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                    }
                                    else if (missionArray[index1].requirements[index2]
                                        .Contains("MISSION:COMPLETED:npe_rewards_login_v1_step2:AFTER_DELAY:PT17H"))
                                    {
                                        apiCalls.PutObject<PutMission>(new PutMission()
                                        {
                                            rewardGroups = new string[1]
                                            {
                                                "brand_group"
                                            }
                                        }, "/lol-missions/v1/player/" + missionArray[index1].id, itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/lol-missions/v1/force", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/riotclient/kill-and-restart-ux", itsInterface.apiVariables.IAuth, itsInterface.apiVariables.IPort);
                                    }
                                }
                            }
                            Dispose(true);
                            return itsInterface.Result(true, "");
                        }
                        catch
                        {
                            Console.WriteLine("MISSION FIX HATA 1");
                            Dispose(true);
                            return itsInterface.Result(true, "");
                        }
                    }
                    Dispose(true);
                    return itsInterface.Result(true, "");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MISSION FIX HATA 2");
                Dispose(true);
                return itsInterface.Result(true, "");
            }
        }


        public HttpRequest CreateRequest(Interface itsInterface)
        {
            itsInterface.request.IgnoreProtocolErrors = true;
            itsInterface.request.CharacterSet = HttpRequestEncoding;
            itsInterface.request.AddHeader("Authorization", "Basic " + itsInterface.apiVariables.IAuth);
            return itsInterface.request;
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
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
