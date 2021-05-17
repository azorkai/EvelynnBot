using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using AutoIt;
using bAUTH;
using Evelynn_Bot.Account_Process;
using Evelynn_Bot.Constants;
using Evelynn_Bot.Entities;
using Evelynn_Bot.ExternalCommands;
using Evelynn_Bot.GameAI;
using Evelynn_Bot.League_API;
using Evelynn_Bot.League_API.GameData;
using Console = System.Console;

namespace Evelynn_Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            JsonRead jsonRead = new JsonRead();
            DashboardHelper.LoginAndStartBot(jsonRead.Id(), jsonRead.Password());
            Console.ReadLine();
        }

    }
}
