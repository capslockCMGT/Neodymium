using System;

namespace GXPEngine.Core
{
	public struct Vector3
	{
		public float x;
		public float y;
		public float z;
		
		public Vector3 (float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
        public static Vector3 zero = new Vector3(0,0,0);
        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }
        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }
        public static Vector3 operator -(Vector3 v)
        {
            return new Vector3(-v.x, -v.y, -v.z);
        }
        public static Vector3 operator *(Vector3 v, float s)
        {
            return new Vector3(v.x * s, v.y * s, v.z * s);
        }
        public static Vector3 operator *(float s, Vector3 v)
        {
            return new Vector3(v.x * s, v.y * s, v.z * s);
        }
        public static bool operator ==(Vector3 v1, Vector3 v2)
        {
            return (v1.x - v2.x < 0.00001f && v1.y - v2.y < 0.00001f && v1.z - v2.z < 0.00001f);
        }
        public static bool operator !=(Vector3 v1, Vector3 v2)
        {
            return !(v1.x - v2.x < 0.00001f && v1.y - v2.y < 0.00001f && v1.z - v2.z < 0.00001f);
        }
        //alternative dot product
        public static float operator *(Vector3 v1, Vector3 v2)
        {
            return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        }

        //alternative cross product
        public static Vector3 operator ^ (Vector3 v1, Vector3 v2)
        {
            return new Vector3(
                v1.y * v2.z - v1.z * v2.y,
                v1.z * v2.x - v1.x * v2.z,
                v1.x * v2.y - v1.y * v2.x
                );
        }
        public static Vector3 operator /(Vector3 v, float s)
        {
			s = 1 / s;
			return v * s;
        }

        public float dot(Vector3 v)
		{
			return x*v.x + y*v.y + z*v.z;
		}

		public Vector3 cross(Vector3 v)
		{
			return new Vector3(
				y*v.z-z*v.y, 
				z*v.x-x*v.z,
				x*v.y-y*v.x
				);
		}

		public float MagnitudeSquared()
		{
			return x*x + y*y + z*z;
		}

		public float Magnitude()
		{
			return Mathf.Sqrt(MagnitudeSquared());
		}

		public static Vector3 normalize(Vector3 v)
		{
			return v/v.Magnitude();
		}
        public void Normalize()
        {
            this /= Magnitude();
        }

        public Vector3 normalized()
        {
            return this / Magnitude();
        }

        public static Vector3 Lerp(float fac, Vector3 min, Vector3 max)
        {
            return new Vector3(
                Mathf.Lerp(fac, min.x, max.x),
                Mathf.Lerp(fac, min.y, max.y),
                Mathf.Lerp(fac, min.z, max.z)
                );
        }
        override public string ToString() {
			return "[Vector2 " + x + ", " + y + ", " + z + "]";
		}
	}
}

