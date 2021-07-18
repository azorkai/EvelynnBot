using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Evelynn_Bot.ExternalCommands
{
    public class Logger
    {

        public void ReportLog(string logtext)
        {
            try
            {
                string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                using (StreamWriter streamWriter = new StreamWriter(directoryName + "\\log.txt", true))
                {
                    streamWriter.WriteLine("[" + DateTime.Now.ToString() + "]" + logtext);
                }
            }
            catch
            {
            }
        }

        public void Log(bool result, string Write)
        {
            if (result)
            {
                DateTime dateTime = DateTime.Now;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Out.WriteAsync(dateTime.ToString("[HH:mm:ss]") + "");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Out.WriteAsync(Write);
                Console.Out.WriteLineAsync("");
            }
            else
            {
                DateTime dateTime = DateTime.Now;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Out.WriteAsync(dateTime.ToString("[HH:mm:ss]") + "");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Out.WriteAsync(Write);
                Console.Out.WriteLineAsync("");
            }
        }
    }
}
