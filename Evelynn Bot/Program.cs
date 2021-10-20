using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using bAUTH;
using Evelynn_Bot.Account_Process;
using Evelynn_Bot.Constants;
using Evelynn_Bot.ExternalCommands;
using EvelynnLCU;
using EvelynnLCU.Plugins.LoL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Console = System.Console;
using License = Evelynn_Bot.Entities.License;

namespace Evelynn_Bot
{
    class Program
    {
        #region WIN32 API

        public static void SetWindowPosition(int x, int y, int width, int height)
        {
            SetWindowPos(Handle, IntPtr.Zero, x, y, width, height, SWP_NOZORDER | SWP_NOACTIVATE);
        }

        public static IntPtr Handle
        {
            get { return GetConsoleWindow(); }
        }

        const int SWP_NOZORDER = 0x4;
        const int SWP_NOACTIVATE = 0x10;

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern void SetWindowText(IntPtr hWnd, string windowName);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int x, int y, int cx, int cy, int flags);

        #endregion

        private static bool CheckInternet()
        {
            Thread.Sleep(3000);
            try
            {
                Ping myPing = new Ping();
                String host = "google.com";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void ExceptionHandler(object sender, UnhandledExceptionEventArgs args, Interface itsInterface)
        {
            Exception e = (Exception)args.ExceptionObject;
            itsInterface.logger.ReportLog($"Error: {e.Message} | Source: {e.Source} | ST: {e.StackTrace}");
            itsInterface.clientKiller.KillAllLeague();
            itsInterface.logger.Log(false, "Unhandled Error! Restarting...");
            Thread.Sleep(5000);
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

        private static bool ExitHandler(WIN32.CtrlTypes cEnum)
        {
            Process.Start("C:\\Windows\\explorer.exe");
            Console.WriteLine("Good Bye!");
            Thread.Sleep(2500);
            return true;
        }


        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        static async Task Main(string[] args)
        {
            Interface itsInterface = new Interface();
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler((sender, e) => ExceptionHandler(sender, e, itsInterface));
            WIN32.ControlDelegate gControlDelegate = new WIN32.ControlDelegate(ExitHandler);
            WIN32.SetConsoleCtrlHandler(gControlDelegate, true);
            WIN32.SetWindowPos(Process.GetCurrentProcess().MainWindowHandle, 0, 0, 0, 0, 0, 1u | 4u);

            try
            {
                await itsInterface.clientKiller.ExecuteBypass();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Thread.Sleep(8000);

            while (!CheckInternet())
            {
                Thread.Sleep(3500);
                itsInterface.logger.Log(false, "No Internet!");
                try
                {
                    await itsInterface.clientKiller.ExecuteBypass();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            UpdateBot.CheckUpdate();

            itsInterface.logger.Log(true, "Version: " + Assembly.GetExecutingAssembly().GetName().Version);

            #region Resize Console

            //Console.WindowWidth = 80;
            //Console.WindowHeight = 15;
            //Console.BufferWidth = 80;
            //Console.BufferHeight = 15;
            //var screen = Screen.PrimaryScreen.Bounds;
            //var height = screen.Height;
            //var ustTaraf = height - (height - 30);
            ////SetWindowPosition(5, ustTaraf, 700, 200);

            #endregion

            //GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            //GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency; //Kaldirildi

            Language.language = itsInterface.jsonRead.Language();
            itsInterface.messages.SetLanguage();
            string botArg = "";
            try { botArg = args[0]; } catch { }
            if (!String.IsNullOrEmpty(botArg))
            {
                try
                {
                    var jsonStr = Encoding.UTF8.GetString(Convert.FromBase64String(args[0]));
                    itsInterface.license = JsonConvert.DeserializeObject<License>(jsonStr);
                    if (itsInterface.license.Status && !String.IsNullOrEmpty(itsInterface.license.Username) && !String.IsNullOrEmpty(itsInterface.license.Password) && !String.IsNullOrEmpty(itsInterface.license.Last))
                    {
                        itsInterface.dashboardHelper.LoginAndStartBot(itsInterface.license.Username, itsInterface.license.Password, itsInterface, true);
                    }
                    else
                    {
                        Environment.Exit(0);
                    }
                }
                catch { Environment.Exit(0); }
            }
            else
            {
                itsInterface.dashboardHelper.LoginAndStartBot(itsInterface.jsonRead.Id(), itsInterface.jsonRead.Password(), itsInterface);
            }

            GC.KeepAlive(gControlDelegate);
            Console.ReadLine();
        }

    }


    public static class Language
    {
        public static string language;
    }
}

