﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Evelynn_Bot.League_API.GameData
{
    public struct Missions
    {
        public string id { get; set; }

        public string missionType { get; set; }

        public string[] requirements { get; set; }

        public MissionRewards[] rewards { get; set; }

        public string status { get; set; }

    }
}
