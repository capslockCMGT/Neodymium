using System;

namespace GXPEngine.Core
{
	public struct Rectangle
	{
		public float x, y, width, height;
		
		//------------------------------------------------------------------------------------------------------------------------
		//														Rectangle()
		//------------------------------------------------------------------------------------------------------------------------
		public Rectangle (float x, float y, float width, float height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														Properties()
		//------------------------------------------------------------------------------------------------------------------------
		public float left { get { return x; } }
		public float right { get { return x + width; } }
		public float top { get { return y; } }
		public float bottom { get { return y + height; } }

		//------------------------------------------------------------------------------------------------------------------------
		//														ToString()
		//------------------------------------------------------------------------------------------------------------------------
		override public string ToString() {
			return (x + "," + y + "," + width + "," + height);
		}
		
		public static Rectangle Intersection(Rectangle r1, Rectangle r2)
		{
			float minLeft = Mathf.Min(r1.left, r2.left);
			float minTop = Mathf.Min(r1.top, r2.top);
			float width = Mathf.Min(r1.right, r2.right) - minLeft;
            float height = Mathf.Min(r1.bottom, r2.bottom) - minTop;
            return new Rectangle(
				minLeft, minTop,width,height
                );
		}
	}
}

