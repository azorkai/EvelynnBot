using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evelynn_Bot.Constants;
using Evelynn_Bot.Entities;
using Evelynn_Bot.ExternalCommands;

namespace Evelynn_Bot.GameAI
{
    public interface IGameAI:IDisposable
    {
        string[] UseImageSearch(string imgPath, string tolerance);
        bool ImageSearch(string path, string tolerance, string message, Interface itInterface);
        void CurrentPlayerStats(Interface itInterface);
        Task Combo(int x, int y);
        Task GoSafeArea();
        Task GoMid();
        void GoTop();
        void GoBot();
        Task GoBase();
    }
}
