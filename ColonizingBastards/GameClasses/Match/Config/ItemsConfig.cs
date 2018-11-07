using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.GameClasses.Match.Config
{
    /// <summary>
    /// This class hold all the relevant information for items like treasure, loot, trap etc.
    /// </summary>
    public static class ItemsConfig
    {
        public static List<Texture2D> LOOT_TEXTURE { get; set; }

        public static Texture2D AMMO_CRATE_TEXTURE { get; set; }

        public static List<Texture2D> AMMO_COUNT_TEXTURES { get; set; } = new List<Texture2D>();

		public static Texture2D TRAP_OPEN_TEXTURE { get; set; }

		public static Texture2D TRAP_CLOSED_TEXTURE { get; set; }

		public const float TRAP_STUN_TIME = 5;//in seconds
	}
}
