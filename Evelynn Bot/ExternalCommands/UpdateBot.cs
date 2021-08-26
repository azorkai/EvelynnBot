using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoUpdaterDotNET;

namespace Evelynn_Bot.ExternalCommands
{
    public class UpdateBot
    {
        public static void CheckUpdate()
        {
            AutoUpdater.Mandatory = true;
            AutoUpdater.UpdateMode = Mode.ForcedDownload;
            AutoUpdater.Start("https://ytdtoken.space/update.xml");
        }
    }
}
