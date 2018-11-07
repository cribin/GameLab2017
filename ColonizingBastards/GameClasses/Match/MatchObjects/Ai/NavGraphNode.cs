using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.MiscUtil.Collidables;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.GameClasses.Match.MatchObjects.Ai
{
    public class NavGraphNode
    {
        public Collidable Platform { get; private set; }

        public List<Tuple<NavGraphNode, Vector2, Vector2>> ReachableNodes { get; private set; }

        //Leftmost point: Platform.MinWidth(). Rightmost point : Platform.MinHeight()

        public NavGraphNode(Collidable platform)
        {
            Platform = platform;
            ReachableNodes = new List<Tuple<NavGraphNode, Vector2, Vector2>>();
        }

      
    }
}
