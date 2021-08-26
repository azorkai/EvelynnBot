using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evelynn_Bot.Constants
{
    public class ImagePaths
    {
        public string enemy_health = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.RelativeSearchPath ?? "Image\\enemy_health.png");
        public string enemy_minions = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.RelativeSearchPath ?? "Image\\enemy_minions.png");
        public string game_started = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.RelativeSearchPath ?? "Image\\game_started.png");
        public string minions = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.RelativeSearchPath ?? "Image\\minions.png");
        public string minions_tutorial = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.RelativeSearchPath ?? "Image\\minions_tutorial.png");
        public string shop = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.RelativeSearchPath ?? "Image\\shop.png");
        public string tower = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.RelativeSearchPath ?? "Image\\tower.png");
        public string tower2 = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.RelativeSearchPath ?? "Image\\tower2.png");
        public string game_started_tutorial = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.RelativeSearchPath ?? "Image\\game_started_tutorial.png");

        public Color AllyMinionColor = Color.FromArgb(44, 89, 119);
        public Color AllyMinionColor2 = Color.FromArgb(76, 144, 204);
        public Color AllyMinionColor3 = Color.FromArgb(80, 128, 180);
        public Color AllyMinionColor4 = Color.FromArgb(8, 12, 16);
        public Color EnemyMinionColor = Color.FromArgb(119, 56, 54);
        public Color TowerColor = Color.FromArgb(202, 52, 44);
        public Color EnemyColor = Color.FromArgb(48, 3, 0);
    }
}
