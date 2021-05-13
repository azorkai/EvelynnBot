using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evelynn_Bot.League_API.GameData
{
    public class Summoner
    {
        public long accountId
        {
            get
            {
                return this.long_3;
            }
            set
            {
                this.long_3 = value;
            }
        }

        public string displayName
        {
            get
            {
                return this.string_2;
            }
            set
            {
                this.string_2 = value;
            }
        }

        public string internalName
        {
            get
            {
                return this.string_1;
            }
            set
            {
                this.string_1 = value;
            }
        }

        public int percentCompleteForNextLevel
        {
            get
            {
                return this.int_2;
            }
            set
            {
                this.int_2 = value;
            }
        }

        public int profileIconId
        {
            get
            {
                return this.int_1;
            }
            set
            {
                this.int_1 = value;
            }
        }

        public string puuid
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

        public long summonerId
        {
            get
            {
                return this.long_2;
            }
            set
            {
                this.long_2 = value;
            }
        }

        public int summonerLevel
        {
            get
            {
                return this.int_0;
            }
            set
            {
                this.int_0 = value;
            }
        }

        public long xpSinceLastLevel
        {
            get
            {
                return this.long_1;
            }
            set
            {
                this.long_1 = value;
            }
        }

        public long xpUntilNextLevel
        {
            get
            {
                return this.long_0;
            }
            set
            {
                this.long_0 = value;
            }
        }

        private long long_0;

        private long long_1;

        private int int_0;

        private long long_2;

        private string string_0;

        private int int_1;

        private int int_2;

        private string string_1;

        private string string_2;

        private long long_3;
    }
}
