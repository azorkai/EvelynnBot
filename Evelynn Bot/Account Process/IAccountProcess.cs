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
        bool LoginAccount(Interface itsInterface);
        bool Initialize(Interface itsInterface);
        bool GetSetWallet(Interface itsInterface);
        bool CheckNewAccount(Interface itsInterface);
        string RandomName(int len, bool two);
        string RandomNameGenerator();
        bool TutorialMissions(Interface itsInterface);
        bool PatchCheck(Interface itsInterface);
        bool LeagueIsPatchAvailable(Interface itsInterface);
        //bool Disenchant(Interface itsInterface);
        //void CreateGame(Interface itsInterface);
        //bool StartQueue(Interface itsInterface);
        //bool SetSpell(Interface itsInterface);
        //bool PickRandomAvailableChampion(Interface itsInterface);
        //int[] GetPickableChampions(Interface itsInterface);
        bool SelectChampion(Interface itsInterface);
        HttpRequest CreateRequest(Interface itsInterface);
    }
}
