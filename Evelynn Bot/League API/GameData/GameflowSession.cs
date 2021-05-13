using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evelynn_Bot.League_API.GameData
{
    public class GameflowSession
    {
        public string phase
        {
            get
            {
                return this.string_0;
            }
            set
            {
                this.string_0 = value;
            }
        }

        private string string_0;

        public enum GameflowSessionEnum
        {
            NONE,
            LOBBY,
            MATCHMAKING,
            CHECKEDINTOTOURNAMENT,
            READYCHECK,
            CHAMPSELECT,
            GAMESTART,
            FAILEDTOLAUNCH,
            INPROGRESS,
            RECONNECT,
            WAITINGFORSTATS,
            PREENDOFGAME,
            ENDOFGAME,
            TERMINATEDINERROR
        }
    }
}
