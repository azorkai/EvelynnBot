using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evelynn_Bot.Results;

namespace Evelynn_Bot.ExternalCommands
{
    public interface IHelper:IDisposable
    {
        IResult KillLeagueProcess();
    }
}
