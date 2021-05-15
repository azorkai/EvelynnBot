using Evelynn_Bot.Constants;
using Evelynn_Bot.Entities;
using Evelynn_Bot.League_API;
using Evelynn_Bot.Results;
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
using AutoIt;
using Evelynn_Bot.GameAI;

namespace Evelynn_Bot.Account_Process
{
    public class AccountProcess : IAccountProcess
    {
        private int caps = 0;

        private int errorCount;
        public static int SW_HIDE = 0;

        public static Encoding HttpRequestEncoding = Encoding.UTF8;

        ApiVariables apiVariables = new ApiVariables();
        public static Summoner summoner = new Summoner();
        public static Wallet wallet = new Wallet();
        JsonRead jsonRead = new JsonRead();

        [DllImport("User32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("User32.dll")]
        public static extern bool EnableWindow(IntPtr hwnd, bool enabled);

        public IResult StartLeague(License license)
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = license.LeaguePath;
                info.UseShellExecute = true;
                info.CreateNoWindow = true;
                info.WindowStyle = ProcessWindowStyle.Hidden;

                Process lol = Process.Start(info);
                lol.WaitForInputIdle();
                IntPtr HWND = FindWindow(null, "Riot Client");
                ShowWindow(HWND, SW_HIDE);
                EnableWindow(HWND, true);
                return new Result(true, Messages.SuccessStartLeague);


            }
            catch (Exception ex6)
            {
                return new Result(false, Messages.ErrorStartLeague);
            }
        }
        public IResult LoginAccount(License license)
        {
            Thread.Sleep(20000);
            try
            {
                using (ApiCalls apiCalls = new ApiCalls())
                {
                    if (license.Lol_username == "")
                    {
                        return new Result(false, Messages.ErrorNullUsername);
                    }
                    if (license.Lol_password == "")
                    {
                        return new Result(false, Messages.ErrorNullPassword);
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

                    string string_3 = $"{{ \"username\": \"{license.Lol_username}\", \"password\": \"{license.Lol_password}\", \"persistLogin\": false }}";

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
                    ClientKiller.SuspendLeagueClient();
                    Thread.Sleep(15000);

                    return new Result(true, Messages.SuccessLogin);
                }

            }
            catch (Exception e)
            {
                using (var helper = new Helper())
                {
                    helper.KillLeagueProcess();
                }

                return new Result(false, Messages.ErrorLogin);
            }
        }
        public IResult Initialize()
        {
            try
            {
                using (var fileStream = new FileStream(@jsonRead.Location() + "lockfile", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var streamReader = new StreamReader(fileStream, Encoding.Default))
                    {
                        string line;
                        while ((line = streamReader.ReadLine()) != null)
                        {
                            string[] lines = line.Split(':');
                            apiVariables.IPort = int.Parse(lines[2]);
                            string riot_pass = lines[3];
                            apiVariables.IAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes("riot:" + riot_pass));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return new Result(false, Messages.ErrorInitialize);
            }
            return new Result(true, Messages.SuccessInitialize);
        }
        public void GetSetWallet()
        {
            using (ApiCalls apiCalls = new ApiCalls())
            {
                summoner = apiCalls.GetObject<Summoner>("/lol-summoner/v1/current-summoner", apiVariables.IAuth, apiVariables.IPort);
                wallet = apiCalls.GetObject<Wallet>("/lol-store/v1/wallet", apiVariables.IAuth, apiVariables.IPort);
                DashboardHelper.UpdateLolWallet(summoner.summonerLevel.ToString(), wallet.ip.ToString());
            }
        }
        public void CheckNewAccount(License license)
        {
            if (string.IsNullOrEmpty(summoner.displayName))
            {
                Logger.Log(true, "New account!");
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

                NewLeaguePlayer newLeaguePlayer = new NewLeaguePlayer();
                newLeaguePlayer.name = RandomNameGenerator();

                using (ApiCalls apiCalls = new ApiCalls())
                {
                    if (apiCalls.PostObject<NewLeaguePlayer>(newLeaguePlayer, "/lol-summoner/v1/summoners", apiVariables.IAuth, apiVariables.IPort))
                    {
                        Logger.Log(true, "Successfully used name!");
                        ClientKiller.KillLeagueClient();
                        Thread.Sleep(7000);
                        ProcessManager.ProcessManager processManager = new ProcessManager.ProcessManager();
                        processManager.Start(license);
                    }
                    else
                    {
                        Logger.Log(true, "Need another name..");
                        newLeaguePlayer.name = RandomNameGenerator();
                        apiCalls.PostObject<NewLeaguePlayer>(newLeaguePlayer, "/lol-summoner/v1/summoners", apiVariables.IAuth, apiVariables.IPort);
                        Logger.Log(true, "Successfully used name!");
                        ClientKiller.KillLeagueClient();
                        Thread.Sleep(7000);
                        ProcessManager.ProcessManager processManager = new ProcessManager.ProcessManager();
                        processManager.Start(license);
                    }
                }
            }
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
        public void TutorialMissions(License license)
        {
            Summoner summoner = new Summoner();
            using (ApiCalls apiCalls = new ApiCalls())
            {
                DashboardHelper.UpdateLolStatus("Playing Tutorial", license);
                summoner = apiCalls.GetObject<Summoner>("/lol-summoner/v1/current-summoner", apiVariables.IAuth, apiVariables.IPort);
                Logger.Log(true, summoner.summonerLevel.ToString());
                if (summoner.summonerLevel < 11)
                {

                    Tutorial[] objectArray = apiCalls.GetObjectArray<Tutorial>("/lol-npe-tutorial-path/v1/tutorials", apiVariables.IAuth, apiVariables.IPort);
                    try
                    {
                        if (!objectArray[0].isViewed)
                        {
                            Logger.Log(true,"TUTORIAL 1");
                            apiCalls.PostObject<Lobby>(new Lobby
                            {
                                gameMode = "TUTORIAL",
                                queueId = 2000
                            }, "/lol-lobby/v2/lobby", apiVariables.IAuth, apiVariables.IPort);
                            Thread.Sleep(10000);
                            apiCalls.PostObject<string>("", "/lol-lobby/v2/lobby/matchmaking/search", apiVariables.IAuth, apiVariables.IPort);

                            using (GameAi gameAi = new GameAi())
                            {
                                gameAi.GameStartedTutorial();
                            }

                            Logger.Log(true,"TUTORIAL 1 ENDED");
                            Thread.Sleep(15000);
                            DoMission();
                            SelectChampion();
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
                            Logger.Log(true,"TUTORIAL 2");
                            apiCalls.PostObject<Lobby>(new Lobby
                            {
                                gameMode = "TUTORIAL",
                                queueId = 2010
                            }, "/lol-lobby/v2/lobby", apiVariables.IAuth, apiVariables.IPort);
                            Thread.Sleep(10000);
                            apiCalls.PostObject<string>("", "/lol-lobby/v2/lobby/matchmaking/search", apiVariables.IAuth, apiVariables.IPort);

                            using (GameAi gameAi = new GameAi())
                            {
                                gameAi.GameStartedTutorial();
                            }

                            Logger.Log(true,"TUTORIAL 2 ENDED");
                            Thread.Sleep(15000);
                            DoMission();
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
                            Logger.Log(true,"TUTORIAL 3");
                            apiCalls.PostObject<Lobby>(new Lobby
                            {
                                gameMode = "TUTORIAL",
                                queueId = 2020
                            }, "/lol-lobby/v2/lobby", apiVariables.IAuth, apiVariables.IPort);
                            Thread.Sleep(10000);
                            apiCalls.PostObject<string>("", "/lol-lobby/v2/lobby/matchmaking/search", apiVariables.IAuth, apiVariables.IPort);

                            using (GameAi gameAi = new GameAi())
                            {
                                gameAi.GameStartedTutorial();
                            }

                            Logger.Log(true,"TUTORIAL 3 ENDED");
                            Thread.Sleep(15000);
                            DoMission();
                            Thread.Sleep(5000);
                        }
                    }
                    catch
                    {
                    }
                }
            }
            
        }
        public void PatchCheck()
        {
            int tryNum = 1;
            while (LeagueIsPatchAvailable())
            {
                Logger.Log(true, "Patch bulundu!");
                Thread.Sleep(60000);
                tryNum++;
                if (tryNum >= 15)
                {
                    break;
                }
            }
            
        }
        public bool LeagueIsPatchAvailable()
        {
            using (ApiCalls apiCalls = new ApiCalls())
            {
                LeaguePatch lolPatchNew = apiCalls.GetObject<LeaguePatch>("/lol-patch/v1/products/league_of_legends/state", apiVariables.IAuth, apiVariables.IPort);
                //Logger.Status($@"Is there a new league patch: {lolPatchNew.isUpdateAvailable}");
                return lolPatchNew.isCorrupted || lolPatchNew.isUpdateAvailable || !lolPatchNew.isUpToDate;
            }
        }
        public void Disenchant()
        {
            try
            {
                using (ApiCalls apiCalls = new ApiCalls())
                {
                    if (caps < 50)
                    {
                        caps++;
                        string playerLoot = apiCalls.GetObject("/lol-loot/v1/player-loot-map", apiVariables.IAuth, apiVariables.IPort);
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
                                    }, "/lol-loot/v1/recipes/CHAMPION_RENTAL_disenchant/craft", apiVariables.IAuth, apiVariables.IPort);
                                    Thread.Sleep(500);
                                }
                                else if (text.StartsWith("CHEST_") && !text.StartsWith("CHEST_generic"))
                                {
                                    flag = true;
                                    apiCalls.PostObject<string[]>(new string[]
                                    {
                                    text
                                    }, "/lol-loot/v1/recipes/" + text + "_OPEN/craft", apiVariables.IAuth, apiVariables.IPort);
                                    Thread.Sleep(500);
                                }
                                else if (text.StartsWith("CHAMPION_"))
                                {
                                    apiCalls.PostObject<string[]>(new string[]
                                    {
                                    text
                                    }, "/lol-loot/v1/recipes/CHAMPION_disenchant/craft", apiVariables.IAuth, apiVariables.IPort);
                                    Thread.Sleep(500);
                                }
                            }
                        }
                        if (flag)
                        {
                            Disenchant();
                        }
                    }
                }
            }

            catch (Exception e)
            {
                Logger.Log(false, Messages.ErrorDisenchant);
            }
        }
        public IResult CreateGame(License license)
        {
            try
            {
                using (var apiCalls = new ApiCalls())
                {
                    Lobby lobby = new Lobby();
                    lobby.gameMode = "CLASSIC";
                    lobby.queueId = 830;
                    var success = apiCalls.PostObject<Lobby>(lobby, "/lol-lobby/v2/lobby", apiVariables.IAuth, apiVariables.IPort);
                    apiCalls.Dispose();
                    if (success == true)
                    {
                        DashboardHelper.UpdateLolStatus("In Lobby", license);
                        return new Result(success, Messages.SuccessCreateGame);
                    }
                    return new Result(success, Messages.ErrorCreateGame);
                }
            }
            catch (Exception e)
            {
                return new Result(true, Messages.ErrorCreateGame);
            }
        }
        public IResult StartQueue(License license)
        {
            using (var apiCalls = new ApiCalls())
            {
                DashboardHelper.UpdateLolStatus("In Queue", license);
                try
                {

                    errorCount++;
                    if (errorCount >= 15)
                    {
                        errorCount = 0;
                        Thread.Sleep(1000);
                        return new Result(false, Messages.ErrorCreateGameTooManyRequest);
                        //RestartBot();
                    }
                    try
                    {
                        /*
                         * if (API.leaverbuster()){ Logger.Status("LQP detected."); } else {API.StartQueue();}
                        */

                        apiCalls.PostObject<string>("", "/lol-lobby/v2/lobby/matchmaking/search", apiVariables.IAuth, apiVariables.IPort);

                        //IF RESPONSE IS EMPTY, THAT MEANS THE CHAMPION IS IN LQP
                        //TODO: CHECK THE RESPONSE, IF ITS EMPTY RUN THE LQP PRIOITY PROCESS

                    }
                    catch
                    {
                        LQP_HATASI:
                        Thread.Sleep(300000);
                        StartQueue(license);
                        return new Result(true, Messages.ErrorStartQueue);
                    }
                    Thread.Sleep(500);
                    Matchmaking matchmaking = new Matchmaking();
                    matchmaking = apiCalls.GetObject<Matchmaking>("/lol-matchmaking/v1/search", apiVariables.IAuth, apiVariables.IPort);
                    GameflowSession gameflowSession = new GameflowSession();
                    if (matchmaking != null)
                    {
                        if (matchmaking.searchState.ToUpper() != Matchmaking.SearchStateEnum.SEARCHING.ToString() && matchmaking.searchState.ToUpper() != Matchmaking.SearchStateEnum.FOUND.ToString())
                        {
                            using (var helper = new Helper())
                            {
                                helper.KillLeagueProcess();
                            }

                            //LoginAccount(new LeagueAccount()); //TODO Fix with new login system.
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
                                matchmaking = apiCalls.GetObject<Matchmaking>("/lol-matchmaking/v1/search", apiVariables.IAuth, apiVariables.IPort);
                                try
                                {
                                    gameflowSession = new GameflowSession();
                                    gameflowSession = apiCalls.GetObject<GameflowSession>("/lol-gameflow/v1/session", apiVariables.IAuth, apiVariables.IPort);
                                    goto MACHATASI;
                                }
                                catch
                                {
                                    goto MACHATASI;
                                }
                                MACBULUNDUSTATE:
                                if (matchmaking.searchState.ToUpper() == "FOUND")
                                {
                                    ClientKiller.SuspendLeagueClient(); // Maç bulunduğu anda suspendd ve hide eder.
                                    try
                                    {
                                        Thread.Sleep(50);
                                        if (apiCalls.PostObject<string>("", "/lol-matchmaking/v1/ready-check/accept", apiVariables.IAuth, apiVariables.IPort))
                                        {

                                        }
                                        if (apiCalls.PostObject<string>("", "/lol-lobby-team-builder/v1/ready-check/accept", apiVariables.IAuth, apiVariables.IPort))
                                        {

                                        }
                                    }
                                    catch
                                    {
                                    }
                                    if (gameflowSession != null && ((gameflowSession != null) ? gameflowSession.phase.ToUpper() : null) == GameflowSession.GameflowSessionEnum.CHAMPSELECT.ToString())
                                    {
                                        break;
                                    }
                                }
                                if (gameflowSession != null && ((gameflowSession != null) ? gameflowSession.phase.ToUpper() : null) == GameflowSession.GameflowSessionEnum.CHAMPSELECT.ToString())
                                {
                                    break;
                                }
                                continue;
                                MACHATASI:
                                if (matchmaking != null)
                                {
                                    goto MACBULUNDUSTATE;
                                }
                            }
                            goto IL_28C;
                            ARAMAHATASIUZUNSURE:
                            using (var helper = new Helper())
                            {
                                helper.KillLeagueProcess();
                            }
                            //LoginAccount(new LeagueAccount()); //TODO Fix with new login system.
                            IL_28C:
                            Thread.Sleep(50);
                            gameflowSession = new GameflowSession();
                            gameflowSession = apiCalls.GetObject<GameflowSession>("/lol-gameflow/v1/session", apiVariables.IAuth, apiVariables.IPort);
                            while (gameflowSession == null && gameflowSession.phase.ToUpper() != GameflowSession.GameflowSessionEnum.CHAMPSELECT.ToString())
                            {
                                gameflowSession = apiCalls.GetObject<GameflowSession>("/lol-gameflow/v1/session", apiVariables.IAuth, apiVariables.IPort);
                            }
                            while (gameflowSession.phase.ToUpper() != GameflowSession.GameflowSessionEnum.CHAMPSELECT.ToString())
                            {
                                gameflowSession = apiCalls.GetObject<GameflowSession>("/lol-gameflow/v1/session", apiVariables.IAuth, apiVariables.IPort);
                                if (gameflowSession.phase.ToUpper() == GameflowSession.GameflowSessionEnum.LOBBY.ToString() || gameflowSession.phase.ToUpper() == GameflowSession.GameflowSessionEnum.NONE.ToString())
                                {
                                    StartQueue(license);
                                    //return new Result(false, Messages.ErrorCreateGame);
                                }
                            }
                            if (gameflowSession.phase.ToUpper() == GameflowSession.GameflowSessionEnum.CHAMPSELECT.ToString())
                            {
                                ClientKiller.SuspendLeagueClient(); // Champ select geldiği gibi suspend ve hide et. 
                                PickRandomAvailableChampion();
                                Logger.Log(SetSpell().Success, SetSpell().Message);
                                gameflowSession = apiCalls.GetObject<GameflowSession>("/lol-gameflow/v1/session", apiVariables.IAuth, apiVariables.IPort);
                                while (gameflowSession.phase.ToUpper() != GameflowSession.GameflowSessionEnum.GAMESTART.ToString() || gameflowSession.phase.ToUpper() != GameflowSession.GameflowSessionEnum.INPROGRESS.ToString())
                                {
                                    if (gameflowSession.phase.ToUpper() == GameflowSession.GameflowSessionEnum.LOBBY.ToString() || gameflowSession.phase.ToUpper() == GameflowSession.GameflowSessionEnum.MATCHMAKING.ToString() || gameflowSession.phase.ToUpper() == GameflowSession.GameflowSessionEnum.NONE.ToString())
                                    {
                                        StartQueue(license);
                                        break;
                                    }
                                    if (gameflowSession.phase.ToUpper() == GameflowSession.GameflowSessionEnum.INPROGRESS.ToString())
                                    {
                                        break;
                                    }
                                    gameflowSession = apiCalls.GetObject<GameflowSession>("/lol-gameflow/v1/session", apiVariables.IAuth, apiVariables.IPort);
                                }
                            }
                            else
                            {
                                //DashboardHelper.SetOfflineAccount(int_1);
                                using (var helper = new Helper())
                                {
                                    helper.KillLeagueProcess();
                                }
                                Thread.Sleep(10000);
                                //LoginAccount(new LeagueAccount()); //TODO Fix with new login system.
                            }
                        }
                    }
                    else
                    {
                        //AccountProcess.FirstProcess();
                        StartQueue(license);
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.GetType().IsAssignableFrom(typeof(ThreadAbortException)))
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        //DashboardHelper.SetOfflineAccount(int_1);
                        using (var helper = new Helper())
                        {
                            helper.KillLeagueProcess();
                        }
                        Thread.Sleep(10000);
                        //LoginAccount(new LeagueAccount()); //TODO Fix with new login system.
                    }
                }
            }
            return null;
        }
        public IResult SetSpell()
        {
            try
            {
                //TODO Dashboard'a bağla! veya Spelleri randomlaştır.
                MySelection mySelection = new MySelection();
                mySelection.wardSkinId = 0;
                mySelection.selectedSkinId = 0;
                mySelection.spell1Id = 4;
                mySelection.spell2Id = 7;
                using (var apiCalls = new ApiCalls())
                {
                    apiCalls.PatchObject<MySelection>(mySelection, "/lol-champ-select/v1/session/my-selection", apiVariables.IAuth, apiVariables.IPort);
                }
            }
            catch
            {
                return new Result(true, Messages.ErrorSpell);
            }
            return new Result(true, Messages.SuccessSpell);
        }
        public IResult PickRandomAvailableChampion()
        {
            int champion = 0;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            int[] objectArray = GetPickableChampions();

            ChampionDatas.ADCChampions = (objectArray != null) ? objectArray.ToList<int>() : null;

            Random random = new Random();
            int index = random.Next(0, ChampionDatas.ADCChampions.Count);
            champion = ChampionDatas.ADCChampions[index];

            ChampionSelectInformation champSelectInfos = new ChampionSelectInformation();
            champSelectInfos.actorCellId = 0;
            champSelectInfos.championId = champion;
            champSelectInfos.completed = true;
            champSelectInfos.id = 0;
            champSelectInfos.type = "pick";

            using (var apiCalls = new ApiCalls())
            {
                for (int k = 0; k < 6; k++)
                {
                    for (int l = 0; l < 6; l++)
                    {
                        try
                        {
                            champSelectInfos.actorCellId = k;
                            champSelectInfos.id = l;
                            apiCalls.PatchObject<ChampionSelectInformation>(champSelectInfos, "/lol-champ-select/v1/session/actions/" + k.ToString(), apiVariables.IAuth, apiVariables.IPort);
                            apiCalls.PostObject<int>(k, "/lol-champ-select/v1/session/actions/" + k.ToString() + "/complete", apiVariables.IAuth, apiVariables.IPort);
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
                GameflowSession gameflowSession = new GameflowSession();
                gameflowSession = apiCalls.GetObject<GameflowSession>("/lol-gameflow/v1/session", apiVariables.IAuth, apiVariables.IPort);
                if (!(gameflowSession.phase.ToUpper() != GameflowSession.GameflowSessionEnum.CHAMPSELECT.ToString()))
                {
                    int num3 = 0;
                    num3 = apiCalls.GetObject<int>("/lol-champ-select/v1/current-champion", apiVariables.IAuth, apiVariables.IPort);
                    if (num3 != 0)
                    {
                        List<string> list3 = Enum.GetNames(typeof(ChampionDatas.LeagueChampions)).ToList<string>();
                        list3.Sort();
                        ChampionDatas.LeagueChampions leagueChampions = ChampionDatas.LeagueChampions.Aatrox;
                        for (int m = 0; m < list3.Count; m++)
                        {
                            try
                            {
                                Enum.TryParse<ChampionDatas.LeagueChampions>(list3[m], out leagueChampions);
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
                        PickRandomAvailableChampion();
                    }
                }
            }
            return new Result(true, Messages.SuccessChampionPick);
        }
        public int[] GetPickableChampions()
        {
            try
            {
                using (var request = CreateRequest())
                {
                    var result = request.Get("https://127.0.0.1:"+ apiVariables.IPort +"/lol-champ-select/v1/pickable-champion-ids").ToString();
                    result = Regex.Match(result, @"\[(.*)\]").Groups[1].Value;
                    return result.Split(',').Select(Int32.Parse).ToArray();
                }
            }
            catch
            {
                return GetPickableChampions();
            }

        }
        public void DoMission()
        {
            try
            {
                using (ApiCalls apiCalls = new ApiCalls())
                {
                    Missions[] object4 = apiCalls.GetObject<Missions[]>("/lol-missions/v1/missions", apiVariables.IAuth, apiVariables.IPort);
                    for (int n = 0; n < object4.Length; n++)
                    {
                        try
                        {
                            if (object4[n].missionType.ToUpper() == "ONETIME")
                            {
                                for (int num = 0; num < object4[n].requirements.Length; num++)
                                {
                                    if (object4[n].requirements[num].ToUpper().Contains("LEVEL_UP:IN:[2]"))
                                    {
                                        apiCalls.PutObject<PutMission>(new PutMission
                                        {
                                            rewardGroups = new string[]
                                            {
                                                        "ahri_group"
                                            }
                                        }, "/lol-missions/v1/player/" + object4[n].id, apiVariables.IAuth, apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/lol-missions/v1/force", apiVariables.IAuth, apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/riotclient/kill-and-restart-ux", apiVariables.IAuth, apiVariables.IPort);
                                    }
                                    else if (object4[n].requirements[num].Contains("MISSION:COMPLETED:npe_rewards_login_v1_step4:AFTER_DELAY:PT17H"))
                                    {
                                        apiCalls.PutObject<PutMission>(new PutMission
                                        {
                                            rewardGroups = new string[]
                                            {
                                                        "ekko_group"
                                            }
                                        }, "/lol-missions/v1/player/" + object4[n].id, apiVariables.IAuth, apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/lol-missions/v1/force", apiVariables.IAuth, apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/riotclient/kill-and-restart-ux", apiVariables.IAuth, apiVariables.IPort);
                                    }
                                    else if (object4[n].requirements[num].Contains("MISSION:COMPLETED:npe_rewards_login_v1_step1:AFTER_DELAY:PT17H"))
                                    {
                                        apiCalls.PutObject<PutMission>(new PutMission
                                        {
                                            rewardGroups = new string[]
                                            {
                                                        "illaoi_group"
                                            }
                                        }, "/lol-missions/v1/player/" + object4[n].id, apiVariables.IAuth, apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/lol-missions/v1/force", apiVariables.IAuth, apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/riotclient/kill-and-restart-ux", apiVariables.IAuth, apiVariables.IPort);
                                    }
                                    else if (object4[n].requirements[num].Contains("MISSION:COMPLETED:npe_rewards_login_v1_step3:AFTER_DELAY:PT17H"))
                                    {
                                        apiCalls.PutObject<PutMission>(new PutMission
                                        {
                                            rewardGroups = new string[]
                                            {
                                                        "nami_group"
                                            }
                                        }, "/lol-missions/v1/player/" + object4[n].id, apiVariables.IAuth, apiVariables.IPort);
                                       apiCalls. PostObject<string>("", "/lol-missions/v1/force", apiVariables.IAuth, apiVariables.IPort);
                                       apiCalls.PostObject<string>("", "/riotclient/kill-and-restart-ux", apiVariables.IAuth, apiVariables.IPort);
                                    }
                                    else if (object4[n].requirements[num].Contains("SERIES:OPT_IN:npe_rewards_login_v1_series"))
                                    {
                                        apiCalls.PutObject<PutMission>(new PutMission
                                        {
                                            rewardGroups = new string[]
                                            {
                                                        "caitlyn_group"
                                            }
                                        }, "/lol-missions/v1/player/" + object4[n].id, apiVariables.IAuth, apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/lol-missions/v1/force", apiVariables.IAuth, apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/riotclient/kill-and-restart-ux", apiVariables.IAuth, apiVariables.IPort);
                                    }
                                    else if (object4[n].requirements[num].Contains("MISSION:COMPLETED:npe_rewards_login_v1_step2:AFTER_DELAY:PT17H"))
                                    {
                                        apiCalls.PutObject<PutMission>(new PutMission
                                        {
                                            rewardGroups = new string[]
                                            {
                                                        "brand_group"
                                            }
                                        }, "/lol-missions/v1/player/" + object4[n].id, apiVariables.IAuth, apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/lol-missions/v1/force", apiVariables.IAuth, apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/riotclient/kill-and-restart-ux", apiVariables.IAuth, apiVariables.IPort);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            Console.WriteLine("MISSION FIX HATA 1");
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("MISSION FIX HATA 2");
            }
        }
        public void SelectChampion()
        {
            try
            {
                using (ApiCalls apiCalls = new ApiCalls())
                {
                    Missions[] @object = apiCalls.GetObject<Missions[]>("/lol-missions/v1/missions", apiVariables.IAuth, apiVariables.IPort);
                    for (int i = 0; i < @object.Length; i++)
                    {
                        try
                        {
                            if (@object[i].missionType.ToUpper() == "ONETIME" && @object[i].status != "COMPLETED")
                            {
                                for (int j = 0; j < @object[i].requirements.Length; j++)
                                {
                                    if (@object[i].requirements[j].ToUpper().Contains("LEVEL_UP:IN:[2]"))
                                    {
                                        apiCalls.PutObject<PutMission>(new PutMission
                                        {
                                            rewardGroups = new string[]
                                            {
                                            "ahri_group"
                                            }
                                        }, "/lol-missions/v1/player/" + @object[i].id, apiVariables.IAuth, apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/lol-missions/v1/force", apiVariables.IAuth, apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/riotclient/kill-and-restart-ux", apiVariables.IAuth, apiVariables.IPort);
                                    }
                                    else if (@object[i].requirements[j].Contains("MISSION:COMPLETED:npe_rewards_login_v1_step4:AFTER_DELAY:PT17H"))
                                    {
                                        apiCalls.PutObject<PutMission>(new PutMission
                                        {
                                            rewardGroups = new string[]
                                            {
                                            "ekko_group"
                                            }
                                        }, "/lol-missions/v1/player/" + @object[i].id, apiVariables.IAuth, apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/lol-missions/v1/force", apiVariables.IAuth, apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/riotclient/kill-and-restart-ux", apiVariables.IAuth, apiVariables.IPort);
                                    }
                                    else if (@object[i].requirements[j].Contains("MISSION:COMPLETED:npe_rewards_login_v1_step1:AFTER_DELAY:PT17H"))
                                    {
                                        apiCalls.PutObject<PutMission>(new PutMission
                                        {
                                            rewardGroups = new string[]
                                            {
                                            "illaoi_group"
                                            }
                                        }, "/lol-missions/v1/player/" + @object[i].id, apiVariables.IAuth, apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/lol-missions/v1/force", apiVariables.IAuth, apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/riotclient/kill-and-restart-ux", apiVariables.IAuth, apiVariables.IPort);
                                    }
                                    else if (@object[i].requirements[j].Contains("MISSION:COMPLETED:npe_rewards_login_v1_step3:AFTER_DELAY:PT17H"))
                                    {
                                        apiCalls.PutObject<PutMission>(new PutMission
                                        {
                                            rewardGroups = new string[]
                                            {
                                            "nami_group"
                                            }
                                        }, "/lol-missions/v1/player/" + @object[i].id, apiVariables.IAuth, apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/lol-missions/v1/force", apiVariables.IAuth, apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/riotclient/kill-and-restart-ux", apiVariables.IAuth, apiVariables.IPort);
                                    }
                                    else if (@object[i].requirements[j].Contains("SERIES:OPT_IN:npe_rewards_login_v1_series"))
                                    {
                                        apiCalls.PutObject<PutMission>(new PutMission
                                        {
                                            rewardGroups = new string[]
                                            {
                                            "caitlyn_group"
                                            }
                                        }, "/lol-missions/v1/player/" + @object[i].id, apiVariables.IAuth, apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/lol-missions/v1/force", apiVariables.IAuth, apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/riotclient/kill-and-restart-ux", apiVariables.IAuth, apiVariables.IPort);
                                    }
                                    else if (@object[i].requirements[j].Contains("MISSION:COMPLETED:npe_rewards_login_v1_step2:AFTER_DELAY:PT17H"))
                                    {
                                        apiCalls.PutObject<PutMission>(new PutMission
                                        {
                                            rewardGroups = new string[]
                                            {
                                            "brand_group"
                                            }
                                        }, "/lol-missions/v1/player/" + @object[i].id, apiVariables.IAuth, apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/lol-missions/v1/force", apiVariables.IAuth, apiVariables.IPort);
                                        apiCalls.PostObject<string>("", "/riotclient/kill-and-restart-ux", apiVariables.IAuth, apiVariables.IPort);
                                    }
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch
            {
            }
        }
        public HttpRequest CreateRequest()
        {
            HttpRequest request = new HttpRequest();
            request.IgnoreProtocolErrors = true;
            request.CharacterSet = HttpRequestEncoding;
            request.AddHeader("Authorization", "Basic " + apiVariables.IAuth);
            return request;
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
