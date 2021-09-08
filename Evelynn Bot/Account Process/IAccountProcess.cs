using Evelynn_Bot.Entities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bAUTH;
using Evelynn_Bot.Constants;
using Evelynn_Bot.ExternalCommands;
using Evelynn_Bot.GameAI;
using Evelynn_Bot.League_API.GameData;
using Leaf.xNet;

namespace Evelynn_Bot.Account_Process
{
    public interface IAccountProcess:IDisposable
    {
        bool StartLeague(Interface itsInterface);
        Task<bool> LoginAccount(Interface itsInterface);
        bool Initialize(Interface itsInterface);
        Task<bool> GetSetWallet(Interface itsInterface);
        Task<Task> CheckNewAccount(Interface itsInterface);
        string RandomName(int len, bool two);
        string RandomNameGenerator();
        bool PatchCheck(Interface itsInterface);
        bool LeagueIsPatchAvailable(Interface itsInterface);
        Task<string> VerifySession(Interface itsInterface);
        Task<Task> ChangeRegion(Interface itsInterface);
        Task<Task> OldClientLoginAccount(Interface itsInterface);
        Task<Task> CheckLeagueBan(Interface itsInterface);
    }
}
