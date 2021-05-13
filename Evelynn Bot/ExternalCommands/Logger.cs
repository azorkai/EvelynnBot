using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evelynn_Bot.ExternalCommands
{
    public class Logger
    {

        public static void Log(bool result, string Write)
        {
            if (result)
            {
                DateTime dateTime = DateTime.Now;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(dateTime.ToString("[HH:mm:ss]") + "");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(Write);
                Console.WriteLine("");
            }
            else
            {
                DateTime dateTime = DateTime.Now;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(dateTime.ToString("[HH:mm:ss]") + "");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(Write);
                Console.WriteLine("");
            }
        }
    }
}
