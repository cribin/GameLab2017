using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.Base.MiscUtil.Collidables
{
    public class Ray:Collidable
    {
        public Ray(Vector2 startPos, Vector2 endPos) : base(new List<Vector2> { startPos, endPos })
        {
        }

        public override CollidableResults CheckForCollision(Collidable collidable, Vector3 velocity)
        {
            CollidableResults rayCastCollisionResult = new CollidableResults()
            {
                Intersect = false,
                HitPoint = Vector3.Zero,
                CollisionNormal = Vector3.Zero
            };

            //is the vector perpendicular to the edge of the polygon where the intersection occured

            for (var i = 0; i < collidable.Points.Count; i++)
            {
                var p1 = collidable.Points[i];
                var p2 = i + 1 >= collidable.Points.Count ? collidable.Points[0] : collidable.Points[i + 1];

                rayCastCollisionResult.HitPoint = new Vector3(FindLineIntersection(Points[0], Points[1], p1, p2), 0f);
                if (!rayCastCollisionResult.HitPoint.Equals(Vector3.Zero))
                {
                    rayCastCollisionResult.Intersect = true;
                    rayCastCollisionResult.CollisionNormal = new Vector3(collidable.Edges[i].Y, -collidable.Edges[i].X, 0f);
                    rayCastCollisionResult.CollisionNormal.Normalize();
                    break;
                }
            }


            return rayCastCollisionResult;
        }

        private Vector2 FindLineIntersection(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
        {

            float denom = ((end1.X - start1.X) * (end2.Y - start2.Y)) - ((end1.Y - start1.Y) * (end2.X - start2.X));

            //  lines are parallel 
            if (Math.Abs(denom) < 0.00001)
                return Vector2.Zero;

            float numer = ((start1.Y - start2.Y) * (end2.X - start2.X)) - ((start1.X - start2.X) * (end2.Y - start2.Y));

            float r = numer / denom;

            float numer2 = ((start1.Y - start2.Y) * (end1.X - start1.X)) - ((start1.X - start2.X) * (end1.Y - start1.Y));

            float s = numer2 / denom;

            if ((r < 0 || r > 1) || (s < 0 || s > 1))
                return Vector2.Zero;

            // Find intersection point
            Vector2 result = new Vector2();
            result.X = start1.X + (r * (end1.X - start1.X));
            result.Y = start1.Y + (r * (end1.Y - start1.Y));

            return result;
        }
    }
}
