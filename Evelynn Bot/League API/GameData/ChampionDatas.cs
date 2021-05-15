using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evelynn_Bot.League_API.GameData
{
    public class ChampionDatas
    {
		public static List<string> Spells = new List<string>
		{
			"Ghost",
			"Heal",
			"Barrier",
			"Exhaust",
			"Flash",
			"Teleport",
			"Ignite",
			"Smite"
		};

		public static List<int> ADCChampions = new List<int>
		{
            21,
            202,
            29,
            81,
            51,
            67,
            523,
            236,
            222,
            15,
            145,
            18,
            110,
            22,
            429,
            119,
            498
		};

		// Token: 0x02000044 RID: 68
		public enum LeagueChampions
		{
			Random,
			const_1 = 111111,
			Aatrox = 266,
			Ahri = 103,
			Akali = 84,
			Alistar = 12,
			Amumu = 32,
			Annie = 1,
			Ashe = 22,
			Aphelios = 523,
			Azir = 258,
			Bard = 432,
			Blitzcrank = 53,
			Brand = 63,
			Braum = 201,
			Caitlyn = 51,
			Camille = 164,
			Cassiopeia = 69,
			ChoGath = 31,
			Corki = 42,
			Darius = 122,
			Diana = 131,
			DrMundo = 36,
			Draven = 119,
			Ekko = 245,
			Elise = 60,
			Evelynn = 28,
			Ezreal = 81,
			Fiddlesticks = 9,
			Fiora = 114,
			Fizz = 105,
			Galio = 3,
			Gangplank = 41,
			Garen = 86,
			Gnar = 150,
			Gragas = 79,
			Graves = 104,
			Hecarim = 120,
			Heimerdinger = 74,
			Illaoi = 420,
			Irelia = 39,
			Ivern = 427,
			Janna = 40,
			JarvanIV = 59,
			Jax = 24,
			Jayce = 126,
			Jhin = 202,
			Jinx = 222,
			KaiSa = 145,
			Kalista = 429,
			Karma = 43,
			Karthus = 30,
			Kassadin = 38,
			Katarina = 55,
			Kayle = 10,
			Kayn = 141,
			Kennen = 85,
			KhaZix = 121,
			Kindred = 203,
			Kled = 240,
			KogMaw = 96,
			LeBlanc = 7,
			LeeSin = 64,
			Leona = 89,
			Lissandra = 127,
			Lucian = 236,
			Lulu = 117,
			// Token: 0x04000255 RID: 597
			Lux = 99,
			// Token: 0x04000256 RID: 598
			Malphite = 54,
			// Token: 0x04000257 RID: 599
			Malzahar = 90,
			// Token: 0x04000258 RID: 600
			Maokai = 57,
			// Token: 0x04000259 RID: 601
			MasterYi = 11,
			// Token: 0x0400025A RID: 602
			MissFortune = 21,
			// Token: 0x0400025B RID: 603
			Mordekaiser = 82,
			// Token: 0x0400025C RID: 604
			Morgana = 25,
			// Token: 0x0400025D RID: 605
			Nami = 267,
			// Token: 0x0400025E RID: 606
			Nasus = 75,
			// Token: 0x0400025F RID: 607
			Nautilus = 111,
			// Token: 0x04000260 RID: 608
			Neeko = 518,
			// Token: 0x04000261 RID: 609
			Nidalee = 76,
			// Token: 0x04000262 RID: 610
			Nocturne = 56,
			// Token: 0x04000263 RID: 611
			NunuAndWillump = 20,
			// Token: 0x04000264 RID: 612
			Olaf = 2,
			// Token: 0x04000265 RID: 613
			Orianna = 61,
			// Token: 0x04000266 RID: 614
			Ornn = 516,
			// Token: 0x04000267 RID: 615
			Pantheon = 80,
			// Token: 0x04000268 RID: 616
			Poppy = 78,
			// Token: 0x04000269 RID: 617
			Pyke = 555,
			// Token: 0x0400026A RID: 618
			Qiyana = 246,
			// Token: 0x0400026B RID: 619
			Quinn = 136,
			// Token: 0x0400026C RID: 620
			Rakan = 497,
			// Token: 0x0400026D RID: 621
			Rammus = 33,
			// Token: 0x0400026E RID: 622
			RekSai = 421,
			// Token: 0x0400026F RID: 623
			Renekton = 58,
			// Token: 0x04000270 RID: 624
			Rengar = 107,
			// Token: 0x04000271 RID: 625
			Riven = 92,
			// Token: 0x04000272 RID: 626
			Ryze = 13,
			// Token: 0x04000273 RID: 627
			Sejuani = 113,
			// Token: 0x04000274 RID: 628
			Shaco = 35,
			// Token: 0x04000275 RID: 629
			Senna = 235,
			// Token: 0x04000276 RID: 630
			Sett = 875,
			// Token: 0x04000277 RID: 631
			Shen = 98,
			// Token: 0x04000278 RID: 632
			Shyvana = 102,
			// Token: 0x04000279 RID: 633
			Singed = 27,
			// Token: 0x0400027A RID: 634
			Sion = 14,
			// Token: 0x0400027B RID: 635
			Sivir,
			// Token: 0x0400027C RID: 636
			Skarner = 72,
			// Token: 0x0400027D RID: 637
			Sona = 37,
			// Token: 0x0400027E RID: 638
			Soraka = 16,
			// Token: 0x0400027F RID: 639
			Swain = 50,
			// Token: 0x04000280 RID: 640
			Sylas = 517,
			// Token: 0x04000281 RID: 641
			Syndra = 134,
			// Token: 0x04000282 RID: 642
			TahmKench = 223,
			// Token: 0x04000283 RID: 643
			Taliyah = 163,
			// Token: 0x04000284 RID: 644
			Talon = 91,
			// Token: 0x04000285 RID: 645
			Taric = 44,
			// Token: 0x04000286 RID: 646
			Teemo = 17,
			// Token: 0x04000287 RID: 647
			Thresh = 412,
			// Token: 0x04000288 RID: 648
			Tristana = 18,
			// Token: 0x04000289 RID: 649
			Trundle = 48,
			// Token: 0x0400028A RID: 650
			Tryndamere = 23,
			// Token: 0x0400028B RID: 651
			TwistedFate = 4,
			// Token: 0x0400028C RID: 652
			Twitch = 29,
			// Token: 0x0400028D RID: 653
			Udyr = 77,
			Urgot = 6,
			Varus = 110,
			Vayne = 67,
			Veigar = 45,
			VelKoz = 161,
			Vi = 254,
			Viktor = 112,
			Vladimir = 8,
			Volibear = 106,
			Warwick = 19,
			Wukong = 62,
			Xayah = 498,
			Xerath = 101,
			XinZhao = 5,
			Yasuo = 157,
			Yorick = 83,
			Yuumi = 350,
			Zac = 154,
			Zed = 238,
			Ziggs = 115,
			Zilean = 26,
			Zoe = 142,
			Zyra
		}
	}
}
