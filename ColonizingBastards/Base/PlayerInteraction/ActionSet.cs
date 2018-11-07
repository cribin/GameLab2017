using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonizingBastards.Base.PlayerInteraction
{
    class ActionSet
    {

        public HashSet<int> actions;
        public HashSet<Tuple<int, Vector2>> axisActions;

        public ActionSet()
        {
            actions = new HashSet<int>();
            axisActions = new HashSet<Tuple<int, Vector2>>();
        }

        public ActionSet(ActionSet actionSet)
        {
            actions = new HashSet<int>(actionSet.actions);
            axisActions = new HashSet<Tuple<int, Vector2>>(actionSet.axisActions);
        }

    }
}
