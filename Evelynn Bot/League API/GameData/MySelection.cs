using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evelynn_Bot.League_API.GameData
{
    public class MySelection
    {
        int int_0;

        int int_1;

        int int_2;

        int int_3;

        public int selectedSkinId
        {
            get
            {
                return int_0;
            }
            set
            {
                int_0 = value;
            }
        }

        public int spell1Id
        {
            get
            {
                return int_1;
            }
            set
            {
                int_1 = value;
            }
        }

        public int spell2Id
        {
            get
            {
                return int_2;
            }
            set
            {
                int_2 = value;
            }
        }

        public int wardSkinId
        {
            get
            {
                return int_3;
            }
            set
            {
                int_3 = value;
            }
        }
    }
}
