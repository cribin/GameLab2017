using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.MiscUtil.Collidables;
using ColonizingBastards.Base.Scene;
using ColonizingBastards.GameClasses.Match.Config;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.GameClasses.Match.MatchObjects.Ai
{
    class NavGraph
    {
        public NavGraphNode[] NavGraphNodes { get; }

        private Scene scene;

        //this list contains a list for each group of climbable polygons, that are connected to each other
        public List<ClimbGroup> CurrClimbGroup { get; }

        public NavGraph(Scene scene)
        {
            this.scene = scene;
            NavGraphNodes = new NavGraphNode[scene.WalkableCollidables.Count];
            CurrClimbGroup = new List<ClimbGroup>();
            //Init a NavGraphNode for each walkable platform in the scene
            for (var i = 0; i < NavGraphNodes.Length; i++)
            {
                NavGraphNodes[i] = new NavGraphNode(scene.WalkableCollidables[i]);
            }

            InitClimbGroups();

            InitClimbReachableNodes();

            InitCloseReachableNodes();

            scene.NavigationGraph = this;
        }

        private void InitClimbGroups()
        {
            //Duplicate original list(avoid passing by reference)
            List<Collidable> remainingClimbCollidables = new List<Collidable>(scene.ClimableCollidables);

            //Group together connected climbables if their close to each other on the X-Axis
            while(remainingClimbCollidables.Count>0)
            {
                Collidable c1 = remainingClimbCollidables[0];
                float deltaX = c1.MaxWidth - c1.MinWidth;
                ClimbGroup newClimbGroup = new ClimbGroup();
                newClimbGroup.AddClimbCollidable(c1);

                foreach (Collidable c2 in remainingClimbCollidables)
                {
                    if (Math.Abs(c1.Center.X - c2.Center.X) < deltaX)
                    {
                        newClimbGroup.AddClimbCollidable(c2);
                    }
                }

                CurrClimbGroup.Add(newClimbGroup);

                remainingClimbCollidables.RemoveAll(c2 => Math.Abs(c1.Center.X - c2.Center.X) < deltaX);
            }

            //Group together connected climbables if their close to each other on the Y-Axis(refine the result from before)
            //TODO:if necessary
            /*foreach (ClimbGroup cGroup in climbGroup)
            {
                List<Collidable> groupNodes = cGroup.ClimbCollidables;

            }*/

        }

        private void InitClimbReachableNodes()
        {
            foreach (ClimbGroup climbGroup in CurrClimbGroup)
            {
                List<Collidable> climbCollidables = climbGroup.ClimbCollidables;
                Collidable upperClimbCollidable = climbCollidables[0];
                foreach (var navGraphNode1 in NavGraphNodes)
                {
                    CollidableResults result1 = upperClimbCollidable.CheckForCollision(navGraphNode1.Platform, Vector3.Zero);
                    //check which platform intersects the upper part of the climbable 
                    if (result1.Intersect && upperClimbCollidable.MaxHeight >= navGraphNode1.Platform.MaxHeight -10 && navGraphNode1.Platform.MinWidth <= upperClimbCollidable.MinWidth + 20 && navGraphNode1.Platform.MaxWidth >= upperClimbCollidable.MaxWidth - 20)
                    {
                        Vector2 startPos = new Vector2(upperClimbCollidable.Center.X, upperClimbCollidable.MinHeight);
                        Collidable lowerClimbCollidable = climbCollidables[climbCollidables.Count - 1];
                        foreach (var navGraphNode2 in NavGraphNodes)
                        {
                            if (navGraphNode1 == navGraphNode2) continue;
                            CollidableResults result2 = lowerClimbCollidable.CheckForCollision(navGraphNode2.Platform,
                                Vector3.Zero);
                            //check which platform intersects the lower part of the climbable 
                            if (result2.Intersect && lowerClimbCollidable.MaxHeight < navGraphNode2.Platform.MaxHeight)
                            {
                                Vector2 endPos = new Vector2(lowerClimbCollidable.Center.X,
                                    navGraphNode2.Platform.MinHeight);

	                            if (startPos.Y < endPos.Y)
	                            {
									navGraphNode1.ReachableNodes.Add(
										new Tuple<NavGraphNode, Vector2, Vector2>(navGraphNode2, 
										startPos - new Vector2(0, 20),
										endPos - new Vector2(0, 20)));
									navGraphNode2.ReachableNodes.Add(
										new Tuple<NavGraphNode, Vector2, Vector2>(navGraphNode1,
										endPos - new Vector2(0, 120),
										startPos - new Vector2(0, 20)));
								}
	                            else
	                            {
									navGraphNode1.ReachableNodes.Add(
										new Tuple<NavGraphNode, Vector2, Vector2>(navGraphNode2,
										startPos - new Vector2(0, 120),
										endPos - new Vector2(0, 20)));
									navGraphNode2.ReachableNodes.Add(
										new Tuple<NavGraphNode, Vector2, Vector2>(navGraphNode1,
										endPos - new Vector2(0, 20),
										startPos - new Vector2(0, 20)));
								}

                            }
                        }
                    }
                }
            }
        }

	    private void InitCloseReachableNodes()
	    {
		    for (int i = 0; i < NavGraphNodes.Length; i++)
		    {
			    NavGraphNode n = NavGraphNodes[i];
			    for (int j = i; j < NavGraphNodes.Length; j++)
			    {
				    NavGraphNode o = NavGraphNodes[j];
				    // Check if vertically close enough
				    if (Math.Abs(n.Platform.MinHeight - o.Platform.MinHeight) <= AiConfig.DISTANCE_JUMP_VERT)
				    {
						// Check horizontal displacement
						Vector2 nMm = n.Platform.WalkableMinMax;
						Vector2 oMm = o.Platform.WalkableMinMax;
					    float leftDis = nMm.X - oMm.Y;
					    float rightDis = oMm.X - nMm.Y;
						if (leftDis <= AiConfig.DISTANCE_JUMP_MAX_HOR && leftDis >= -AiConfig.DISTANCE_VALID_OVERLAP)
					    {
						    // Horizontally reachable (both ways) on the left side of n
						    Vector2 wpN = new Vector2(nMm.X + AiConfig.DISTANCE_OFFS_DROP_HOR, n.Platform.MinHeight - AiConfig.DISTANCE_OFFS_DROP_HOR);
						    Vector2 wpO = new Vector2(oMm.Y - AiConfig.DISTANCE_OFFS_DROP_HOR, o.Platform.MinHeight - AiConfig.DISTANCE_OFFS_DROP_HOR);

					        Vector2 diff = wpN - wpO;
						    if (!IsWithinPlatform(wpN - 0.3f * diff) && !IsWithinPlatform(wpO + 0.3f * diff)
                                && !IsWithinPlatform(wpN) && !IsWithinPlatform(wpO))
						    {
							    n.ReachableNodes.Add(new Tuple<NavGraphNode, Vector2, Vector2>(o, wpN, wpO));
							    o.ReachableNodes.Add(new Tuple<NavGraphNode, Vector2, Vector2>(n, wpO, wpN));
						    }
					    }
						else if (rightDis <= AiConfig.DISTANCE_JUMP_MAX_HOR && rightDis >= -AiConfig.DISTANCE_VALID_OVERLAP)
						{
							// Horizontally reachable (both ways) on the right side of n
							Vector2 wpN = new Vector2(nMm.Y - AiConfig.DISTANCE_OFFS_DROP_HOR, n.Platform.MinHeight - AiConfig.DISTANCE_OFFS_DROP_HOR);
							Vector2 wpO = new Vector2(oMm.X + AiConfig.DISTANCE_OFFS_DROP_HOR, o.Platform.MinHeight - AiConfig.DISTANCE_OFFS_DROP_HOR);

						    Vector2 diff = wpN - wpO;
						    if (!IsWithinPlatform(wpN - 0.3f * diff) && !IsWithinPlatform(wpO + 0.3f * diff)
						        && !IsWithinPlatform(wpN) && !IsWithinPlatform(wpO))
                            {
								n.ReachableNodes.Add(new Tuple<NavGraphNode, Vector2, Vector2>(o, wpN, wpO));
								o.ReachableNodes.Add(new Tuple<NavGraphNode, Vector2, Vector2>(n, wpO, wpN));
							}
						}
					}
					else
				    {
                        NavGraphNode above, below;
					    if (n.Platform.MinHeight < o.Platform.MinHeight)
					    {
						    // N is higher then O
						    above = n;
						    below = o;
					    }
					    else
					    {
							// O is higher then N
						    above = o;
						    below = n;
					    }

						// Check horizontal overlap of the platforms
					    Vector2 abMm = above.Platform.WalkableMinMax;
						Vector2 belMm = above.Platform.WalkableMinMax;
				        if (abMm.X >= belMm.X)
				        {
				            // Can drop from platform above to below (left side)
				            Vector2 wpAbove = new Vector2(abMm.X - AiConfig.DISTANCE_OFFS_DROP_COLL, above.Platform.MinHeight - AiConfig.DISTANCE_OFFS_DROP_COLL_VERT);
				            Vector2 wpBelow = new Vector2(abMm.X - AiConfig.DISTANCE_OFFS_DROP_COLL, below.Platform.MinHeight - 5);
				            if (!IntersectsWithCollidable(wpAbove, wpBelow - wpAbove))
				            {
				                wpAbove += new Vector2(-AiConfig.DISTANCE_OFFS_DROP_HOR, AiConfig.DISTANCE_OFFS_DROP_COLL_VERT - AiConfig.DISTANCE_OFFS_DROP_HOR);
                                wpBelow += new Vector2(-AiConfig.DISTANCE_OFFS_DROP_HOR, 0);
				                above.ReachableNodes.Add(new Tuple<NavGraphNode, Vector2, Vector2>(below, wpAbove, wpBelow));
				            }
				        }
				        else if (abMm.Y <= belMm.Y)
				        {
				            // Can drop from platform above to below (right side)
				            Vector2 wpAbove = new Vector2(abMm.X + AiConfig.DISTANCE_OFFS_DROP_COLL, above.Platform.MinHeight - AiConfig.DISTANCE_OFFS_DROP_COLL_VERT);
				            Vector2 wpBelow = new Vector2(abMm.X + AiConfig.DISTANCE_OFFS_DROP_COLL, below.Platform.MinHeight - 5);
				            if (!IntersectsWithCollidable(wpAbove, wpBelow - wpAbove))
				            {
				                wpAbove += new Vector2(AiConfig.DISTANCE_OFFS_DROP_HOR, AiConfig.DISTANCE_OFFS_DROP_COLL_VERT - AiConfig.DISTANCE_OFFS_DROP_HOR);
				                wpBelow += new Vector2(AiConfig.DISTANCE_OFFS_DROP_HOR, 0);
                                above.ReachableNodes.Add(new Tuple<NavGraphNode, Vector2, Vector2>(below, wpAbove, wpBelow));
				            }
				        }

                    }

                }
		    }
	    }

        private bool IntersectsWithCollidable(Vector2 p, Vector2 r)
        {

            foreach (Collidable c in scene.GetCollidables())
            {
                if (c.IntersectsWithPolygon(p, r))
                    return true;
            }

            return false;
        }

	    private bool IsWithinPlatform(Vector2 p)
	    {
			foreach (Collidable c in scene.WalkableCollidables)
			{
				if (c.IsWithinCollidable(p))
					return true;
			}
			
			return false;
		}


        public NavGraphNode findNode(Collidable collidable)
	    {
		    foreach (NavGraphNode node in NavGraphNodes)
		    {
			    if (node.Platform == collidable)
				    return node;
		    }

		    return null;
	    }
    }
}
