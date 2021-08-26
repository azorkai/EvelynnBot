using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using AutoIt;
using bAUTH;
using Evelynn_Bot.Account_Process;
using Evelynn_Bot.Constants;
using Evelynn_Bot.ExternalCommands;
using EvelynnLCU;
using EvelynnLCU.Plugins.LoL;
using Newtonsoft.Json;
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
            get
            {
                return GetConsoleWindow();
            }
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

        static void Main(string[] args)
        {
            //UpdateBot.CheckUpdate();
            
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

            Interface itsInterface = new Interface();
            bool a;
            a = true;
            while (a)
            {
                Thread.Sleep(2000);
                if (itsInterface.gameAi.AllyMinionCheck(itsInterface))
                {
<<<<<<< HEAD
                    Console.WriteLine("Minyon");
                    //AutoItX.MouseClick("RIGHT", itsInterface.gameAi.X, itsInterface.gameAi.Y, 1, 1);
                }
                else
                {
                    Console.WriteLine("No Minion");
=======
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
>>>>>>> a9894b9e6cb5e3ae3c1d4ed95e1c65f0bfcedc25
                }
            }
            //Language.language = itsInterface.jsonRead.Language();
            //itsInterface.messages.SetLanguage();
            //string botArg = "";
            //try { botArg = args[0]; } catch { }
            //if (!String.IsNullOrEmpty(botArg))
            //{
            //    try
            //    {
            //        var jsonStr = Encoding.UTF8.GetString(Convert.FromBase64String(args[0]));
            //        itsInterface.license = JsonConvert.DeserializeObject<License>(jsonStr);
            //        if (itsInterface.license.Status && !String.IsNullOrEmpty(itsInterface.license.Username) && !String.IsNullOrEmpty(itsInterface.license.Password))
            //        {
            //            itsInterface.dashboardHelper.LoginAndStartBot(itsInterface.license.Username, itsInterface.license.Password, itsInterface, true);
            //        }
            //        else
            //        {
            //            Environment.Exit(0);
            //        }
            //    }
            //    catch { Environment.Exit(0); }
            //}
            //else
            //{
            //    itsInterface.dashboardHelper.LoginAndStartBot(itsInterface.jsonRead.Id(), itsInterface.jsonRead.Password(), itsInterface);
            //}

            Console.ReadLine();
        }

    }
    public static class Language
    {
        public static string language;
    }
}
