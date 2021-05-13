using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Evelynn_Bot.League_API.GameData
{
    public class Missions
    {
        public string id { get; set; }

        public string missionType { get; set; }

        public string[] requirements { get; set; }

        public MissionRewards[] rewards { get; set; }

        public string status { get; set; }


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [CompilerGenerated]
        private string string_0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [CompilerGenerated]
        private string string_1;

        [CompilerGenerated]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string[] string_2;

        [CompilerGenerated]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MissionRewards[] pkOaDabVhk;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [CompilerGenerated]
        private string string_3;

    }
}
