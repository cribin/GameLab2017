using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonizingBastards.GameClasses.Match.Config
{
	static class MatchConfig
	{

		// Fill with general settings for this logic/screen (this match)


		// WARNING: The sum of these factors may not be very close to 1 or even bigger.

		// Define what the ratio is of foliage containing treasure vs. empty foliage
		public static float TreasureFactor = 0.15f;
		// Define what the ratio is of foliage containing ammo crates vs. empty foliage
		public static float AmmoCrateFactor = 0.1f;
		// Define what the ratio is of foliage containing indigenous vs. empty foliage
		public static float IndigenousFactor = 0.05f; // FOR ALPHA RELEASE
        // Define what the ratio is of foliage containing snake vs. empty foliage
	    public static float SnakeSpawnFactor = 0.05f;

        //Determines how many treasures are spawned depending on the number of players
	    public static int TreasuresPerPlayer = 3;
		// Same for indigeneous people
		public static float IndPeoplePerPlayer = 0.5f;


		// The parameter used in the probability calculation to determine treasure locations.
		// Indirectly define the mean and variance used in the distribution applied.
		// These factors get multiplied with the count of total foliage in the level to calculate mean and variance respectively.
		public static double TreasureDistanceProbabilityMeanFactor = 0.675;
		public static double TreasureDistanceProbabilityVarianceFactor = 0.3;

		// Hud
		// The ratio which defines how much space the left and right border take for the score hud
		public static float BorderRatio = 1f / 6;
		// The distance from top
		public static int TopSpacing = 20;
		public static int TotalWidth = 1920;

		// Defines how many bullets a player gets when he picks up an ammo crate
		public static uint AmmoCrateContentSize = 3;

	}
}
