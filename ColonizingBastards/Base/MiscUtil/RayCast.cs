using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.MiscUtil.Collidables;
using Microsoft.Xna.Framework;
using Ray = ColonizingBastards.Base.MiscUtil.Collidables.Ray;

namespace ColonizingBastards.Base.MiscUtil
{
  
    /// <summary>
    /// Helper class to shoot rays. Right now only used to detect slopes
    /// </summary>
    public class RayCast
    {
        public static CollidableResults DetectSlope(Collidable ray1, Collidable ray2, Collidable collidable)
        {
            CollidableResults rayCastCollisionResult = new CollidableResults()
            {
                Intersect = false,
                HitPoint = Vector3.Zero,
                CollisionNormal = Vector3.Zero
            };

            if (ray1.Points.Count > 2 || ray2.Points.Count > 2)
                return null;

            

            var rayCastResult1 = ray1.CheckForCollision(collidable, Vector3.Zero);
            var rayCastResult2 = ray2.CheckForCollision(collidable, Vector3.Zero);
            bool slopeOnHitPoint1 = false;
            bool slopeOnHitPoint2 = false;

            if (rayCastResult1.Intersect)
            {
                if (Math.Abs(Vector3.Dot(rayCastResult1.CollisionNormal, Vector3.UnitX)) > 0.000001)
                    slopeOnHitPoint1 = true;
            }
            if (rayCastResult2.Intersect)
            {
                if (Math.Abs(Vector3.Dot(rayCastResult2.CollisionNormal, Vector3.UnitX)) > 0.000001)
                    slopeOnHitPoint2 = true;
                
            }

            //Conditons for slope collision:
            //1: Both rays hit on a slope
            //2: One ray hits a slope and the hit point where the slope is detected is higher than the other hitpoint
            bool condition1 = slopeOnHitPoint1 && slopeOnHitPoint2;
            bool condition2 = slopeOnHitPoint1 && rayCastResult1.HitPoint.Y > rayCastResult2.HitPoint.Y;
            bool condition3 = slopeOnHitPoint2 && rayCastResult2.HitPoint.Y > rayCastResult1.HitPoint.Y;


            if (condition1)
            {
                //Ray centerRay = new Ray(new Vector2((ray1.Points[0].X + ray2.Points[0].X)*0.5f,ray1.Points[0].Y), new Vector2((ray1.Points[1].X + ray2.Points[1].X) * 0.5f, ray1.Points[1].Y));
                if(Math.Abs(rayCastResult1.CollisionNormal.X) < Math.Abs(rayCastResult2.CollisionNormal.X))
                    rayCastCollisionResult = rayCastResult1;
                else
                    rayCastCollisionResult = rayCastResult2;
                //rayCastCollisionResult = rayCastResult1;
            }
            else if (condition2)
                rayCastCollisionResult = rayCastResult1;
            else if (condition3)
                rayCastCollisionResult = rayCastResult2;

            return rayCastCollisionResult;
        }

        public static bool CheckIfInsideClimbingArea(Collidable ray, Collidable collidable)
        {
            var rayCastResult = ray.CheckForCollision(collidable, Vector3.Zero);
            return rayCastResult.Intersect;
        }
    }
}
