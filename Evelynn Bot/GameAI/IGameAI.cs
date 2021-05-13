using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evelynn_Bot.Entities;
using Evelynn_Bot.Results;

namespace Evelynn_Bot.GameAI
{
    public interface IGameAI:IDisposable
    {
        string[] UseImageSearch(string imgPath, string tolerance);
        IResult ImageSearch(string path, string tolerance, string message);
        void CurrentPlayerStats(Player player);
        void GoMid();
        void GoTop();
        void GoBot();
        void GoBase();
    }
}
