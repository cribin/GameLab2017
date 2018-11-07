using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.Base.MiscUtil.Collidables
{
    
    /// <summary>
    /// 
    /// </summary>
    public class Polygon:Collidable
    {
        public Polygon(List<Vector2> points, bool isClimbable, bool isWalkable) : base(points)
        {
            IsClimbable = isClimbable;
            IsWalkable = isWalkable;
            Edges = new List<Vector2>();
            BuildEdges();

        }

        public void Offset(Vector2 v)
        {
            Offset(v.X, v.Y);
        }

        public void Offset(float x, float y)
        {
            for (var i = 0; i < Points.Count; i++)
            {
                var p = Points[i];
                Points[i] = new Vector2(p.X + x, p.Y + y);
            }
        }

        // Calculate the projection of a polygon on an axis
        // and returns it as a [min, max] interval
        private void ProjectPolygon(Vector2 axis, Polygon polygon,
            ref float min, ref float max)
        {
            // To project a point on an axis use the dot product
            var dotProduct = Vector2.Dot(axis, polygon.Points[0]);
            min = dotProduct;
            max = dotProduct;
            for (var i = 1; i < polygon.Points.Count; i++)
            {
                dotProduct = Vector2.Dot(polygon.Points[i], axis);
                if (dotProduct < min)
                {
                    min = dotProduct;
                }
                else
                {
                    if (dotProduct > max)
                        max = dotProduct;
                }
            }
        }

        // Calculate the distance between [minA, maxA] and [minB, maxB]
        // The distance will be negative if the intervals overlap
        private float IntervalDistance(float minA, float maxA, float minB, float maxB)
        {
            if (minA < minB)
                return minB - maxA;
            return minA - maxB;
        }

        // Check if polygon A is going to collide with polygon B for the given velocity
        public override CollidableResults CheckForCollision(Collidable polygon, Vector3 velocity)
        {
            var result = new CollidableResults
            {
                Intersect = true,
                WillIntersect = true
            };

            var edgeCountA = Edges.Count;
            var edgeCountB = polygon.Edges.Count;
            var minIntervalDistance = float.PositiveInfinity;
            var translationAxis = new Vector2();
            Vector2 edge;

            // Loop through all the edges of both polygons
            for (var edgeIndex = 0; edgeIndex < edgeCountB + edgeCountA; edgeIndex++)
            {
                if (edgeIndex < edgeCountB)
                    edge = polygon.Edges[edgeIndex];
                else edge = Edges[edgeIndex - edgeCountB];

                // ===== 1. Find if the polygons are currently intersecting =====

                // Find the axis perpendicular to the current edge
                var axis = new Vector2(-edge.Y, edge.X);
                axis.Normalize();

                // Find the projection of the polygon on the current axis
                float minA = 0;
                float minB = 0;
                float maxA = 0;
                float maxB = 0;
                ProjectPolygon(axis, this, ref minA, ref maxA);
                ProjectPolygon(axis, (Polygon)polygon, ref minB, ref maxB);

                // Check if the polygon projections are currentlty intersecting
                if (IntervalDistance(minB, maxB, minA, maxA) > 0) result.Intersect = false;

                // ===== 2. Now find if the polygons *will* intersect =====

                // Project the velocity on the current axis
                var velocityProjection = Vector2.Dot(axis, new Vector2(velocity.X, velocity.Y));
               
                // Get the projection of polygon A during the movement
                if (velocityProjection < 0)
                    minB += velocityProjection;
                else maxB += velocityProjection;

                // Do the same test as above for the new projection
                var intervalDistance = IntervalDistance(minB, maxB, minA, maxA);
                if (intervalDistance > 0) result.WillIntersect = false;

                // If the polygons are not intersecting and won't intersect, exit the loop
                if (!result.Intersect && !result.WillIntersect) break;

                // Check if the current interval distance is the minimum one. If so store
                // the interval distance and the current distance.
                // This will be used to calculate the minimum translation vector
                intervalDistance = Math.Abs(intervalDistance);
                if (intervalDistance < minIntervalDistance)
                {
                    minIntervalDistance = intervalDistance;
                    translationAxis = axis;

                    var d = polygon.Center - Center;
                    if (Vector2.Dot(d, translationAxis) < 0) translationAxis = -translationAxis;
                }
            }

            // The minimum translation vector can be used to push the polygons appart.
            // First moves the polygons by their velocity
            // then move polygonA by MinimumTranslationVector.
            if (result.WillIntersect) result.MinimumTranslationVector = new Vector3(translationAxis * minIntervalDistance, 0f);

            return result;
        }

        public static Polygon RectangleToPolygon(Rectangle rectangle)
        {
            var cornerPoints = new List<Vector2>
            {
                new Vector2(rectangle.X, rectangle.Y),
                new Vector2(rectangle.X + rectangle.Width, rectangle.Y),
                new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height),
                new Vector2(rectangle.X, rectangle.Y + rectangle.Height)
            };

            return new Polygon(cornerPoints, false, false);
        }

       /* public override CollidableResults CheckForCircleCollision(Circle circle, Vector3 velocity)
        {
            var result = new CollidableResults
            {
                Intersect = true,
                WillIntersect = true
            };

           
            float radiusSquared = circle.Radius * circle.Radius;
            Vector2 vertex = Points.Last();
            float nearestDistance = Single.MaxValue;
            bool nearestIsInside = false;
            int nearestVertex = -1;
            bool lastIsInside = false;

            for (int i = 0; i < Points.Count; i++)
            {
                Vector2 nextVertex = Points[i];

                Vector2 axis = circle.Center - vertex;

                //check if the circle is within radius rr of each vertex 
                float distance = axis.LengthSquared() - radiusSquared;

                if (distance <= 0)
                {
                    result.Intersect = true;
                    break;
                }

                //Otherwise check if the circle's centre is within the Voroni edge region of the polygon
                bool isInside = false;

                Vector2 edge = Edges[i];//TODO:check this

                float edgeLengthSquared = edge.LengthSquared();

                if (edgeLengthSquared != 0)
                {
                    float dot = Vector2.Dot(edge, axis);

                    if (dot >= 0 && dot <= edgeLengthSquared)
                    {
                        Vector2 projection = vertex + (dot / edgeLengthSquared) * edge;

                        axis = projection - circle.Center;

                        if (axis.LengthSquared() <= radiusSquared)
                        {
                            result.Intersect = true;
                            break;
                        }
                        else
                        {
                            if (edge.X > 0)
                            {
                                if (axis.Y > 0)
                                {
                                    result.Intersect = false;
                                    break;
                                }
                            }
                            else if (edge.X < 0)
                            {
                                if (axis.Y < 0)
                                {
                                    result.Intersect = false;
                                    break;
                                    
                                }
                            }
                            else if (edge.Y > 0)
                            {
                                if (axis.X < 0)
                                {
                                    result.Intersect = false;
                                    break;
                                }
                            }
                            else
                            {
                                if (axis.X > 0)
                                {
                                    result.Intersect = false;
                                    break;
                                }
                            }

                            isInside = true;
                        }
                    }
                }

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestIsInside = isInside || lastIsInside;
                    nearestVertex = i;
                }

                vertex = nextVertex;
                lastIsInside = isInside;
            }

            if (nearestVertex == 0)
            {
                result.Intersect =  nearestIsInside || lastIsInside;
            }
            else
            {
                result.Intersect =  nearestIsInside;
            }

        
            return result;

        }*/
    }
}
