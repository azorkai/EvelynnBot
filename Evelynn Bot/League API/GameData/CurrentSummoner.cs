using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Evelynn_Bot.League_API.GameData
{
    public class CurrentSummoner
    {
        public ChampionStats championStats { get; set; }

        public double currentGold { get; set; }

        public string summonerName { get; set; }

        [CompilerGenerated]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ChampionStats championStats_0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [CompilerGenerated]
        private double double_0;

        [CompilerGenerated]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string string_0;
    }
}
