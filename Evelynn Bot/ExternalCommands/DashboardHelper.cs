using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using bAUTH;
using Evelynn_Bot.Constants;
using Evelynn_Bot.Entities;
using Evelynn_Bot.GameAI;
using Evelynn_Bot.League_API.GameData;
using Quartz;
using Quartz.Impl;

namespace Evelynn_Bot.ExternalCommands
{
    public class DashboardHelper
    {

        public const string URI = "https://api.ytdtoken.space/v1/";

        public bool whileLoop = true;
        public int onlineClient = 0;
        public async void LoginAndStartBot(string username, string password, Interface itsInterface)
        {
            string r = itsInterface.req.CreateRequest(URI,
                new string[] { "data" },
                new string[] { itsInterface.sec.EncryptString(
                    string.Format("{0}|{1}|{2}|{3}|{4}",
                        itsInterface.u.GetRandomString(new Random().Next(100,128)),
                        "LOGIN",
                        username,
                        password,
                        itsInterface.u.GetRandomString(new Random().Next(100, 128))
                    ))},
                Method.POST);

            itsInterface.license = itsInterface.req.VerifyLicense(r, itsInterface);

            if (itsInterface.license.Status)
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

                
                job.JobDataMap["InterfaceClass"] = itsInterface;


                // Trigger the job to run now, and then repeat every 60 seconds
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("TriggerActions", "DBLicenseAndAction")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(60)
                        .RepeatForever())
                    .Build();
                

                // Tell quartz to schedule the job using our trigger
                await scheduler.ScheduleJob(job, trigger);

                itsInterface.logger.Log(false, itsInterface.messages.WaitingForStart);
                Console.WriteLine(itsInterface.dashboard.IsStop);
                CHECKSTART:
                if (itsInterface.dashboard.IsStart)
                {
                    itsInterface.dashboard.IsStart = false;
                    itsInterface.dashboard.IsStop = false; 
                    itsInterface.dashboard.IsRestart = false; 
                    itsInterface.processManager.Start(itsInterface);
                }
                else
                {
                    goto CHECKSTART;
                }


                Console.ReadLine();
            }
            else
            {
                itsInterface.logger.Log(false,itsInterface.messages.ErrorLogin);
                Console.ReadLine();
            }


        }

        public void UpdateLolWallet(string level, string be, Interface itsInterface)
        {
            string botRequest = itsInterface.req.CreateRequest(URI,
                new string[] { "data" },
                new string[] { itsInterface.sec.EncryptString(
                    string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}",
                        itsInterface.u.GetRandomString(new Random().Next(100,128)),
                        "UPDATE_LOL_WALLET",
                        itsInterface.license.Username,
                        itsInterface.license.Password,
                        itsInterface.license.ID,
                        itsInterface.license.Last,
                        level,
                        be
                    ))},
                Method.POST);

            Console.WriteLine(DecryptString(botRequest));
        }

        public void UpdateLolStatus(string status, Interface itsInterface)
        {
            string botRequest = itsInterface.req.CreateRequest(URI,
                new string[] { "data" },
                new string[] { itsInterface.sec.EncryptString(
                    string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}",
                        itsInterface.u.GetRandomString(new Random().Next(100,128)),
                        "CHANGE_LOL_STATUS",
                        itsInterface.license.Username,
                        itsInterface.license.Password,
                        itsInterface.license.ID,
                        itsInterface.license.Last,
                        status
                    ))},
                Method.POST);

            if (status == "Finished")
            {
                itsInterface.req.GetNewLoLAccount(botRequest, itsInterface);
            }
        }

        public void UpdateLPQStatus(string tF, Interface itsInterface)
        {
            string botRequest = itsInterface.req.CreateRequest(URI,
                new string[] { "data" },
                new string[] { itsInterface.sec.EncryptString(
                    string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}",
                        itsInterface.u.GetRandomString(new Random().Next(100,128)),
                        "UPDATE_LPQ_STATUS",
                        itsInterface.license.Username,
                        itsInterface.license.Password,
                        itsInterface.license.ID,
                        itsInterface.license.Last,
                        tF
                    ))},
                Method.POST);

            Console.WriteLine(DecryptString(botRequest));
        }


        public string DecryptString(string cipherText)
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine($"DECRYPT STRING HATA {e}");
                return "";
            }
        }
    }

    public class DashboardActionHelper : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            Interface itsInterface = (Interface)dataMap["InterfaceClass"];

            //ClientKiller.SuspendLeagueClient(); // Her dakikada hide ve suspend eder clienti \\ Gerekli mi bilmiyorum
            
            string r = itsInterface.req.CreateRequest(DashboardHelper.URI,
                new string[] { "data" },
                new string[] { itsInterface.sec.EncryptString(
                    string.Format("{0}|{1}|{2}|{3}|{4}|{5}",
                        itsInterface.u.GetRandomString(new Random().Next(100,128)),
                        "ACTION",
                        itsInterface.license.Username,
                        itsInterface.license.Password,
                        itsInterface.license.ID,
                        itsInterface.license.Last
                    ))},
                Method.POST);

            await itsInterface.req.GetActionStatus(r, itsInterface);
        }
    }

}
