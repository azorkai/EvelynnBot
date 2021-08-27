using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evelynn_Bot.Entities
{
    public struct Player
    {
        public float MaxHealth { get; set; }
        public float CurrentHealth { get; set; }
        public double CurrentGold { get; set; }
        public int Level { get; set; }
        public long Level_Q { get; set; }
        public long Level_W { get; set; }
        public long Level_E { get; set; }
        public long Level_R { get; set; }
        public EvelynnLCU.API_Models.InGameData Data { get; set; }
        public string CurrentGame_ChampName { get; set; }

    }
}
