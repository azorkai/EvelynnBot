using Evelynn_Bot.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Evelynn_Bot.Constants;

namespace Evelynn_Bot.ExternalCommands
{
    public class Helper : IHelper
    {

        public IResult KillLeagueProcess()
        {
            try
            {
                System.Diagnostics.Process[] processesByName = System.Diagnostics.Process.GetProcessesByName("RiotClientUx");
                foreach (System.Diagnostics.Process process in processesByName)
                {
                    process.Kill();
                }
                Thread.Sleep(5000);
                System.Diagnostics.Process[] processesByName2 = System.Diagnostics.Process.GetProcessesByName("LeagueClient");
                foreach (System.Diagnostics.Process process2 in processesByName2)
                {
                    process2.Kill();
                }
                Thread.Sleep(5000);
                System.Diagnostics.Process[] processesByName3 = System.Diagnostics.Process.GetProcessesByName("League of Legends");
                foreach (System.Diagnostics.Process process3 in processesByName3)
                {
                    process3.Kill();
                }
                Thread.Sleep(5000);
                System.Diagnostics.Process[] processesByName4 = System.Diagnostics.Process.GetProcessesByName("RiotClientServices");
                foreach (System.Diagnostics.Process process4 in processesByName4)
                {
                    process4.Kill();
                }
            }
            catch
            {
                return new Result(false, Messages.ErrorKillLeagueProcess);
            }

            return new Result(true, Messages.SuccessCreateGame);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~Helper()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
