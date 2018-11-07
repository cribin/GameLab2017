using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonizingBastards.GameClasses.Config
{
	static class MainConfig
	{
		// Content directories
		public static string CONTENT_DIRECTORY = "Content/";
		public static string CONTENT_ASSET_DIRECTORY = CONTENT_DIRECTORY + "assets/";
		public static string CONTENT_MAP_DIRECTORY = CONTENT_ASSET_DIRECTORY + "maps/";
		public static string CONTENT_GRAPHICS_DIRECTORY = CONTENT_ASSET_DIRECTORY + "graphics/";
		public static string CONTENT_CHARACTERS_DIRECTORY = CONTENT_ASSET_DIRECTORY + "characters/";
		public static string CONTENT_FONTS_DIRECTORY = CONTENT_ASSET_DIRECTORY + "fonts/";

		// Content Pipeline directories
		public static string PIPELINE_ASSET_DIRECTORY = "assets/";
		public static string PIPELINE_GRAPHICS_DIRECTORY = PIPELINE_ASSET_DIRECTORY + "graphics/";
		public static string PIPELINE_FONTS_DIRECTORY = PIPELINE_ASSET_DIRECTORY + "fonts/";
		public static string PIPELINE_SOUNDS_DIRECTORY = PIPELINE_ASSET_DIRECTORY + "sounds/";
		public static string PIPELINE_SONGS_DIRECTORY = PIPELINE_ASSET_DIRECTORY + "songs/";

		// Character files
		public static string CONTENT_CHARACTER_FILE_SPRITESHEET(int playerId)
		{
			return "Player" + playerId;
		}
		public static string CONTENT_CHARACTER_FILE_XML(int playerId)
		{
			return "Player" + playerId + ".xml";
		}

	    public static string CONTENT_SHOPKEEPER_FILE = "Shopkeeper.xml";
	}
}
