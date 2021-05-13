using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evelynn_Bot.League_API.GameData
{
    public class ChampionSelectInformation
    {
		public string type
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

		public bool isAllyAction
		{
			get
			{
				return this.bool_1;
			}
			set
			{
				this.bool_1 = value;
			}
		}

		public int id
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

		public bool completed
		{
			get
			{
				return this.bool_0;
			}
			set
			{
				this.bool_0 = value;
			}
		}

		public int championId
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

		public int actorCellId
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

		public bool isInProgress
		{
			get
			{
				return this.bool_2;
			}
			set
			{
				this.bool_2 = value;
			}
		}
        public int pickTurn
		{
			get
			{
				return this.int_3;
			}
			set
			{
				this.int_3 = value;
			}
		}

		private int int_0;

		private int int_1;

		private bool bool_0;

		private int int_2;

		private bool bool_1;

		private string string_0;

		private bool bool_2;

		private int int_3;
	}
}
