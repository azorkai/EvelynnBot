using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using bAUTH;
using Evelynn_Bot.Constants;
using Evelynn_Bot.Entities;
using Quartz;
using Quartz.Impl;

namespace Evelynn_Bot.ExternalCommands
{
    public static class DashboardHelper
    {
        public const string URI = "https://api.ytdtoken.space/v1/";

        public static License license = new License();

        public static bUtils help = new bUtils();
        public static bHTTP req = new bHTTP();
        public static bSecurity sec = new bSecurity();

        public static bool whileLoop = true;
        public static int onlineClient = 0;

        public static async void LoginAndStartBot(string username, string password)
        {
            string r = req.CreateRequest(URI,
                new string[] { "data" },
                new string[] { sec.EncryptString(
                    string.Format("{0}|{1}|{2}|{3}|{4}",
                        help.GetRandomString(new Random().Next(100,128)),
                        "LOGIN",
                        username,
                        password,
                        help.GetRandomString(new Random().Next(100, 128))
                    ))},
                Method.POST);

            license = req.VerifyLicense(r);

            if (license.Status)
            {
                // Grab the Scheduler instance from the Factory
                StdSchedulerFactory factory = new StdSchedulerFactory();
                IScheduler scheduler = await factory.GetScheduler();

                // and start it off
                await scheduler.Start();

                // define the job and tie it to our HelloJob class
                IJobDetail job = JobBuilder.Create<DashboardActionHelper>()
                    .WithIdentity("GetActions", "DBLicenseAndAction")
                    .Build();

                // Trigger the job to run now, and then repeat every 10 seconds
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("TriggerActions", "DBLicenseAndAction")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(60)
                        .RepeatForever())
                    .Build();

                // Tell quartz to schedule the job using our trigger
                await scheduler.ScheduleJob(job, trigger);
                Logger.Log(false, Messages.WaitingForStart); 
                CHECKSTART:
                if (req.dashboardActions.IsStart)
                {
                    ProcessManager.ProcessManager processManager = new ProcessManager.ProcessManager();
                    processManager.Start(license);
                }
                else
                {
                    goto CHECKSTART;
                }


                Console.ReadLine();
            }
            else
            {
                Logger.Log(false,Messages.ErrorLogin);
                Console.ReadLine();
            }


        }

        public static void UpdateLolWallet(string level, string be)
        {
            string botRequest = req.CreateRequest(URI,
                new string[] { "data" },
                new string[] { sec.EncryptString(
                    string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}",
                        help.GetRandomString(new Random().Next(100,128)),
                        "UPDATE_LOL_WALLET",
                        license.Username,
                        license.Password,
                        license.ID,
                        license.Last,
                        level,
                        be
                    ))},
                Method.POST);

            Console.WriteLine(DecryptString(botRequest));
        }

        public static void UpdateLolStatus(string status, License l)
        {
            string botRequest = req.CreateRequest(URI,
                new string[] { "data" },
                new string[] { sec.EncryptString(
                    string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}",
                        help.GetRandomString(new Random().Next(100,128)),
                        "CHANGE_LOL_STATUS",
                        license.Username,
                        license.Password,
                        license.ID,
                        license.Last,
                        status
                    ))},
                Method.POST);

            if (status == "Finished")
            {
                req.GetNewLoLAccount(botRequest, l);
            }
        }

        public static void UpdateLPQStatus(string tF)
        {
            string botRequest = req.CreateRequest(URI,
                new string[] { "data" },
                new string[] { sec.EncryptString(
                    string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}",
                        help.GetRandomString(new Random().Next(100,128)),
                        "UPDATE_LPQ_STATUS",
                        license.Username,
                        license.Password,
                        license.ID,
                        license.Last,
                        tF
                    ))},
                Method.POST);

            Console.WriteLine(DecryptString(botRequest));
        }

        public static void LicenceCheck()
        {
            try
            {
                bUtils help = new bUtils();
                bHTTP req = new bHTTP();
                bSecurity sec = new bSecurity();
                if (license.Status == true)
                {
                    string rz = req.CreateRequest(URI,
                        new string[] { "data" },
                        new string[] { sec.EncryptString(
                            string.Format("{0}|{1}|{2}|{3}|{4}",
                                help.GetRandomString(new Random().Next(100,128)),
                                "REFRESH",
                                license.ID,
                                45,
                                help.GetRandomString(new Random().Next(100, 128))
                            ))},
                        Method.POST);
                    var vt = req.VerifyToken(rz);
                    license.Status = vt;
                    Console.WriteLine($"VT: {vt}    LS: {license.Status}");
                    onlineClient = req.GetOnlineClients(rz);
                    Console.WriteLine("CHECK VERIFIED");
                }
                else
                {
                    whileLoop = false;
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static string DecryptString(string cipherText)
        {
            // KEY ve IV fonksiyon icin artik gerekli degil.
            string password = "HnSdyBPo4I";
            SHA256 mySHA256 = SHA256Managed.Create();
            byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
            byte[] iv = new byte[16] { 0x9, 0xF, 0x9, 0x0, 0xF, 0x0, 0x0, 0x0, 0x0, 0xF, 0xA, 0x0, 0x1, 0x0, 0xF, 0x0 };

            Aes encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;
            encryptor.Key = key.Take(32).ToArray();
            encryptor.IV = iv;
            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);
            string plainText = String.Empty;
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
                cryptoStream.FlushFinalBlock();
                byte[] plainBytes = memoryStream.ToArray();
                plainText = Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length);
            }
            finally
            {
                memoryStream.Close();
                cryptoStream.Close();
            }
            return plainText;
        }



    }

    public class DashboardActionHelper : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            ClientKiller.SuspendLeagueClient(); // Her dakikada hide ve suspend eder clienti \\ Gerekli mi bilmiyorum
            string r = DashboardHelper.req.CreateRequest(DashboardHelper.URI,
                new string[] { "data" },
                new string[] { DashboardHelper.sec.EncryptString(
                    string.Format("{0}|{1}|{2}|{3}|{4}|{5}",
                        DashboardHelper.help.GetRandomString(new Random().Next(100,128)),
                        "ACTION",
                        DashboardHelper.license.Username,
                        DashboardHelper.license.Password,
                        DashboardHelper.license.ID,
                        DashboardHelper.license.Last
                    ))},
                Method.POST);

            await DashboardHelper.req.GetActionStatus(r);
        }
    }

}
