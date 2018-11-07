using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.Base.MiscUtil.Collidables
{
    public class CollidableResults
    {
        public bool WillIntersect; // Are the polygons going to intersect forward in time?
        public bool Intersect; // Are the polygons currently intersecting
        public bool IntersectLeft;
        public bool IntersectRight;
        public bool IntersectUp;
        public bool IntersectDown;
        public Vector3 MinimumTranslationVector; // The translation to apply to polygon A to push the polygons appart.
        public Vector3 HitPoint;
        public Vector3 CollisionNormal;//the normal perpendicular to the collision point
    }

    public abstract class Collidable
    {
        public List<Vector2> Points { get; set; }

        public List<Vector2> Edges { get; set; }

        public bool IsClimbable { get; protected set; }

        public float CenterYClimbGroup { get; set; }

        public float ClimbGroupMinY { get; set; }

        public float ClimbGroupMaxY { get; set; }

        public bool IsWalkable { get; protected set; }

        //!!NEEDS TO BE CORRECTED, if a polygon is on top of this collidable
        public Vector2 WalkableMinMax = new Vector2(float.MaxValue,float.MinValue);

        protected Collidable(List<Vector2> points)
        {
            Points = points;

            MinWidth = points.Min(p => p.X);
            MaxWidth = points.Max(p => p.X);
            MinHeight = points.Min(p => p.Y);
            MaxHeight = points.Max(p => p.Y);
        }

        protected void BuildEdges()
        {
            Edges.Clear();

            for (var i = 0; i < Points.Count; i++)
            {
                var p1 = Points[i];
                var p2 = i + 1 >= Points.Count ? Points[0] : Points[i + 1];
                Edges.Add(p2 - p1);

                
                if (!IsWalkable) continue;
                float test = (float)Math.Atan2(Edges[i].Y, Math.Abs(Edges[i].X));
                //accept slope if the steepness is less than 45 deg
                if ( test < Math.PI / 4)
                {
                    if (p1.X < p2.X)
                    {
                        if (p1.X < WalkableMinMax.X)
                            WalkableMinMax.X = p1.X;

                        if (p2.X > WalkableMinMax.Y)
                            WalkableMinMax.Y = p2.X;
                    }
                    else
                    {
                        if (p2.X < WalkableMinMax.X)
                            WalkableMinMax.X = p2.X;

                        if (p1.X > WalkableMinMax.Y)
                            WalkableMinMax.Y = p1.X;
                    }
                }
            }
        }

        /// <summary>
        ///     returns center of the collidable
        /// </summary>
        public Vector2 Center
        {
            get
            {
                float totalX = 0;
                float totalY = 0;
                for (var i = 0; i < Points.Count; i++)
                {
                    totalX += Points[i].X;
                    totalY += Points[i].Y;
                }

                return new Vector2(totalX / Points.Count, totalY / Points.Count);
            }
        }

     
        /// returns the point with highest y value
        /// </summary>
        /// <returns></returns>
        public float MaxHeight { get; set; }
      
        public float MinHeight { get; set; }
       

        public float MaxWidth { get; set; }
       
        public float MinWidth { get; set; }

        public abstract CollidableResults CheckForCollision(Collidable collidable, Vector3 velocity);

		// Relies on ordered points
		// https://stackoverflow.com/questions/4243042/c-sharp-point-in-polygon
		// (Temporary solution)
		public bool IsWithinCollidable(Vector2 point)
        {
	        List<Vector2> polygon = new List<Vector2>(Points);
			polygon.Add(Points[0]);
			bool result = false;
			int j = polygon.Count() - 1;
			for (int i = 0; i < polygon.Count(); i++)
			{
				if (polygon[i].Y < point.Y && polygon[j].Y >= point.Y || polygon[j].Y < point.Y && polygon[i].Y >= point.Y)
				{
					if (polygon[i].X + (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < point.X)
					{
						result = !result;
					}
				}
				j = i;
			}
			return result;
		}

		// Checks if the given line segment p+t*r, t in [0,1] intersects with the line given by the collidable
	    public bool IntersectsWithPolygon(Vector2 p, Vector2 r)
	    {
			int numP = Points.Count;

		    for (int ind = 0; ind < numP; ind++)
		    {
			    int indNext = (ind + 1) % numP;
			    Vector2 q = Points[ind];
			    Vector2 s = Points[indNext] - q;

				// Check for an intersection with this edge
			    float c1 = Cross2D(r, s);
			    float c2 = Cross2D(q - p, r);

			    if (Math.Abs(c1) < 0.0001 && Math.Abs(c2) < 0.0001)
			    {
				    // Lines are colinear
				    float t0 = Vector2.Dot(q - p, r) / Vector2.Dot(r, r);
				    float t1 = t0 + Vector2.Dot(s, r) / Vector2.Dot(r, r);

					// Check if the colinear line segments overlap in [0,1]
				    if (Vector2.Dot(s, r) < 0)
				    {
					    if ((t1 <= 0 && t0 >= 0) || (t1 <= 1 && t0 >= 1) ||
					        (t1 >= 0 && t1 <= 1 && t0 >= 0 && t0 <= 1))
						    return true;
				    }
				    else
				    {
					    if ((t0 <= 0 && t1 >= 0) || (t0 <= 1 && t1 >= 1) ||
					        (t1 >= 0 && t1 <= 1 && t0 >= 0 && t0 <= 1))
						    return true;
				    }
			    }
			    else if (Math.Abs(c1) < 0.0001)
			    {
					// Parallel, no intersection
				    continue;
			    }
			    else
			    {
				    float t = Cross2D(q - p, s) / c1;
				    float u = Cross2D(q - p, r) / c1;

				    if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
				    {
						// Collision of line segments
					    return true;
				    }
				    else
				    {
						// Intersection outside of line segment
					    continue;
				    }
			    }

		    }

			// Checked all parts of the polygon; no intersection
		    return false;
	    }

	    public float Cross2D(Vector2 v1, Vector2 v2)
	    {
		    return (v1.X * v2.Y - v1.Y * v2.X);
	    }

        /*// On edge or positive (right side in e1 -> e2)
        protected bool IsOnPositiveSide(Vector2 e1, Vector2 e2, Vector2 p)
        {
            Vector2 e = e2 - e1;
            Vector2 eR = new Vector2(e.Y, -e.X);
            Vector2 e1P = p - e1;
            return Vector2.Dot(eR, e1P) >= 0;
        }*/

        //public abstract CollidableResults CheckForCircleCollision(Circle circle, Vector3 velocity);
        // Return points representing an enlarged polygon.
         public void EnlargePolygon(
            float offset)
         {
             List<Vector2> enlarged_points = new List<Vector2>();
             int num_points = Points.Count;
             for (int j = 0; j < num_points; j++)
             {
                 // Find the new location for point j.
                 // Find the points before and after j.
                 int i = (j - 1);
                 if (i < 0) i += num_points;
                 int k = (j + 1) % num_points;

                 // Move the points by the offset.
                 Vector2 v1 = new Vector2(
                     Points[j].X - Points[i].X,
                     Points[j].Y - Points[i].Y);
                 v1.Normalize();
                 v1 *= offset;
                 Vector2 n1 = new Vector2(-v1.Y, v1.X);

                 Vector2 pij1 = new Vector2(
                     (Points[i].X + n1.X),
                     (Points[i].Y + n1.Y));
                 Vector2 pij2 = new Vector2(
                     (float)(Points[j].X + n1.X),
                     (float)(Points[j].Y + n1.Y));

                 Vector2 v2 = new Vector2(
                     Points[k].X - Points[j].X,
                     Points[k].Y - Points[j].Y);
                 v2.Normalize();
                 v2 *= offset;
                 Vector2 n2 = new Vector2(-v2.Y, v2.X);

                 Vector2 pjk1 = new Vector2(
                     Points[j].X + n2.X,
                     Points[j].Y + n2.Y);
                 Vector2 pjk2 = new Vector2(
                     Points[k].X + n2.X,
                     Points[k].Y + n2.Y);

                 // See where the shifted lines ij and jk intersect.
                 bool lines_intersect, segments_intersect;
                 Vector2 poi, close1, close2;
                 FindIntersection(pij1, pij2, pjk1, pjk2,
                     out lines_intersect, out segments_intersect,
                     out poi, out close1, out close2);
                 Debug.Assert(lines_intersect,
                     "Edges " + i + "-->" + j + " and " +
                     j + "-->" + k + " are parallel");

                 enlarged_points.Add(poi);
             }

             for (var i = 0; i < enlarged_points.Count; i++)
             {
                 Vector2 point = enlarged_points[i];
                 enlarged_points[i] = new Vector2((float)Math.Round(point.X, MidpointRounding.AwayFromZero), (float)Math.Round(point.Y, MidpointRounding.AwayFromZero));
             }
             Points = enlarged_points;

         }

        // Find the point of intersection between
        // the lines p1 --> p2 and p3 --> p4.
        private void FindIntersection(
            Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4,
            out bool lines_intersect, out bool segments_intersect,
            out Vector2 intersection,
            out Vector2 close_p1, out Vector2 close_p2)
        {
            // Get the segments' parameters.
            float dx12 = p2.X - p1.X;
            float dy12 = p2.Y - p1.Y;
            float dx34 = p4.X - p3.X;
            float dy34 = p4.Y - p3.Y;

            // Solve for t1 and t2
            float denominator = (dy12 * dx34 - dx12 * dy34);

            float t1 =
                ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34)
                    / denominator;
            if (float.IsInfinity(t1))
            {
                // The lines are parallel (or close enough to it).
                lines_intersect = false;
                segments_intersect = false;
                intersection = new Vector2(float.NaN, float.NaN);
                close_p1 = new Vector2(float.NaN, float.NaN);
                close_p2 = new Vector2(float.NaN, float.NaN);
                return;
            }
            lines_intersect = true;

            float t2 =
                ((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12)
                    / -denominator;

            // Find the point of intersection.
            intersection = new Vector2(p1.X + dx12 * t1, p1.Y + dy12 * t1);

            // The segments intersect if t1 and t2 are between 0 and 1.
            segments_intersect =
                ((t1 >= 0) && (t1 <= 1) &&
                 (t2 >= 0) && (t2 <= 1));

            // Find the closest points on the segments.
            if (t1 < 0)
            {
                t1 = 0;
            }
            else if (t1 > 1)
            {
                t1 = 1;
            }

            if (t2 < 0)
            {
                t2 = 0;
            }
            else if (t2 > 1)
            {
                t2 = 1;
            }

            close_p1 = new Vector2(p1.X + dx12 * t1, p1.Y + dy12 * t1);
            close_p2 = new Vector2(p3.X + dx34 * t2, p3.Y + dy34 * t2);
        }
    }
}
