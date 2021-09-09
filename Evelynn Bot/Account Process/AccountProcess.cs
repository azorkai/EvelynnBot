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
using bAUTH;
using Evelynn_Bot.GameAI;
using EvelynnLCU.API_Models;
using EvelynnLCU.Plugins.LoL;
using EvelynnLCU;
using EvelynnLCU.API_Models;

namespace Evelynn_Bot.Account_Process
{
    public class AccountProcess : IAccountProcess
    {
        public bool StartLeague(Interface itsInterface)
        {
            try
            { 
                itsInterface.clientKiller.KillAllLeague();
                string text = itsInterface.clientKiller.GetLeaguePath() + "Config\\";
                File.Delete($"{text}game.cfg");
                File.Delete($"{text}PersistedSettings.json");
                File.Copy(Directory.GetCurrentDirectory() + "\\Config\\game.cfg", $"{text}game.cfg", overwrite: true);
                File.Copy(Directory.GetCurrentDirectory() + "\\Config\\PersistedSettings.json", $"{text}PersistedSettings.json", overwrite: true);
                itsInterface.clientKiller.StartLeague();
                return itsInterface.Result(true, itsInterface.messages.SuccessStartLeague);
            }
            catch (Exception ex6)
            {
                Console.WriteLine($"LOL ACMA HATA: {ex6}");
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
                
                Dispose(true);
                itsInterface.lcuPlugins = null;
                return itsInterface.Result(true, itsInterface.messages.SuccessLogin);
            }
            catch (Exception e)
            {
                //TODO: EĞER GİRİŞ BAŞARISIZ İSE SİTEYE BU BİLGİYİ GÖNDER
                itsInterface.clientKiller.KillAllLeague();
                Dispose(true);
                return itsInterface.Result(false, itsInterface.messages.ErrorLogin);
            }
        }
        public async Task<Task> ChangeRegion(Interface itsInterface)
        {
            RegionLocale clientRegion =  await itsInterface.lcuPlugins.GetRegionAsync();
            string upper = itsInterface.license.Lol_region.ToUpper();
            string str = upper == "OC" ? "OC1" : upper;
            string region = str == "EUN" ? "EUNE" : str;
            Console.WriteLine($"Current Region: {clientRegion.region}");
            if (clientRegion.region != region)
            {
                await itsInterface.lcuPlugins.SetRegionAsync(region);
                Thread.Sleep(10000);
                return itsInterface.processManager.StartAccountProcess(itsInterface);
            }

            return Task.CompletedTask;
        }
        public async Task<string> VerifySession(Interface itsInterface)
        {
            int loginAttempt = 0;
            LolLoginLoginSession loginSession = await itsInterface.lcuPlugins.GetSessionAsync();
            bool? isConnected = loginSession.connected;
            if (isConnected.Value)
            {
                string? summonerName = loginSession.username;
                if (!String.IsNullOrEmpty(summonerName))
                {
                    summonerName = loginSession.username;
                    if (summonerName.Length > 0)
                    {
                        if (!(loginSession.isNewPlayer.GetValueOrDefault() & loginSession.isNewPlayer.HasValue))
                        {
                            // HESAP DOĞRULANDI DEVAM EDEBİLİRSİN
                            return "already_created_account";
                        }
                        // YENİ HESAP, SET SUMMONER KISMINA GÖTÜR
                        return "new_player_set_account";
                    }
                }
                if (!String.IsNullOrEmpty(summonerName))
                {
                    summonerName = loginSession.username;
                }
                else
                {
                    return "invalid_summoner_name";
                }
            }
            if (loginSession.error.messageId == "ACCOUNT_BANNED") // Account has banned
            {
                // Sent the banned account to the dashboard, then get new account.
                //GClass119.GClass119_0.method_12(Class14.gclass1_0.Int64_0);
                //Class14.action_0 = Class14.Action.RequestAccount;
                return "banned_account";
            }
            else if (loginSession.error.messageId != "RATE_LIMITED")
            {
                if (loginSession.error.messageId == "LOGIN_QUEUE_BUSY")
                {
                    Console.WriteLine("We have a rate limit on league side.");
                    Thread.Sleep(300000);
                    return "restart_client_error";
                }
                else if (loginSession.error.messageId == "UNSPECIFIED_ERROR") { return "restart_client_error"; }
                else if (loginSession.error.messageId == "INVALID_CREDENTIALS")
                {
                    ++loginAttempt;
                    // Account credentials are wrong
                    itsInterface.logger.Log(false, itsInterface.messages.ErrorLogin);
                    if (loginAttempt >= 3)
                    {
                        // BURADA HESABI PANELE BİLDİR VE YENI HESAP İSTE
                        //GClass115.GStruct0 gstruct0_2 = GClass119.GClass119_0.method_13(Class14.gclass1_0.Int64_0);
                        //Class14.action_0 = Class14.Action.RequestAccount;
                        return "invalid_credentials";
                    }
                    else
                    {
                        return "restart_client_error";
                    }
                }
                else if (loginSession.error.messageId == "LOGGED_IN_ELSEWHERE")
                {
                    // YENİ HESAP İSTE
                    //Class14.action_0 = Class14.Action.RequestAccount;
                    return "logged_in_from_another";
                }
                else if (loginSession.error.messageId == "CHANNEL_AUTH_FAILED")
                    return "restart_client_error";
            }
            else
            {
                // RATE LIMIT WAIT 5 MINUTES
                Console.WriteLine("We have a rate limit on league side.");
                Thread.Sleep(300000);
                return "restart_client_error";
            }
            return(loginSession.error.messageId);
        }
        public async Task<Task> CheckLeagueBan(Interface itsInterface)
        {
            var isBanned = await itsInterface.lcuPlugins.CheckBanAsync();
            if (isBanned.isPermaBan.Value)
            {
                // Account has perma banned!
                // TODO: BURDA HESABI PANELE GÖNDER SONRA YENİ HESAP AL ( SET LOL STATUS BANNED EKLENECEK PANELE )
                Console.WriteLine("ACCOUNT PERMA BANNED!");
                Console.WriteLine("ACCOUNT PERMA BANNED!");
                Console.WriteLine("ACCOUNT PERMA BANNED!");
                Console.WriteLine("ACCOUNT PERMA BANNED!");
                Console.WriteLine("ACCOUNT PERMA BANNED!");
                return Task.CompletedTask;
            }
            return Task.CompletedTask;

        }

        public void LogYaz(string Hata1, string LFile)
        {
            FileStream dosyaAc = new FileStream(@LFile,
                    FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter dosyaYaz = new StreamWriter(dosyaAc);
                dosyaYaz.BaseStream.Seek(0, SeekOrigin.End);
                dosyaYaz.WriteLine(Hata1);
                dosyaYaz.Flush(); dosyaYaz.Close();
            
        }

        public async Task<Task> OldClientLoginAccount(Interface itsInterface)
        {
            try
            {
                if (itsInterface.license.Lol_username == "")
                {
                     itsInterface.Result(false, itsInterface.messages.ErrorNullUsername);
                     return Task.CompletedTask;
                }

                if (itsInterface.license.Lol_password == "")
                {
                    itsInterface.Result(false, itsInterface.messages.ErrorNullPassword);
                    return Task.CompletedTask;
                }


                itsInterface.lcuApi.BeginTryInit(InitializeMethod.Lockfile);
                itsInterface.lcuApi.Socket.DumpToDebug = false;
                itsInterface.lcuPlugins = new Plugins(itsInterface.lcuApi);
                Thread.Sleep(3500);
                await ChangeRegion(itsInterface);

                string logins = await itsInterface.lcuPlugins.LoginSessionAsync(itsInterface.license.Lol_username, itsInterface.license.Lol_password);

                var LogFile = $@"{Path.GetTempPath()}\Eve.TXT";

                LogYaz(logins, LogFile);

                // Verify Session
                string session = await VerifySession(itsInterface);
                itsInterface.logger.Log(false, session);
                switch (session)
                {
                    case "banned_account":
                        // hesabı panele gönder ve yenisini al.
                        Console.WriteLine("BANNED ACCOUNT, GETTING NEW ACCOUNT");
                        break;
                    case "new_player_set_account":
                        await itsInterface.lcuPlugins.CompleteNewAccountAsync();
                        Console.WriteLine("NEW PLAYER, SETTING UP NEW PLAYER");
                        if (itsInterface.license.Lol_isEmptyNick == false)
                        {
                            Dispose(true);
                            using (AccountProcess accountProcess = new AccountProcess())
                            {
                                await accountProcess.CheckNewAccount(itsInterface);
                            }
                        }
                        break;
                    case "invalid_credentials":
                        Console.WriteLine("INVALID CREDENTIALS, GETTING NEW ACCOUNT");
                        break;
                    case "restart_client_error":
                        Console.WriteLine("CLIENT ERROR! RESTART");

                        return itsInterface.processManager.StartAccountProcess(itsInterface);
                    case "invalid_summoner_name":
                        Console.WriteLine("We have a invalid summoner name!");
                        break;
                    case "logged_in_from_another":
                        Console.WriteLine("This account has logged in from somewhere else already!");
                        break;
                    default:
                        Console.WriteLine($"LOGIN SESSION: {session}");
                        break;
                }
                
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

                Dispose(true);
                itsInterface.Result(true, itsInterface.messages.SuccessLogin);
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                Console.WriteLine($"LOGIN HATA: {e}");
                //TODO: EĞER GİRİŞ BAŞARISIZ İSE SİTEYE BU BİLGİYİ GÖNDER
                itsInterface.clientKiller.KillAllLeague();
                Dispose(true);
                itsInterface.Result(false, itsInterface.messages.ErrorLogin);
                return itsInterface.processManager.StartAccountProcess(itsInterface);
            }
        }
        public bool Initialize(Interface itsInterface)
        {
            try
            {
                itsInterface.lcuApi.BeginTryInit(InitializeMethod.Lockfile);
                itsInterface.lcuApi.Socket.DumpToDebug = false;
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
                itsInterface.lcuPlugins.KillUXAsync();

                itsInterface.logger.Log(true, "New account!");

                await Task.Delay(5000);

                await itsInterface.lcuPlugins.CompleteNewAccountAsync();

                var name = RandomNameGenerator();
                itsInterface.lcuPlugins.KillUXAsync();
                if (await itsInterface.lcuPlugins.SetSummonerName(name))
                {
                    itsInterface.logger.Log(true, "Successfully used name!");
                    Dispose(true);
                    return itsInterface.processManager.Start(itsInterface);
                }
                else
                {
                    itsInterface.logger.Log(true, "Error on creating nickname. Trying again!");
                    return CheckNewAccount(itsInterface);
                }
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
        public bool PatchCheck(Interface itsInterface)
        {
            try
            {
                int tryNum = 1;
                while (LeagueIsPatchAvailable(itsInterface))
                {
                    itsInterface.logger.Log(true, itsInterface.messages.Patch);
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
            return itsInterface.Result(lolPatchNew.isCorrupted.Value || lolPatchNew.isUpdateAvailable.Value || !lolPatchNew.isUpToDate.Value, "");
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
