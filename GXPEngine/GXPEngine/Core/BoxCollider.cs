using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;

namespace GXPEngine.Core
{
	public struct Parallelogon
	{
		public Vector3 a0, a, b, c;
        public override string ToString()
        {
            return a + ", " + b + ", " + c + ", " + a0;
        }
	}
	//public struct CollisionManifold
	//{
	//	Vector3[] collisionPoints;
	//}
    /// <summary>
    /// Deprecated artefact from original GXP. Don't use this.
    /// </summary>
    public class BoxCollider : Collider
	{
		private GameObject _owner;
		private Vector3 _size = Vector3.zero;
        private Vector3 _offset = Vector3.zero;
        public Vector3 size
		{
			get { return _size; }
			set { _size = value; }
        }
        public Vector3 offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														BoxCollider()
        //------------------------------------------------------------------------------------------------------------------------		
        public BoxCollider(GameObject owner) {
			_owner = owner;
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														GetExtents()
		//------------------------------------------------------------------------------------------------------------------------		
		public Vector3[] GetExtents()
		{
			if (_size.x == 0) _size.x = 1;
            if (_size.y == 0) _size.y = 1;
            if (_size.z == 0) _size.z = 1;
			Vector3[] result = new Vector3[]
			{
				new Vector3(-_size.x,-_size.y,-_size.z),
				new Vector3(-_size.x,-_size.y, _size.z),
				new Vector3(-_size.x, _size.y, _size.z),
				new Vector3(-_size.x, _size.y,-_size.z),
				new Vector3( _size.x,-_size.y,-_size.z),
				new Vector3( _size.x,-_size.y, _size.z),
				new Vector3( _size.x, _size.y, _size.z),
				new Vector3( _size.x, _size.y,-_size.z),
			};
            for (int i = 0; i < 8; i++)
            {
				result[i] += offset;
                result[i] = _owner.TransformPoint(result[i]);
            }
			return result;
        }
        //------------------------------------------------------------------------------------------------------------------------
        //														DrawExtents()
        //------------------------------------------------------------------------------------------------------------------------		
        public void DrawExtents()
        {
			Gizmos.DrawBox(offset, 2*size, _owner);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														HitTest()
        //------------------------------------------------------------------------------------------------------------------------		
        public override bool HitTest (Collider other) {
			if (other is BoxCollider) {
				Vector3[] c = GetExtents();
				if (c == null) return false;
				Vector3[] d = ((BoxCollider)other).GetExtents();
				if (d == null) return false;
                if (!areaOverlap(c, d)) return false; 
				if (!areaOverlap(d, c)) return false;
                return CheckEdgeCollisions(c, d);
            }	
            else {
				return false;
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														HitTestPoint()
		//------------------------------------------------------------------------------------------------------------------------		
		public override bool HitTestPoint (float x, float y, float z) {
			Vector3[] c = GetExtents();
			if (c == null) return false;
			Vector3 p = new Vector3(x, y, z);
			return pointOverlapsArea(p, c);
        }
        //------------------------------------------------------------------------------------------------------------------------
        //														HitTestLine()
        //------------------------------------------------------------------------------------------------------------------------		
        public override bool RayCastTest(Vector3 p1, Vector3 p2)
        {
            Vector3[] c = GetExtents();
            if (c == null) return false;
            return lineOverlapsArea(p1, p2, c);
        }
        //------------------------------------------------------------------------------------------------------------------------
        //														RayCast()
        //------------------------------------------------------------------------------------------------------------------------		

        /// <summary>
        /// <para>
        /// Calculates coordinates of the intersection point between the line and a plane set by a parallelogram in parallelogram coordinates. 
        /// </para>
        /// <para>
        /// </para>
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="p"></param>
        /// <param name="a"></param>
        /// <returns>
		/// <para>Returns coordinates of the intersection point in world space</para>
        /// <para>Changes <paramref name="distance"/> from the point l0 to hit AND <paramref name="normal"/> of the plane
		/// if the distance is lesser than initial and the intersection point is within the parallelogram.</para>
		/// <para>Changes are monitored with <paramref name="change"/></para>
		/// </returns>
        public Vector3 lineVSPara(Vector3 p0, Vector3 d1, Vector3 d2, Vector3 l0, Vector3 a, ref float distance, ref Vector3 normal, out bool change)
        {
            //Gizmos.DrawParallelogram(p0, d1, d2);
            change = false;
			a.Normalize();
			Vector3 n = (d1 ^ d2);
			if (n * a == 0)
				return new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			float d = (p0 - l0) * n / (a * n);
			if (d<0)
                return new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 p = l0 + a * d;
            Vector3 xraw = d1 ^ (p - p0);
            if (xraw * n > n * n || xraw * n < 0)
				return new Vector3(float.MaxValue, float.MaxValue, float.MaxValue); ;
            Vector3 yraw = - d2 ^ (p - p0);
            if (yraw * n > n * n || yraw * n < 0)
                return new Vector3(float.MaxValue, float.MaxValue, float.MaxValue); ;

            //Gizmos.DrawPlus(p.x, p.y, p.z, 0.05f, color: 0xff0044ff);
            //Gizmos.DrawLine(p.x, p.y, p.z, p.x + n.x * 0.1f, p.y + n.y * 0.1f, p.z + n.z * 0.1f, color: 0xff00ff00);
            if (d < distance)
			{
				//Console.WriteLine(d);
                distance = d;
                normal = n;
				change = true;
            }
            return p;
        }
        //------------------------------------------------------------------------------------------------------------------------
        //														RayCast()
        //------------------------------------------------------------------------------------------------------------------------		
        public override bool RayCast(Vector3 p1, Vector3 p2, out float distance, out Vector3 normal, GameObject worldSpace = null)
        {
			if (worldSpace == null)
				worldSpace = Game.main;
            //p1 = worldSpace.TransformPoint(p1);
            //p2 = worldSpace.TransformPoint(p2);
            //p1 = _owner.InverseTransformPoint(p1);
            //p2 = _owner.InverseTransformPoint(p2);
            distance = float.MaxValue;
            normal = Vector3.zero;

            Vector3[] c = GetExtents();
            if (c == null) return false;
            if (!RayCastTest(p1, p2))
                return false;
            Vector3 d1 = c[1] - c[0];
            Vector3 d2 = c[3] - c[0];
            Vector3 d3 = c[4] - c[0];

			//Gizmos.DrawLine(c[1], c[0], color:0xff55ff55);
            //Gizmos.DrawLine(c[3], c[0], color: 0xff55ff55);
            //Gizmos.DrawLine(c[4], c[0], color: 0xff55ff55);

            Vector3 a = p2 - p1;
			bool change = false;
			Vector3 intersection;

            Vector3 minIntersection = lineVSPara(c[0], d1, d2, p1, a, ref distance, ref normal, out change);
            intersection = lineVSPara(c[0], d2, d3, p1, a, ref distance, ref normal, out change);
			if (change) minIntersection = intersection;
            intersection = lineVSPara(c[0], d3, d1, p1, a, ref distance, ref normal, out change);
            if (change) minIntersection = intersection;

            intersection = lineVSPara(c[6], -d2, -d1, p1, a, ref distance, ref normal, out change);
            if (change) minIntersection = intersection;
            intersection = lineVSPara(c[6], -d3, -d2, p1, a, ref distance, ref normal, out change);
            if (change) minIntersection = intersection;
            intersection = lineVSPara(c[6], -d1, -d3, p1, a, ref distance, ref normal, out change);
            if (change) minIntersection = intersection;
			//Console.WriteLine(minIntersection);

			normal.Normalize();
			//Gizmos.DrawLine(p1, p2);
            //Gizmos.DrawPlus(minIntersection.x, minIntersection.y, minIntersection.z, 0.05f, color: 0xff0044ff);
            //Gizmos.DrawLine(minIntersection.x, minIntersection.y, minIntersection.z, 
			//				minIntersection.x + normal.x * 0.1f, minIntersection.y + normal.y * 0.1f, minIntersection.z + normal.z * 0.1f, color: 0xff00ff00);
			if (distance == float.MaxValue)
				return false;
            return true;
        }
        //------------------------------------------------------------------------------------------------------------------------
        //														boundDistance()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// <para>
        /// Returns the distance to the intersection point from the par-on's origin between the parallelepipedon and a plane with <paramref name="normal"/>
        /// which bounds the given parallelepipedon with dimentions of <paramref name="a"/>, <paramref name="b"/>, <paramref name="c"/>
		/// in that direction
        /// </para>
        /// <para>
        /// Used for detecting edge to edge collisions
        /// </para>
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="O"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public Vector3 boundDistance(Vector3 normal, Vector3 a, Vector3 b, Vector3 c)
		{
			normal.Normalize();
			float dotA = a * normal;
            float dotB = b * normal;
            float dotC = c * normal;
			Vector3 p = 
                ((dotA < 0) ? -a : a) +
                ((dotB < 0) ? -b : b) +
                ((dotC < 0) ? -c : c);
			return p/2;
        }
        //------------------------------------------------------------------------------------------------------------------------
        //														areaOverlap()
        //------------------------------------------------------------------------------------------------------------------------
        private bool areaOverlap(Vector3[] c, Vector3[] d)
        {
            Vector3 d1 = c[1] - c[0];
            Vector3 d2 = c[3] - c[0];
            Vector3 d3 = c[4] - c[0];

            float tripleProd = d1 * (d2 ^ d3);
            // first normal:
            if (tripleProd == 0.0f) tripleProd = 1.0f;

            float t = ((d[0] - c[0]) * (d2 ^ d3)) / tripleProd;
            float maxT = t;
            float minT = t;

            for (int i = 1; i < 8; i++)
            {
                t = ((d[i] - c[0]) * (d1 ^ d2)) / tripleProd;
                minT = Math.Min(minT, t);
                maxT = Math.Max(maxT, t);
            }

            if ((minT >= 1) || (maxT <= 0)) return false;
			//Console.WriteLine("first");
            // second normal:

            t = ((d[0] - c[0]) * (d3 ^ d1)) / tripleProd;
            maxT = t; minT = t;

            for (int i = 1; i < 8; i++)
            {
                t = ((d[i] - c[0]) * (d3 ^ d1)) / tripleProd;
                minT = Math.Min(minT, t);
                maxT = Math.Max(maxT, t);
            }

            if ((minT >= 1) || (maxT <= 0)) return false;
            //Console.WriteLine("second");
            // third normal:

            t = ((d[0] - c[0]) * (d1 ^ d2)) / tripleProd;
            maxT = t; minT = t;

            for (int i = 1; i < 8; i++)
            {
                t = ((d[i] - c[0]) * (d1 ^ d2)) / tripleProd;
                minT = Math.Min(minT, t);
                maxT = Math.Max(maxT, t);
            }

            if ((minT >= 1) || (maxT <= 0)) return false;
			//Console.WriteLine("third");
			return true;
        }

		public bool CheckEdgeCollisions(Vector3[] c, Vector3[] d)
        {
			Vector3[] cEdges = new Vector3[]
				{
					c[1] - c[0],
					c[3] - c[0],
                    c[4] - c[0],
                };

            Vector3[] dEdges = new Vector3[]
                {
                    d[1] - d[0],
                    d[3] - d[0],
                    d[4] - d[0],
                };
            Vector3 O = (c[6] + c[0]) / 2;
			float t, maxT, minT;

			// checking all edge pairs (x1 x2, x1 y2... z1 y2, z1 z2)
			for (int C = 0; C < 3; C++)
			{
				for (int D = 0; D < 3; D++)
				{
					//Console.WriteLine(C * 3 + D);
                    Vector3 normal = (cEdges[C] ^ dEdges[D]).normalized();
                    Vector3 p = boundDistance(normal, cEdges[0], cEdges[1], cEdges[2]);
                    float dot = normal * p;

                    t = ((d[0] - O) * normal) / dot;
                    maxT = t; minT = t;

                    for (int i = 1; i < 8; i++)
                    {
                        t = ((d[i] - O) * normal) / dot;
                        minT = Math.Min(minT, t);
                        maxT = Math.Max(maxT, t);
                    }
					if ((minT >= 1) || (maxT <= -1))
					{
                        //Gizmos.DrawPlus((O).x, (O).y, (O).z, 0.05f, color: 0xffaaff00);
                        //Gizmos.DrawLine(O+p, O+p + cEdges[C], color: 0xffaaff00);
                        //Gizmos.DrawLine(O+p, O+p + dEdges[D], color: 0xffaaff00);
                        return false;
					}

                }
			}
            return true;

        }


		/// <summary>
		/// Creates a projection of the parallelepiped onto a plane with the given normal and encapsulates it as a hexagon (parallelogon)
		/// </summary>
		/// <param name="c"></param>
		/// <param name="normal"></param>
		/// <returns></returns>
		public Parallelogon project(Vector3[] c, Vector3 normal)
		{
			normal.Normalize();
			Parallelogon res = new Parallelogon();
			Vector3 d1 = c[1] - c[0];
			Vector3 d2 = c[3] - c[0];
			Vector3 d3 = c[4] - c[0];
			float proja = (d1 * normal);
            float projb = (d2 * normal);
            float projc = (d3 * normal);
            res.a = d1 - proja * normal;
			res.b = d2 - projb * normal;
			res.c = d3 - projc * normal;
			if (proja < 0)
				res.a = -res.a;
			if (projb > 0)
				res.b = -res.b;
            if (projc < 0)
                res.c = -res.c;

            res.a0 = (c[6] + c[0]) / 2 - (res.a + res.b + res.c)/2;
			//Gizmos.DrawParallelogon(res);
            return res;

		}
        //------------------------------------------------------------------------------------------------------------------------
        //														pointOverlapsArea()
        //------------------------------------------------------------------------------------------------------------------------
        //ie. for hittestpoint and mousedown/up/out/over
        private bool pointOverlapsArea(Vector3 p, Vector3[] c) {
            Vector3 d1 = c[1] - c[0];
            Vector3 d2 = c[3] - c[0];
            Vector3 d3 = c[4] - c[0];
            float tripleProd = d1 * (d2 ^ d3);
			float t;

            //first normal
            t = ((p - c[0]) * (d2 ^ d3)) / tripleProd;
            if ((t > 1) || (t < 0))	return false;
			

            //second normal
            t = ((p - c[0]) * (d3 ^ d1)) / tripleProd;
            if ((t > 1) || (t < 0)) return false;

            //second normal
            t = ((p - c[0]) * (d1 ^ d2)) / tripleProd;
            if ((t > 1) || (t < 0)) return false;
            return true;			
		}


        //------------------------------------------------------------------------------------------------------------------------
        //														lineOverlapsArea()
        //------------------------------------------------------------------------------------------------------------------------
        //ie. for hittestpoint and mousedown/up/out/over
        private bool lineOverlapsArea(Vector3 p1, Vector3 p2, Vector3[] c)
        {
			Vector3 a = (p2 - p1).normalized();
            Parallelogon proj = project(c, a);
			Vector3 lineProj = p1 + ((proj.a0 - p1) * a) * a;
			Vector3 b0 = proj.a0 + proj.a;
			Vector3 c0 = b0 + proj.b;
            //Gizmos.DrawPlus(lineProj.x, lineProj.y, lineProj.z, 0.05f, color: 0xffff00ff);

            //Gizmos.DrawPlus(proj.a0.x, proj.a0.y, proj.a0.z, 0.05f, color: 0xffff00aa);
            //Gizmos.DrawPlus(b0.x, b0.y, b0.z, 0.05f, color: 0xffff0077);
            //Gizmos.DrawPlus(c0.x, c0.y, c0.z, 0.05f, color: 0xffff0033);

            //first normal

            Vector3 d = (proj.b + proj.c);
            float dot =  (proj.a ^ d) * a;
			float t = (proj.a ^ (lineProj - proj.a0)) * a / dot;
            if ((t > 1) || (t < 0)) return false;

			d = (proj.c - proj.a);
			dot = (proj.b ^ d) * a;
			t = (proj.b ^ (lineProj - b0)) * a / dot;
			if ((t > 1) || (t < 0)) return false;

			d = (- proj.a - proj.b);
			dot = (proj.c ^ d) * a;
			t = (proj.c ^ (lineProj - c0)) * a / dot;
			if ((t > 1) || (t < 0)) return false;

			return true;
        }

        public override float TimeOfImpact (Collider other, float vx, float vy, float vz, out Vector3 normal) {
			normal = new Vector3 ();
			if (other is BoxCollider) {
				Vector3[] c = GetExtents();
				if (c == null) return float.MaxValue;
				Vector3[] d = ((BoxCollider)other).GetExtents();
				if (d == null) return float.MaxValue;

				float maxTOI = float.MinValue;
				float minTOE = float.MaxValue;
				// normals of this vs points of other:
				float nx = -c [0].y + c [1].y;
				float ny = -c [1].x + c [0].x;
				if (updateImpactExitTime (
					    c [0].x, c [0].y, nx, ny, 
					    c [3].x - c [0].x, c [3].y - c [0].y, d, -vx, -vy, ref maxTOI, ref minTOE)) {
					normal.x = nx;
					normal.y = ny;
				}
				if (minTOE <= maxTOI || minTOE <= 0)
					return float.MaxValue;
				nx = c [0].y - c [3].y;
				ny = c [3].x - c [0].x;
				if (updateImpactExitTime (
					    c [0].x, c [0].y, nx, ny, 
					    c [1].x - c [0].x, c [1].y - c [0].y, d, -vx, -vy, ref maxTOI, ref minTOE)) {
					normal.x = nx;
					normal.y = ny;
				}
				if (minTOE <= maxTOI || minTOE <= 0)
					return float.MaxValue;

				// normals of other vs points of this:
				nx = -d [0].y + d [1].y;
				ny = -d [1].x + d [0].x;
				if (updateImpactExitTime (
					    d [0].x, d [0].y, nx, ny, 
					    d [3].x - d [0].x, d [3].y - d [0].y, c, vx, vy, ref maxTOI, ref minTOE)) {
					normal.x = nx;
					normal.y = ny;
				}
				if (minTOE <= maxTOI || minTOE <= 0)
					return float.MaxValue;

				nx = d [0].y - d [3].y;
				ny = d [3].x - d [0].x;
				if (updateImpactExitTime (
					    d [0].x, d [0].y, nx, ny, 
					    d [1].x - d [0].x, d [1].y - d [0].y, c, vx, vy, ref maxTOI, ref minTOE)) {
					normal.x = nx;
					normal.y = ny;
				}
				if (minTOE <= maxTOI || minTOE <= 0)
					return float.MaxValue;
				// normalize the normal when there's an actual collision:
				float nLen = Mathf.Sqrt (normal.x * normal.x + normal.y * normal.y);
				normal.x /= nLen;
				normal.y /= nLen;
				if (normal.x * vx + normal.y * vy > 0) {
					normal.x *= -1;
					normal.y *= -1;
				}
				if (maxTOI >= 0)
					return maxTOI;
				// remaining case: maxTOI is negative, minTOE is positive. => currently overlapping
				if (Mathf.Abs (maxTOI) < Mathf.Abs (minTOE)) // only return collision if going towards deeper overlap!
					return 0;
				return float.MaxValue;
			} else {
				return float.MaxValue;
			}
		}


		// cx,cy: corner point of body 1
		// nx,ny: current normal (not necessarily normalized)
		// dx,dy: body vector of body 1 that gives the max depth along this normal
		// d: points of body 2
		// vx,vy: relative velocity of body 2 w. resp. to body 1
		// TOI/TOE: time of impact/exit. Updated when we find better values along this normal.
		//
		// Returns true if the TOI is updated.
		private bool updateImpactExitTime(float cx, float cy, float nx, float ny, float dx, float dy, Vector3[] d, float vx, float vy, ref float maxTOI, ref float minTOE) {
			float dot = (dy * ny + dx * nx);

			if (dot == 0.0f) dot = 1.0f; // hm

			float t, minT, maxT;

			t = ((d[0].x - cx) * nx + (d[0].y - cy) * ny) / dot;
			maxT = t; minT = t;

			t = ((d[1].x - cx) * nx + (d[1].y - cy) * ny) / dot;
			minT = Math.Min(minT,t); maxT = Math.Max(maxT,t);

			t = ((d[2].x - cx) * nx + (d[2].y - cy) * ny) / dot;
			minT = Math.Min(minT,t); maxT = Math.Max(maxT,t);

			t = ((d[3].x - cx) * nx + (d[3].y - cy) * ny) / dot;
			minT = Math.Min(minT,t); maxT = Math.Max(maxT,t);

			// relative velocity:
			float vp = (vx*nx + vy*ny) / dot;

			if (Mathf.Abs(vp)<0.0001f) {
				if (minT >= 1 || maxT < 0) { // no overlap in this direction, ever.
					minTOE = float.MinValue;
					maxTOI = float.MaxValue;
					return true;
				}
			} else {
				float TOI, TOE;
				if (vp > 0) {
					TOI = -maxT / vp;
					TOE = (1 - minT) / vp;
				} else {
					TOE = -maxT / vp;
					TOI = (1 - minT) / vp;
				}
				if (TOE < minTOE) {
					minTOE = TOE;
				}
				if (TOI > maxTOI) {
					maxTOI = TOI; 
					return true;
				}
			}
			return false;
		}


		public override Collision GetCollisionInfo (Collider other) 
		{
			float penetrationDepth = float.MaxValue;
			Vector3 normal=new Vector3();
			Vector3 point=new Vector3();
			if (other is BoxCollider) {
				//Console.WriteLine ("\n\n===== Computing collision data:\n");
				Vector3[] c = GetExtents();
				if (c == null) return null;
				Vector3[] d = ((BoxCollider)other).GetExtents();
				if (d == null) return null;

                Vector3[] cEdges = new Vector3[]
                    {
						c[1] - c[0],
						c[3] - c[0],
						c[4] - c[0],
                    };

                Vector3[] dEdges = new Vector3[]
                    {
						d[1] - d[0],
						d[3] - d[0],
						d[4] - d[0],
                    };
                Vector3 cO = (c[6] + c[0]) / 2;
                Vector3 dO = (d[6] + d[0]) / 2;

				//Console.WriteLine ("\nSide vectors of this:\n {0},{1} and {2},{3}",
				//	c[1].x-c[0].x,c[1].y-c[0].y,c[3].x-c[0].x,c[3].y-c[0].y
				//);

				// normals of this vs points of other:
				Vector3 n = cEdges[0] ^ cEdges[1];
				if (!updateCollisionPoint (
					c [0], n, cEdges[2], d,
					true, ref penetrationDepth, ref normal, ref point))
					return null;

                n = cEdges[1] ^ cEdges[2];
                if (!updateCollisionPoint (
					c [0], n, cEdges[0], d,
					true, ref penetrationDepth, ref normal, ref point))
					return null;

                n = cEdges[2] ^ cEdges[0];
                if (!updateCollisionPoint(
                    c[0], n, cEdges[1], d,
                    true, ref penetrationDepth, ref normal, ref point))
                    return null;

                //Console.WriteLine ("\nSide vectors of other:\n {0},{1} and {2},{3}",
                //	d[1].x-d[0].x,d[1].y-d[0].y,d[3].x-d[0].x,d[3].y-d[0].y
                //);
                // normals of other vs points of this:
                n = dEdges[0] ^ dEdges[1];
                if (!updateCollisionPoint(
                    d[0], n, dEdges[2], c,
                    false, ref penetrationDepth, ref normal, ref point))
                    return null;

                n = dEdges[1] ^ dEdges[2];
                if (!updateCollisionPoint(
                    d[0], n, dEdges[0], c,
                    false, ref penetrationDepth, ref normal, ref point))
                    return null;

                n = dEdges[2] ^ dEdges[0];
                if (!updateCollisionPoint(
                    d[0], n, dEdges[1], c,
                    false, ref penetrationDepth, ref normal, ref point))
                    return null;
				/*
				if (convertToParentSpace && _owner.parent!=null) {
					normal = _owner.parent.InverseTransformPoint (normal.x, normal.y);
					float nLen = Mathf.Sqrt (normal.x * normal.x + normal.y * normal.y);
					normal.x /= nLen;
					normal.y /= nLen;

					point = _owner.parent.InverseTransformPoint (point.x, point.y);
				}
				*/

				//edges vs edges
				for (int C = 0; C < 3; C++)
				{
					for (int D = 0; D < 3; D++)
					{
						float startPenetration = penetrationDepth;
						n = cEdges[C] ^ dEdges[D];
						if (n.MagnitudeSquared() < 0.00001f)
							continue;
						Vector3 p = boundDistance(n, cEdges[0], cEdges[1], cEdges[2]);
                        if (!updateCollisionPoint(
							cO + p, n, - p * 2, d,
							true, ref penetrationDepth, ref normal, ref point))
						{
							//Gizmos.DrawPlus(cO + p, 0.05f, color: 0xff55ffff);
							//Gizmos.DrawLine(cO + p, cO - p, color: 0xff55ffff);
							//Gizmos.DrawLine(cO + p - cEdges[C], cO + p + cEdges[C], color: 0xffaa00ff);
							//Gizmos.DrawLine(cO + p - dEdges[D], cO + p + dEdges[D], color: 0xffaa00ff);
							//Gizmos.DrawLine(cO - p - cEdges[C], cO - p + cEdges[C], color: 0xffaa00ff);
							//Gizmos.DrawLine(cO - p - dEdges[D], cO - p + dEdges[D], color: 0xffaa00ff);
							return null;
						}
						if (penetrationDepth != startPenetration)
                        {
                            Vector3 dirc = (normal * n > 0) ? p : - p;
                            Vector3 dird = boundDistance(-normal, dEdges[0], dEdges[1], dEdges[2]);

                            Vector3 pc = cO + dirc;
							Vector3 pd = dO + dird;

                            Vector3 ec = (dirc * cEdges[C] > 0) ? - cEdges[C] : cEdges[C];
                            Vector3 ed = (dird * dEdges[D] > 0) ? - dEdges[D] : dEdges[D];
							ec.Normalize();
							ed.Normalize();

							point = pc + ((ed ^ normal) * (pd - pc)) * ec;

							//Gizmos.DrawLine(pc, pc + ec, color: 0xffaa00ff, width: 10);
                            //Gizmos.DrawLine(pd, pd + ed, color: 0xffaa00ff, width: 10);
                        }
					}
				}
				//Gizmos.DrawPlus(point, 0.05f, color: 0xff00aa00);
                //Gizmos.DrawLine(point, point - (normal * penetrationDepth), color: 0xff00aaaa);
                //Console.WriteLine(penetrationDepth);
                return new Collision(_owner, ((BoxCollider)other)._owner, normal, point, penetrationDepth);
			} else {
				return null;
			}
		}
	
		private bool updateCollisionPoint(
			Vector3 c, Vector3 n, Vector3 a, Vector3[] d, bool invertNormal,
			ref float minPenetrationDepth, ref Vector3 normal, ref Vector3 point
		) {
			float dot = a * n;

			if (dot == 0.0f) dot = 1.0f; // hm

			Vector3 argMin=new Vector3();
			Vector3 argMax=new Vector3();
			float minT=float.MaxValue;
			float maxT=float.MinValue;
			for (int i = 0; i < d.Length; i++) {
				float t = (d [i] - c) * n / dot;
				if (t < minT) {
					minT = t;
					argMin = d [i];
				}
				if (t > maxT) {
					maxT = t;
					argMax = d [i];
				}
			}
			// Two cases where no collision:
			if (maxT < 0)
				return false;
			if (minT > 1)
				return false;
			bool updateNormal = false;
			float lenD = a.Magnitude();

			if (lenD == 0)
				lenD = 1; // hm
			if (maxT*lenD < minPenetrationDepth) {
				minPenetrationDepth = maxT*lenD;
				updateNormal = true;
				point = argMax;
			}
			if ((1 - minT)*lenD < minPenetrationDepth) {
				minPenetrationDepth = (1 - minT)*lenD;
				updateNormal = true;
				point = argMin;
				invertNormal = !invertNormal;
			}
			if (updateNormal) {
				normal = invertNormal ? -n.normalized() : n.normalized();
				//Console.WriteLine ("NEW BEST");
			} else {
				//Console.WriteLine ("NO UPDATE");
			}
			//Console.WriteLine (" (check:) best normal: "+normal);
			return true;
		}

        public override float GetArea()
        {
            if (_size.x == 0) _size.x = 1;
            if (_size.y == 0) _size.y = 1;
            if (_size.z == 0) _size.z = 1;
            return _owner.scaleX * _owner.scaleY * _owner.scaleZ
				 * _size.x * _size.y * _size.z * 8;
        }
    }
}


