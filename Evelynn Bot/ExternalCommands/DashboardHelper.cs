using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public async void LoginAndStartBot(string username, string password, Interface itsInterface, bool method = false)
        {
            if (method)
            {
                string r = itsInterface.req.CreateRequest(URI,
                    new string[] { "data" },
                    new string[] { itsInterface.sec.EncryptString(
                        string.Format("{0}|{1}|{2}|{3}|{4}",
                            itsInterface.u.GetRandomString(new Random().Next(100,128)),
                            "RESTART_LOGIN",
                            username,
                            password,
                            itsInterface.license.ID,
                            itsInterface.u.GetRandomString(new Random().Next(100, 128))
                        ))},
                    Method.POST);

                itsInterface.license = itsInterface.req.VerifyLicense(r, itsInterface);
            }
            else
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
            }

            if (itsInterface.license.Status)
            {
                itsInterface.isBotStarted = false;
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

                if (method)
                {
                    itsInterface.isBotStarted = true;
                    itsInterface.dashboard.IsStart = false;
                    itsInterface.dashboard.IsStop = false;
                    itsInterface.dashboard.IsRestart = false;
                    itsInterface.processManager.Start(itsInterface);
                }
                else
                {
                    itsInterface.logger.Log(false, itsInterface.messages.WaitingForStart);
                    CHECKSTART:
                    Thread.Sleep(10000);
                    GC.Collect();
                    GC.Collect();
                    if (itsInterface.dashboard.IsStart)
                    {
                        itsInterface.isBotStarted = true;
                        itsInterface.dashboard.IsStart = false;
                        itsInterface.dashboard.IsStop = false;
                        itsInterface.dashboard.IsRestart = false;
                        itsInterface.processManager.Start(itsInterface);
                    }
                    else
                    {
                        goto CHECKSTART;
                    }
                }
                Console.ReadLine();
            }
            else
            {
                itsInterface.logger.Log(false,itsInterface.messages.ErrorLogin);
                Console.ReadLine();
            }


        }

        #region Website API

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

            //Console.WriteLine(DecryptString(botRequest));
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

        public void UpdateLPQStatus(string trueFalse, Interface itsInterface)
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
                        trueFalse
                    ))},
                Method.POST);

            //Console.WriteLine(DecryptString(botRequest));
        }

        #endregion
    }


    public class DashboardActionHelper : IJob
    {
        #region Security
        string[] musicList = ReadAllResourceLines("Evelynn_Bot.Constants.mn.txt");
        public static string[] ReadAllResourceLines(string resourceName)
        {
            using (Stream stream = Assembly.GetEntryAssembly()
                .GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return EnumerateLines(reader).ToArray();
            }
        }

        public static IEnumerable<string> EnumerateLines(TextReader reader)
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }

        #endregion

        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            Interface itsInterface = (Interface)dataMap["InterfaceClass"];

            //ClientKiller.SuspendLeagueClient(); // Her dakikada hide ve suspend eder clienti \\ Gerekli mi bilmiyorum
            
            string r = itsInterface.req.CreateRequest(DashboardHelper.URI,
                new string[] { "data" },
                new string[] { itsInterface.sec.EncryptString(
                    string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}",
                        itsInterface.u.GetRandomString(new Random().Next(100,128)),
                        "ACTION",
                        itsInterface.license.Username,
                        itsInterface.license.Password,
                        itsInterface.license.ID,
                        itsInterface.license.Last,
                        itsInterface.uptime.Millisec.ToString(),
                        itsInterface.isBotStarted ? "Running" : "Idle"
                    ))},
                Method.POST);

            await itsInterface.req.GetActionStatus(r, itsInterface);

            // Change Title
            
            var rand = new Random();
            int randomNumber = rand.Next(0, musicList.Length);

            // Read the random line
            string line = musicList.Skip(randomNumber - 1).Take(1).First();
            Program.SetWindowText(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle, line);
        }
    }

}
