using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.GameClasses.Match.Config;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.GameClasses.Match.MatchObjects.Ai
{
    class AiBehavior
    {
        public AiBehavior()
        {
            RadiusAgression = AiConfig.RADIUS_AGGRESSION;
            DistanceHunting = AiConfig.DISTANCE_HUNTING;
            IsAggressive = AiConfig.IS_AGGRESSIVE;
            DoesHit = AiConfig.DOES_HIT;
            DoesShoot = AiConfig.DOES_SHOOT;
            HuntsTreasure = AiConfig.DOES_HUNT_TREASURES;
            WaypointReachedDistance = AiConfig.DISTANCE_WAYPOINT_REACHED;
        }

        public AiBehavior(float radiusAgression, float distanceHunting, bool isAggressive, bool doesHit, bool doesShoot, bool huntsTreasure, Vector3 waypointReachedDistance)
        {
            RadiusAgression = radiusAgression;
            DistanceHunting = distanceHunting;
            IsAggressive = isAggressive;
            DoesHit = doesHit;
            DoesShoot = doesShoot;
            HuntsTreasure = huntsTreasure;
            WaypointReachedDistance = waypointReachedDistance;
        }

        public float RadiusAgression { get; set; }
        public float DistanceHunting { get; set; }
        public bool IsAggressive { get; set; }
        public bool DoesHit { get; set; }
        public bool DoesShoot { get; set; }
        public bool HuntsTreasure { get; set; }
        public Vector3 WaypointReachedDistance { get; set; }


    }
}
