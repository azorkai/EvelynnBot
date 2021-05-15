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

        public int level { get; set; }

    }
}
