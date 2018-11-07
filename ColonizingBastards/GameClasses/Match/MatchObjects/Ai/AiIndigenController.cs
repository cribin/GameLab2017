using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using ColonizingBastards.Base.MiscUtil.Collidables;
using ColonizingBastards.Base.Objects;
using ColonizingBastards.Base.PlayerInteraction;
using ColonizingBastards.Base.Scene;
using ColonizingBastards.GameClasses.Config;
using ColonizingBastards.GameClasses.Match.Config;
using ColonizingBastards.GameClasses.Match.MatchObjects.Characters;
using ColonizingBastards.GameClasses.Match.MatchObjects.Characters.Skills;
using ColonizingBastards.GameClasses.Match.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.GameClasses.Match.MatchObjects.Ai
{
    /// <summary>
    /// Controller for 1 (!) possessed character
    /// </summary>
    class AiIndigenController : AiController
    {
        public AiIndigenController(Scene scene, NavGraph navGraph, AiBehavior behavior) : base(scene, navGraph)
        {
            this.Behavior = behavior;
	        this.random = scene.Random;
            this.Path = new List<Waypoint>();
        }

	    private Random random;

        public AiBehavior Behavior { get; set; }
        public DefaultCharacter Target { get; set; }
        public List<Waypoint> Path { get; set; }
	    private Vector3 lastCenterPos = new Vector3(0);
        private long lastWaypointChange = 0;
        private Collidable lastPlatform = null;
        private Collidable lastPlatformTarget = null;
        private long lastSuccQueryTarget = 0;

	    private long timeTargetAcquired = 0;

        private SkillStunPlayer stunSkill;

        protected override ActionSet Simulate(GameTime gameTime)
        {
            ActionSet actions = new ActionSet();


            if (Character() == null)
                return actions;

            if (stunSkill == null)
            {
                this.stunSkill = new SkillStunPlayer(scene, Character(), Character().CharacterPhysics, CharacterConfig.SKILL_COOLDOWN_CUT);
            }
            
            // First, simulate the movement of the character
            SimulateMovement(actions);

            // Check if within Action-Distance to Target (react if necessary)
            if (!Character().CharacterPhysics.IsStunned && Target != null)
            {
                if (DistanceToTarget() < AiConfig.DISTANCE_HIT_TARGET)
                {
                    bool hasTreasure = Target.InvTreasure != null;
                    stunSkill.Execute((long) gameTime.TotalGameTime.TotalMilliseconds, Target);
                    if (Target.CharacterPhysics.IsStunned)
                    {
                        if (hasTreasure)
                        {
                            DespawnAi();
                        }
                        else
                        {
                            Target = FindTarget();
                        }
                    }
                }
            }

            return actions;
        }      

        private void SimulateMovement(ActionSet actions)
        {
            // First, check if this character is aggressive
            if (Behavior.IsAggressive)
            {
                Vector3 thisPos = Character().GetPosition();
                // Check if there is a target already and if it's still huntable
	            if (!Character().CharacterPhysics.IsInAir)
	            {
		            if (Target != null && !Target.CharacterPhysics.IsStunned && ((Target.GetPosition() - thisPos).Length() <= Behavior.DistanceHunting))
		            {
			            // Target still valid, hunt it
		                if (Target.InvTreasure == null && (Environment.TickCount - timeTargetAcquired) > AiConfig.TIME_STICK_WITH_EMPTY_TARGET)
		                {
		                    Target = FindTarget();
		                }
			            WalkToTarget();
		            }
		            else
		            {
			            // No valid target
			            Target = FindTarget();
			            // Search for a new one (within aggression radius)
			            

			            if (Target != null)
			            {
				            // Hunt the new target
				            Target = Target;
				            WalkToTarget();
			            }
			            else
			            {
				            // No target in range (walk randomly)
				            //WalkRandom();
			            }
		            }
	            }
            }
            else
            {
                // Walk randomly
                //WalkRandom();
            }


			// Waypoint in path is set (if necessary) 
			// Check the current waypoint (reached?)
			while (Path.Count > 0 && WithinV3Distance(Path.First().Position - CharacterPos(), Behavior.WaypointReachedDistance) /*&&
                !Character().CharacterPhysics.IsClimbing*/)
			{
				Path.RemoveAt(0);
			    lastWaypointChange = System.Environment.TickCount;
			}

			// Resolve being stuck in climbing, while target changes platform
	        if (Target != null && Path.Count > 0 &&
	            (SafeQueryTargetPlatform() != Path.Last().RefPlatform))
	        {
		        Waypoint n = Path.First();
				Path.Clear();
	            //Path.Add(n);
	            //lastWaypointChange = System.Environment.TickCount;
	        }


			bool isClimbing = Character().CharacterPhysics.IsClimbing;
	        bool notMoved = (lastCenterPos - Character().GetCenterPosition()).Length() < 1;

			Vector3 pos = CharacterPos();
			if (NextWaypointUnreachable())
	        {
		        Path.Clear();
	            lastWaypointChange = System.Environment.TickCount;
            } else if (Path.Count >= 2 && isClimbing && pos.Y <= Path[0].Position.Y
				&& pos.Y > Path[1].Position.Y)
			{
				Path.RemoveAt(0);
				lastWaypointChange = System.Environment.TickCount;
			} else if (Path.Count >= 1 && isClimbing && notMoved)
			{
                // Stuck during climbing -> drop
			    actions.actions.Add(InputConfig.Actions.JUMP);
            }



            Vector2 movement;
			// Compute the movement action
			if (Path.Count > 0)
			{
				Vector2 dir = DirectionToWaypoint(Path.First());
				movement = new Vector2(dir.X, dir.Y);
			    movement.Normalize();
                // Flip because of the Windows coordinate system
			    movement.Y = -movement.Y;

				if (Character().CharacterPhysics.IsClimbing &&
				    dir.X < AiConfig.DISTANCE_WAYPOINT_REACHED.X
					&& Math.Abs(dir.Y) > AiConfig.DISTANCE_JUMP_VERT)
				{
					movement.X = 0;
				}

				float horDiff = Path.First().Position.X - CharacterPos().X;
				if (!isClimbing && !Character().CharacterPhysics.IsInAir 
					&& Math.Abs(horDiff) >= AiConfig.DISTANCE_JUMP_MIN_HOR_ACT &&
					Math.Abs(horDiff) <= AiConfig.DISTANCE_JUMP_MAX_HOR_ACT && 
                    (lastPlatform != Path.First().RefPlatform))
                {
					bool facingRight = Character().CharacterPhysics.IsFacingRight();
	                float horSpeed = Character().CharacterPhysics.GetSpeed().X;
	                if ((horDiff > 0 && facingRight && horSpeed > AiConfig.SPEED_JUMP_MIN) || 
						(horDiff < 0 && !facingRight && horSpeed < -AiConfig.SPEED_JUMP_MIN))
	                {
		                actions.actions.Add(InputConfig.Actions.JUMP);
	                }
                }
			}
			else if (isClimbing)
			{
				// If climbing with no waypoint, react appropriately
				if (Target != null)
				{
					float diffY = Target.GetGroundPosition().Y - CharacterPos().Y;
					if (Math.Abs(diffY) < AiConfig.DISTANCE_VERT_LIMIT_CLIMB_HOR)
					{
						movement = DirectionToPosition(Target.GetCenterPosition());
						movement.Normalize();
					}
					else if (diffY < 0)
					{
						movement = new Vector2(0, (diffY < 0) ? 1 : 0);
					}
					else
					{
						movement = new Vector2(0, -1);
					}
				}
				else
				{
					movement = new Vector2(0, -1);
				}
			}
			else
			{
				float diffX = notMoved ? DirectionToTarget().X : 0;
				movement = new Vector2(diffX, 0);
				movement.Normalize();
			}


			// Ensure, that the character moves
			Vector2 absM = new Vector2(Math.Abs(movement.X), Math.Abs(movement.Y));
	        if (absM.X > 0.1f && absM.X < CharacterConfig.INPUT_X_SENS)
	        {
		        movement.X *= 1 / absM.X;
	        }
	        else if (absM.Y > 0.1f && absM.Y < CharacterConfig.INPUT_X_SENS)
			{
				movement.Y *= 1 / absM.Y;
			}
			actions.axisActions.Add(new Tuple<int, Vector2>(InputConfig.Actions.MOVE_DIRECTION, movement));
			

	        lastCenterPos = this.Character().GetCenterPosition();

        }

	    private bool NextWaypointUnreachable()
	    {
		    if (Path.Count == 0)
			    return false;

		    if ((CharacterPos().Y - Path.First().Position.Y >= AiConfig.DISTANCE_WAYPOINT_UNREACHABLE_VERT) &&
				(Math.Abs(CharacterPos().X - Path.First().Position.X) <= AiConfig.DISTANCE_WAYPOINT_UNREACHABLE_HOR) &&
				!Character().CharacterPhysics.IsClimbing)
			    return true;

		    if ((System.Environment.TickCount - lastWaypointChange) > AiConfig.TIME_UNREACHABLE &&
		        (lastCenterPos - Character().GetCenterPosition()).Length() < 1 &&
		        Character().CharacterPhysics.GetSpeed().Length() < 0.2)
	        {
	            return true;
	        }

	        if ((System.Environment.TickCount - lastWaypointChange) > AiConfig.TIME_LIFESPAN_WAYPOINT_MAX)
	        {
	            return true;
	        }

			return false;
	    }

        private int pathCompCounter = 0;
        private void WalkToTarget()
        {
            Collidable currP = Character().CharacterPhysics.QueryCurrentPlatform();

            if (Target == null)
                return;

            bool bothLast = false;

            // Check if the target is on the same platform
            if (currP != null)
            {
                lastPlatform = currP;
            }
            else
            {
                if (lastPlatform != null)
                {
                    currP = lastPlatform;
                    bothLast = true;
                }
                else
                {
                    return;
                }
            }

            Collidable tgtP = Target.CharacterPhysics.QueryCurrentPlatform();

            if (tgtP == null)
            {
                if (pathCompCounter > 100)
                {
                    return;
                }

                if (lastPlatformTarget != null)
                {
                    tgtP = lastPlatformTarget;
                }
                else
                {
                    return;
                }
            }
            else
            {
                pathCompCounter = 0;

                if (Path.Count > 0 && Path.Last().RefPlatform == tgtP)
                {
                    // Path is still valid
                    return;
                }

                bothLast = false;
            }

            lastPlatformTarget = tgtP;

            if (currP == tgtP && !bothLast)
            {
                // Simply walk towards target
                Path.Clear();
                Path.Add(new Waypoint(Target.GetCenterPosition(), currP));
                lastWaypointChange = System.Environment.TickCount;
            }
            else
            {
                // Find a way to the target's platform
	            if (Path.Count == 0 || (Path.Count > 0 && Path.Last().RefPlatform != tgtP))
	            {
		            Path = ComputePath(currP, tgtP);
		            if (Path.Count > 0)
		            {
						lastWaypointChange = System.Environment.TickCount;
					}
	            }
            }

        }

        private List<Waypoint> ComputePath(Collidable currP, Collidable tgtP)
        {
			List<Waypoint> res = new List<Waypoint>();
			// Map to remember last step of fastest connection to <key>
			Dictionary<NavGraphNode, NavGraphNode> pathMap = new Dictionary<NavGraphNode, NavGraphNode>();

	        NavGraphNode start = scene.NavigationGraph.findNode(currP);
	        NavGraphNode target = null;

			// Do breadth-first-search on NavGraph (start by adding all neighboring nodes of the start node to "to visit")
			// <which new node to visit, last platform before this new one> 
	        List<Tuple<NavGraphNode, NavGraphNode>> toVisit = new List<Tuple<NavGraphNode, NavGraphNode>>();
	        foreach (Tuple<NavGraphNode, Vector2, Vector2> t in start.ReachableNodes)
	        {
		        if (!pathMap.ContainsKey(t.Item1))
		        {
			        pathMap.Add(t.Item1, start);
			        toVisit.Add(new Tuple<NavGraphNode, NavGraphNode>(t.Item1, start));
		        }
	        }

	        while (toVisit.Count > 0)
	        {
		        Tuple<NavGraphNode, NavGraphNode> t = toVisit.First();
		        NavGraphNode next = t.Item1;
				toVisit.RemoveAt(0);

				// Check if the currently visited node is the target platform
		        if (next.Platform == tgtP)
		        {
					// Found our target platform
			        if (!pathMap.ContainsKey(next))
			        {
				        pathMap.Add(next, t.Item2);
			        }
			        target = t.Item1;
			        break;
		        }

				// Not yet reached our target, check connections to other platforms (which not have been visited yet)
		        foreach (Tuple<NavGraphNode, Vector2, Vector2> newConn in next.ReachableNodes)
		        {
			        if (!pathMap.ContainsKey(newConn.Item1))
			        {
				        // Not yet visited
				        if (!pathMap.ContainsKey(newConn.Item1))
				        {
					        pathMap.Add(newConn.Item1, next);
					        toVisit.Add(new Tuple<NavGraphNode, NavGraphNode>(newConn.Item1, next));
				        }
			        }
		        }

	        }

			// Check if target platform has been found
	        if (target != null)
	        {
				// Compute final path
		        res = BacktrackPath(pathMap, start, target);
            }

	        return res;
        }

		// Compute waypoints from start to target via the paths defined with the pathMap<next, previous>
	    private List<Waypoint> BacktrackPath(Dictionary<NavGraphNode, NavGraphNode> pathMap, NavGraphNode start, NavGraphNode target)
	    {
		    List<Waypoint> res = new List<Waypoint>();

		    NavGraphNode current = target;
		    while (current != start)
		    {
			    NavGraphNode next = pathMap[current];
				// Compute path from previous (next) to the current one (<- reverse order)
			    res.InsertRange(0, ComputeConnection(next, current));
			    current = next;
		    }

			return res;
	    }

		// Computes a (hopefully) efficient connection from one Platform start to Platform end
	    private List<Waypoint> ComputeConnection(NavGraphNode start, NavGraphNode end)
	    {
		    List<Waypoint> res = new List<Waypoint>();

		    Collidable startC = start.Platform;
		    Collidable endC = end.Platform;

			// Position of the transition waypoints between the two nodes
		    Vector2 fst, snd;
		    foreach (Tuple<NavGraphNode, Vector2, Vector2> t in start.ReachableNodes)
		    {
			    if (t.Item1 == end)
			    {
				    
				    fst = t.Item2; // waypoint on start
					snd = t.Item3; // waypoint on end

					// if different platforms (but roughly on same height): ensure, that the waypoint clearly
					// lies on the target platform
					if (end.Platform != start.Platform && Math.Abs(fst.Y - snd.Y) <= AiConfig.DISTANCE_JUMP_VERT
						&& Math.Abs(fst.X - snd.X) > AiConfig.DISTANCE_JUMP_MIN_HOR)
			        {
			            if (fst.X < snd.X)
			            {
			                snd.X += AiConfig.DISTANCE_OVERSHOOT_HOR;
			            }
                        else if (fst.X > snd.X)
			            {
			                snd.X -= AiConfig.DISTANCE_OVERSHOOT_HOR;
			            }
			        }

					// If target-waypoint is higher up (and only reachable by climbing): ensure that the
					// AI climbs high enough and doesn't get stuck at the top
			        if (fst.Y > snd.Y && Math.Abs(fst.Y - snd.Y) > AiConfig.DISTANCE_JUMP_VERT)
			        {
						fst.Y -= AiConfig.DISTANCE_OVERSHOOT_UP;
						snd.Y -= 0.8f * AiConfig.DISTANCE_OVERSHOOT_UP;
			        }
					else if (fst.Y < snd.Y && Math.Abs(fst.Y - snd.Y) > AiConfig.DISTANCE_JUMP_VERT)
					{
						fst.Y -= AiConfig.DISTANCE_OVERSHOOT_UP;
					}

					// Add in the correct order
					res.Add(new Waypoint(new Vector3(fst.X, fst.Y, 0), startC));
					res.Add(new Waypoint(new Vector3(snd.X, snd.Y, 0), endC));

					return res;
			    }
		    }


			return res;
	    }


	    private void WalkRandom()
        {
            Collidable currPlatform = Character().CharacterPhysics.QueryCurrentPlatform();

	        if (currPlatform == null)
		        return;

	        if (Path.Count == 0)
	        {
		        Path.Add(RandomWaypointOnPlatform(currPlatform));
	        }
            else if (Path.Count == 1 && WithinV3Distance(Path.First().Position - CharacterPos(), Behavior.WaypointReachedDistance))
            {
                // Reached the current waypoint, and it is the only one. Add another.
                if (Path.First().RefPlatform == currPlatform)
                {
                    Path.Add(RandomWaypointOnPlatform(currPlatform));
                }
                else
                {
                    Path.Add(RandomWaypointOnPlatform(Path.First().RefPlatform));
                }
            }
            // Else: First waypoint is not yet reached or there is already a waypoint after it. Do nothing.

        }

        private double DistanceToWaypoint(Waypoint w)
        {
            return (w.Position - CharacterPos()).Length();
        }

        private Vector2 DirectionToWaypoint(Waypoint w)
        {
            Vector3 diff = (w.Position - CharacterPos());
            return new Vector2(diff.X, diff.Y);
        }

        private Vector2 DirectionToPosition(Vector3 pos)
        {
            Vector3 diff = (pos - CharacterPos());
            return new Vector2(diff.X, diff.Y);
        }

        private float DistanceToTarget()
        {
            if (Target != null)
            {
                return (Target.GetGroundPosition() - CharacterPos()).Length();
            }
            else
            {
                return Single.MaxValue;
            }
        }

		private Vector3 DirectionToTarget()
		{
			if (Target != null)
			{
				return (Target.GetGroundPosition() - CharacterPos());
			}
			else
			{
				return Vector3.Zero;
			}
		}

		private bool WithinV3Distance(Vector3 p, Vector3 lim)
	    {
		    if (Math.Abs(p.X) <= lim.X && Math.Abs(p.Y) <= lim.Y && Math.Abs(p.Z) <= lim.Z)
			    return true;

		    return false;
	    }

	    private Vector3 CharacterPos()
	    {
		    DefaultCharacter c = Character();
		    float x = c.GetPosition().X + 0.5f * c.GetSize().X;
		    float y = c.GetPosition().Y + 0.75f * c.GetSize().Y;
			float z = c.GetPosition().Z + 0.5f * c.GetSize().Z;

			return new Vector3(x, y, z);
	    }

        private Waypoint RandomWaypointOnPlatform(Collidable platform)
        {
            float posX = (float) random.NextDouble() * (platform.WalkableMinMax.Y - platform.WalkableMinMax.X);
            float posY = platform.MaxHeight;

            return new Waypoint(new Vector3(posX, posY, 0), platform);
        }

        private DefaultCharacter Character()
        {
            return (DefaultCharacter) this.PossesedCharacters[0];
        }

        private Collidable SafeQueryTargetPlatform()
        {
            if (Environment.TickCount - lastSuccQueryTarget <= AiConfig.TIME_DELTA_SAFE_QUERY)
            {
                return lastPlatformTarget;
            }
            Collidable res = Target?.CharacterPhysics.QueryCurrentPlatform();

            if (res == null)
            {
                return lastPlatformTarget;
            }
            else
            {
                lastSuccQueryTarget = Environment.TickCount;
                return res;
            }
        }

        private DefaultCharacter FindTarget()
        {
            DefaultCharacter tgtNew = null;
            double dis = Behavior.RadiusAgression;
            foreach (Player player in scene.GetPlayers())
            {
                foreach (Character character in player.PossesedCharacters)
                {
                    DefaultCharacter defChar = (DefaultCharacter)character;
                    if (defChar == null || defChar.CharacterPhysics.IsStunned)
                        continue;


                    double disTmp = (defChar.GetPosition() - Character().GetPosition()).Length();
                    // Prefer characters with treasures
                    if ((tgtNew?.InvTreasure == null && disTmp <= dis) || (tgtNew != null && tgtNew.InvTreasure == null && defChar.InvTreasure != null))
                    {
                        tgtNew = defChar;
                        dis = disTmp;
                    }
                }
            }
			if (tgtNew != null)
			{
				timeTargetAcquired = Environment.TickCount;
			}
			return tgtNew;
        }

        private void DespawnAi()
        {
            Path.Clear();
            scene.RemoveActor(Character());
            scene.Characters.Remove(Character());
            scene.RemoveAiController(this);

            //Remove footstep particle effect for the current Ai
            scene.RemoveParticleEffect(Character().CharacterPhysics.FootstepEffectManager);

            //Show particle effects if AI disappears
            Vector3 position = Character().GetCenterPosition();
            Vector3 size = Character().GetSize();
            
            FoliageCutParticleEffectFactory foliageCutFactory =
                new FoliageCutParticleEffectFactory(new Vector2(position.X, position.Y+20), (int)size.Length(), 600, new Vector2(0.8f, 1.3f), new Vector2(-0.5f, -10f)/*Vector2.Zero*/, 1f, 0f, Color.ForestGreen, Color.Brown, 400, rotVel: 0.75f);
            ParticleEffectManager foliageCutEffectManager = new ParticleEffectManager(ParticleConfig.FOLIAGE_CUT_TEXTURE, foliageCutFactory, 30, 200, false, BlendState.NonPremultiplied, effectDuration: 600);
            scene.ParticleEffects.Add(foliageCutEffectManager);
        }
    }

    public class Waypoint
    {
        public Vector3 Position { get; set; }
        public Collidable RefPlatform { get; set; }

        public Waypoint(Vector3 position, Collidable refPlatform)
        {
            Position = position;
            RefPlatform = refPlatform;
        }
    }
}
