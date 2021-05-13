using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Evelynn_Bot.Entities
{
    public class License
    {
        public bool Status { get; set; }

        public string ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Register { get; set; }
        public string Expire { get; set; }
        public string Last { get; set; }
        public string Lol_username { get; set; }
        public string Lol_password { get; set; }
        public int Lol_maxLevel { get; set; }
        public int Lol_maxBlueEssences { get; set; }
        public bool Lol_disenchant { get; set; }
        public bool Lol_doTutorial { get; set; }
        public bool Lol_isEmptyNick { get; set; }
        public string LeaguePath { get; set; }
    }
}
