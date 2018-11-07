using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.GameClasses.Match.Config
{
	static class AiConfig
	{
    
	    // AI Behavior - default
		public const float RADIUS_AGGRESSION = 1500;
		public const float DISTANCE_HUNTING = 600;
		public static Vector3 DISTANCE_WAYPOINT_REACHED = new Vector3(15, 30, 10000);
	    public const float DISTANCE_HIT_TARGET = 40;
	    public const bool IS_AGGRESSIVE = true;
		public const bool DOES_HIT = true;
		public const bool DOES_SHOOT = true;
		public const bool DOES_HUNT_TREASURES = true;


		// AI Navigation values
	    public const float DISTANCE_JUMP_MIN_HOR = 120;
		public const float DISTANCE_JUMP_MAX_HOR = 300;
	    public const float DISTANCE_JUMP_MIN_HOR_ACT = 80;
	    public const float DISTANCE_JUMP_MAX_HOR_ACT = 180;
		public const float DISTANCE_JUMP_VERT = 140;
		public const float DISTANCE_VALID_OVERLAP = 40;
	    public const float DISTANCE_OFFS_DROP_HOR = 25;
	    public const float DISTANCE_OFFS_DROP_COLL_VERT = 100;
	    public const float DISTANCE_OFFS_DROP_COLL = 5;
		public const float DISTANCE_WAYPOINT_UNREACHABLE_VERT = 160;
		public const float DISTANCE_WAYPOINT_UNREACHABLE_HOR = 50;
		public const float DISTANCE_VERT_LIMIT_CLIMB_HOR = 50;
		public const float DISTANCE_OVERSHOOT_UP = 25;
		public const float DISTANCE_OVERSHOOT_HOR = 25;

		public const float SPEED_JUMP_MIN = 100;

		public const long TIME_UNREACHABLE = 400;
	    public const long TIME_DELTA_SAFE_QUERY = 100;
	    public const long TIME_LIFESPAN_WAYPOINT_MAX = 5000;
		public const long TIME_STICK_WITH_EMPTY_TARGET = 15000;






	}
}
