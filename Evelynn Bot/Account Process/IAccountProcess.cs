using Evelynn_Bot.Entities;
using Evelynn_Bot.Results;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leaf.xNet;

namespace Evelynn_Bot.Account_Process
{
    public interface IAccountProcess:IDisposable
    {
        IResult StartLeague(License license);
        IResult LoginAccount(License license);
        IResult Initialize();
        void GetSetWallet();
        void CheckNewAccount(License license);
        string RandomName(int len, bool two);
        string RandomNameGenerator();
        void TutorialMissions(License license);
        void PatchCheck();
        bool LeagueIsPatchAvailable();
        void Disenchant();
        IResult CreateGame(License license);
        IResult StartQueue(License license);
        IResult SetSpell();
        IResult PickRandomAvailableChampion();
        int[] GetPickableChampions();
        void SelectChampion();
        HttpRequest CreateRequest();
    }
}
