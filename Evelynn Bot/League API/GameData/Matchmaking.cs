using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evelynn_Bot.League_API.GameData
{
    public struct Matchmaking
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

        public LowPriorityData lowPriorityData { get; set; }

        public struct LowPriorityData
        {
            public string bustedLeaverAccessToken { get; set; }
            public List<object> penalizedSummonerIds { get; set; }
            public double penaltyTime { get; set; }
            public double penaltyTimeRemaining { get; set; }
            public string reason { get; set; }
        }

        public enum SearchStateEnum
        {
            INVALID,
            ABANDONEDLOWPRIORITYQUEUE,
            CANCELED,
            SEARCHING,
            FOUND,
            ERROR,
            SERVICEERROR,
            SERVICESHUTDOWN,
        }
    }
}
