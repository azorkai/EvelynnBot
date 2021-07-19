using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using AutoIt;
using bAUTH;
using Evelynn_Bot.Constants;
using Console = System.Console;

namespace Evelynn_Bot
{
    class Program
    {

        static void Main(string[] args)
        {
            //GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            //GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency; //Kaldirildi
            Interface itsInterface = new Interface();
            Language.language = itsInterface.jsonRead.Language();
            itsInterface.messages.SetLanguage();
            itsInterface.dashboardHelper.LoginAndStartBot(itsInterface.jsonRead.Id(), itsInterface.jsonRead.Password(), itsInterface);
            Console.ReadLine();
        }

    }
    public static class Language
    {
        public static string language;
    }
}
