using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.Base.MiscUtil.Collidables
{
    /// <summary>
    /// Represent climbable polygons that belong together
    /// 
    /// </summary>
    class ClimbGroup
    {
        private List<Collidable> climbCollidables;

        public float ClimbGroupCenterY { get; private set; } = float.MinValue;
        public float ClimbGroupMinY { get; private set; } = float.MaxValue;
        public float ClimbGroupMaxY { get; private set; } = float.MinValue;

        public List<Collidable> ClimbCollidables
        {
            get
            {
                climbCollidables = climbCollidables.OrderBy(c => c.Center.Y).ToList();
                return climbCollidables;
            }

            set { climbCollidables = value; }
        }

        //Stores the height of the climb collidable with the largest height=(MaxHeight - MinHeight)
        public float DeltaMaxHeight { get; private set; }

        
        public ClimbGroup()
        {
            climbCollidables = new List<Collidable>();
            DeltaMaxHeight = float.MinValue;
        }

        public void AddClimbCollidable(Collidable collidable)
        {
            if (climbCollidables.Contains(collidable)) return;
            climbCollidables.Add(collidable);
            var deltaHeight = collidable.MaxHeight - collidable.MinHeight;
            if (deltaHeight > DeltaMaxHeight)
                DeltaMaxHeight = deltaHeight;

            if (ClimbGroupCenterY.Equals(float.MinValue))
                ClimbGroupCenterY = collidable.Center.Y;
            else
                ClimbGroupCenterY = (ClimbGroupCenterY + collidable.Center.Y) / 2f;

            if (collidable.MinHeight < ClimbGroupMinY)
                ClimbGroupMinY = collidable.MinHeight;

            if(collidable.MaxHeight > ClimbGroupMaxY)
                ClimbGroupMaxY = collidable.MaxHeight;

            foreach (Collidable c in climbCollidables)
            {
                c.CenterYClimbGroup = ClimbGroupCenterY;
                c.ClimbGroupMinY = ClimbGroupMinY;
                c.ClimbGroupMaxY = ClimbGroupMaxY;
            }
        }

        //TODO:not used right now, but should be modified if used
        public void RemoveClimbCollidable(Collidable collidable)
        {
            climbCollidables.Remove(collidable);
            var deltaHeight = collidable.MaxHeight - collidable.MinHeight;
            if (Math.Abs(deltaHeight - DeltaMaxHeight) < 0.01)
            {
                DeltaMaxHeight = climbCollidables.Max(c => (c.MaxHeight - c.MinHeight));
            }

            ClimbGroupMinY = climbCollidables.Min(c => c.MinHeight);
        }
    }
}
