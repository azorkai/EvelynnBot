using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evelynn_Bot.League_API.GameData
{
    public class Matchmaking
    {
        public string searchState
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

        public enum SearchStateEnum
        {
            INVALID,
            ABANDONEDLOWPRIORITYQUEUE,
            CANCELED,
            SEARCHING,
            FOUND,
            ERROR,
            SERVICEERROR,
            SERVICESHUTDOWN
        }
    }
}
