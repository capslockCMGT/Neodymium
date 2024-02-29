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
		
		override public string ToString() {
			return "[Vector2 " + x + ", " + y + ", " + z + "]";
		}
	}
}

