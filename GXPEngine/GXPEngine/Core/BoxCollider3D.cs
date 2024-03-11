using System;
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
    /// <summary>
    /// Deprecated artefact from original GXP. Don't use this.
    /// </summary>
    public class BoxCollider3D : Collider
	{
		private Box _owner;
		
		//------------------------------------------------------------------------------------------------------------------------
		//														BoxCollider()
		//------------------------------------------------------------------------------------------------------------------------		
		public BoxCollider3D(Box owner) {
			_owner = owner;
		}


		//------------------------------------------------------------------------------------------------------------------------
		//														HitTest()
		//------------------------------------------------------------------------------------------------------------------------		
		public override bool HitTest (Collider other) {
			if (other is BoxCollider3D) {
				Vector3[] c = _owner.GetExtents();
				if (c == null) return false;
				Vector3[] d = ((BoxCollider3D)other)._owner.GetExtents();
				if (d == null) return false;
				if (!areaOverlap(c, d)) return false;
				return areaOverlap(d, c);
			}	
            else {
				return false;
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														HitTestPoint()
		//------------------------------------------------------------------------------------------------------------------------		
		public override bool HitTestPoint (float x, float y, float z) {
			Vector3[] c = _owner.GetExtents();
			if (c == null) return false;
			Vector3 p = new Vector3(x, y, z);
			return pointOverlapsArea(p, c);
        }
        //------------------------------------------------------------------------------------------------------------------------
        //														HitTestLine()
        //------------------------------------------------------------------------------------------------------------------------		
        public override bool RayCastTest(Vector3 p1, Vector3 p2)
        {
            Vector3[] c = _owner.GetExtents();
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
			Vector3 n = (d1 ^ d2).normalized();
			if (n * a == 0)
				return new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			float d = (p0 - l0) * n / (a * n);
            Vector3 p = l0 + a * d;
            Vector3 xraw = d1 ^ (p - p0);
            if (xraw * n > 1 || xraw * n < 0)
				return new Vector3(float.MaxValue, float.MaxValue, float.MaxValue); ;
            Vector3 yraw = - d2 ^ (p - p0);
            if (yraw * n > 1 || yraw * n < 0)
                return new Vector3(float.MaxValue, float.MaxValue, float.MaxValue); ;

            //Gizmos.DrawPlus(p.x, p.y, p.z, 0.05f, color: 0xff0044ff);
            //Gizmos.DrawLine(p.x, p.y, p.z, p.x + n.x * 0.1f, p.y + n.y * 0.1f, p.z + n.z * 0.1f, color: 0xff00ff00);
            if (d < distance)
			{
				Console.WriteLine(d);
                distance = d;
                normal = n;
				change = true;
            }
            return p;
        }
        //------------------------------------------------------------------------------------------------------------------------
        //														RayCast()
        //------------------------------------------------------------------------------------------------------------------------		
        public bool RayCast(Vector3 p1, Vector3 p2)
        {
            Vector3[] c = _owner.GetExtents();
            if (c == null) return false;
            if (!RayCastTest(p1, p2))
                return false;
            Vector3 d1 = c[1] - c[0];
            Vector3 d2 = c[3] - c[0];
            Vector3 d3 = c[4] - c[0];
			Vector3 a = p2 - p1;
			float distance = float.MaxValue;
			Vector3 normal = Vector3.zero;
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

            Gizmos.DrawPlus(minIntersection.x, minIntersection.y, minIntersection.z, 0.05f, color: 0xff0044ff);
            Gizmos.DrawLine(minIntersection.x, minIntersection.y, minIntersection.z, 
							minIntersection.x + normal.x * 0.1f, minIntersection.y + normal.y * 0.1f, minIntersection.z + normal.z * 0.1f, color: 0xff00ff00);
			if (distance == float.MaxValue)
				return false;
            return true;
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
            float t2 = ((d[0] - c[0]) * (d3 ^ d1)) / tripleProd;
            float maxT = t;
            float minT = t;

            for (int i = 1; i < 8; i++)
            {
                t = ((d[0] - c[0]) * (d1 ^ d2)) / tripleProd;
                t2 = ((d[0] - c[0]) * (d3 ^ d1)) / tripleProd;
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
			Gizmos.DrawParallelogon(res);
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
            Gizmos.DrawPlus(lineProj.x, lineProj.y, lineProj.z, 0.05f, color: 0xffff00ff);

            Gizmos.DrawPlus(proj.a0.x, proj.a0.y, proj.a0.z, 0.05f, color: 0xffff00aa);
            Gizmos.DrawPlus(b0.x, b0.y, b0.z, 0.05f, color: 0xffff0077);
            Gizmos.DrawPlus(c0.x, c0.y, c0.z, 0.05f, color: 0xffff0033);

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
			if (other is BoxCollider3D) {
				Vector3[] c = _owner.GetExtents();
				if (c == null) return float.MaxValue;
				Vector3[] d = ((BoxCollider3D)other)._owner.GetExtents();
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
			if (other is BoxCollider3D) {
				//Console.WriteLine ("\n\n===== Computing collision data:\n");
				Vector3[] c = _owner.GetExtents();
				if (c == null) return null;
				Vector3[] d = ((BoxCollider3D)other)._owner.GetExtents();
				if (d == null) return null;

				//Console.WriteLine ("\nSide vectors of this:\n {0},{1} and {2},{3}",
				//	c[1].x-c[0].x,c[1].y-c[0].y,c[3].x-c[0].x,c[3].y-c[0].y
				//);

				// normals of this vs points of other:
				float nx = -c [0].y + c [1].y;
				float ny = -c [1].x + c [0].x;
				if (!updateCollisionPoint (
					    c [0].x, c [0].y, nx, ny, 
					    c [3].x - c [0].x, c [3].y - c [0].y, d,
					    true, ref penetrationDepth, ref normal, ref point))
					return null;

				nx = c [0].y - c [3].y;
				ny = c [3].x - c [0].x;
				if (!updateCollisionPoint (
					c [0].x, c [0].y, nx, ny, 
					c [1].x - c [0].x, c [1].y - c [0].y, d, 
					true, ref penetrationDepth, ref normal, ref point))
					return null;

				//Console.WriteLine ("\nSide vectors of other:\n {0},{1} and {2},{3}",
				//	d[1].x-d[0].x,d[1].y-d[0].y,d[3].x-d[0].x,d[3].y-d[0].y
				//);
				// normals of other vs points of this:
				nx = -d [0].y + d [1].y;
				ny = -d [1].x + d [0].x;
				if (!updateCollisionPoint (
					d [0].x, d [0].y, nx, ny, 
					d [3].x - d [0].x, d [3].y - d [0].y, c, 
					false, ref penetrationDepth, ref normal, ref point))
					return null;

				nx = d [0].y - d [3].y;
				ny = d [3].x - d [0].x;
				if (!updateCollisionPoint (
					d [0].x, d [0].y, nx, ny, 
					d [1].x - d [0].x, d [1].y - d [0].y, c, 
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
				return new Collision(_owner, ((BoxCollider3D)other)._owner, new Vector3(normal.x, normal.y, 0), new Vector3(point.x, point.y, 0), penetrationDepth);
			} else {
				return null;
			}
		}
	
		private bool updateCollisionPoint(
			float cx, float cy, float nx, float ny, float dx, float dy, Vector3[] d, bool invertNormal,
			ref float minPenetrationDepth, ref Vector3 normal, ref Vector3 point
		) {
			float dot = (dy * ny + dx * nx);

			if (dot == 0.0f) dot = 1.0f; // hm

			Vector3 argMin=new Vector3();
			Vector3 argMax=new Vector3();
			float minT=float.MaxValue;
			float maxT=float.MinValue;
			for (int i = 0; i < d.Length; i++) {
				float t = ((d [i].x - cx) * nx + (d [i].y - cy) * ny) / dot;
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
			float lenD = Mathf.Sqrt (dx * dx + dy * dy);

			//Console.WriteLine ("\n  considering normal: {0},{1}\n  minT, maxT: {2},{3}\n  intersection candidates: {4},{5}",
			//	nx,ny,minT,maxT,(1-minT)*lenD,maxT*lenD
			//);
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
				float len = invertNormal ? -Mathf.Sqrt (nx * nx + ny * ny) : Mathf.Sqrt (nx * nx + ny * ny);
				normal.x = nx / len;
				normal.y = ny / len;
				//Console.WriteLine ("NEW BEST");
			} else {
				//Console.WriteLine ("NO UPDATE");
			}
			//Console.WriteLine (" (check:) best normal: "+normal);
			return true;
		}
	}
}


