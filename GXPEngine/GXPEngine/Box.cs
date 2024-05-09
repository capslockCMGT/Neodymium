using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;

namespace GXPEngine
{
    public class Box : GameObject
    {
        private static BufferRenderer boxModel;
        protected static Vector3[] _bounds;
        protected Texture2D _texture;

        private uint _color = 0xFFFFFF;

        public bool pixelated = Game.main.PixelArt;

        /// <summary>
		/// Initializes a new instance of the <see cref="GXPEngine.Box"/> class.
		/// Specify a System.Drawing.Bitmap to use. Bitmaps will not be cached.
		/// </summary>
		/// <param name='bitmap'>
		/// Bitmap.
		/// </param>
		/// <param name="addCollider">
		/// If <c>true</c>, this box will have a collider that will be added to the collision manager.
		/// </param> 
		public Box(System.Drawing.Bitmap bitmap, bool addCollider = true) : base(addCollider)
        {
            if (Game.main == null)
            {
                throw new Exception("Boxes cannot be created before creating a Game instance.");
            }
            name = "BMP" + bitmap.Width + "x" + bitmap.Height;
            initializeFromTexture(new Texture2D(bitmap));
        }

        public Box(Texture2D texture, bool addCollider = true) : base(addCollider)
        {
            if (Game.main == null)
            {
                throw new Exception("Boxes cannot be created before creating a Game instance.");
            }
            name = "Box from " + texture.filename;
            initializeFromTexture(texture);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Box()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="GXPEngine.Box"/> class.
        /// Specify an image file to load. Please use a full filename. Initial path is the application folder.
        /// Images will be cached internally. That means once it is loaded, the same data will be used when
        /// you load the file again.
        /// </summary>
        /// <param name='filename'>
        /// The name of the file that should be loaded.
        /// </param>
        /// <param name="keepInCache">
        /// If <c>true</c>, the box's texture will be kept in memory for the entire lifetime of the game. 
        /// This takes up more memory, but removes load times.
        /// </param> 
        /// <param name="addCollider">
        /// If <c>true</c>, this box will have a collider that will be added to the collision manager.
        /// </param> 
        public Box(string filename, bool keepInCache = true, bool addCollider = true) : base(addCollider)
        {
            if (Game.main == null)
            {
                throw new Exception("Boxes cannot be created before creating a Game instance.");
            }
            name = filename;
            initializeFromTexture(Texture2D.GetInstance(filename, keepInCache));
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														OnDestroy()
        //------------------------------------------------------------------------------------------------------------------------
        protected override void OnDestroy()
        {
            if (_texture != null) _texture.Dispose();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														createCollider
        //------------------------------------------------------------------------------------------------------------------------
        protected override Collider createCollider()
        {
            return new BoxCollider(this);
        }


        //------------------------------------------------------------------------------------------------------------------------
        //														initializeFromTexture()
        //------------------------------------------------------------------------------------------------------------------------
        protected void initializeFromTexture(Texture2D texture)
        {
            _texture = texture;
            if (boxModel != null) return;
            boxModel = new BufferRenderer();
            _bounds = new Vector3[8];
            for (int i = 0; i < 6; i++)
            {
                float third = 1 / 3f;
                float half = .5f;
                int sel = i % 3;
                int dir = i>2 ? 1 : -1;
                float dir01 = dir * .5f + .5f;
                Vector3 normal = new Vector3(sel == 1?dir:0, sel==0?dir:0, sel==2?dir:0);
                Vector3 x = new Vector3(sel == 2?1:0, 0, sel!=2?1:0);
                Vector3 y = new Vector3(sel == 0?1:0, sel!=0?1:0, 0);
                Vector3 res; 
                res = normal - x - y;
                boxModel.AddVert(res.x, res.y, res.z);
                boxModel.AddUv(third * (1 + sel), half * (1 + dir01));
                boxModel.AddNormal(normal.x, normal.y, normal.z);
                if (sel == 1) _bounds[(dir == 1 ? 1 : 0) * 4 + 0] = res;
                res = normal + x - y;
                boxModel.AddVert(res.x, res.y, res.z);
                boxModel.AddUv(third * sel, half * (1 + dir01));
                boxModel.AddNormal(normal.x, normal.y, normal.z);
                if (sel == 1) _bounds[(dir == 1 ? 1 : 0) * 4 + 1] = res;
                res = normal + x + y;
                boxModel.AddVert(res.x, res.y, res.z);
                boxModel.AddUv(third * sel, half * dir01);
                boxModel.AddNormal(normal.x, normal.y, normal.z);
                if (sel == 1) _bounds[(dir == 1 ? 1 : 0) * 4 + 2] = res;
                res = normal - x + y;
                boxModel.AddVert(res.x, res.y, res.z);
                boxModel.AddUv(third * (1 + sel), half * dir01);
                boxModel.AddNormal(normal.x, normal.y, normal.z);
                if (sel == 1) _bounds[(dir == 1 ? 1 : 0) * 4 + 3] = res;
            }
            boxModel.CreateBuffers();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														texture
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the texture that is used to create this box.
        /// If no texture is used, null will be returned.
        /// Use this to retreive the original width/height or filename of the texture.
        /// </summary>
        public Texture2D texture
        {
            get { return _texture; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														RenderSelf()
        //------------------------------------------------------------------------------------------------------------------------
        override protected void RenderSelf(GLContext glContext)
        {
            if (game != null)
            {

                if (OnScreen())
                {
                    boxModel.texture = _texture;
                    boxModel.pixelated = pixelated;
                    glContext.SetColor((byte)((_color >> 16) & 0xFF),
                                       (byte)((_color >> 8) & 0xFF),
                                       (byte)(_color & 0xFF),
                                       (byte)(0xFF));
                    boxModel.DrawBuffers(glContext);
                    glContext.SetColor(255, 255, 255, 255);
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														color
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the color filter for this sprite.
        /// This can be any value between 0x000000 and 0xFFFFFF.
        /// </summary>
        public uint color
        {
            get { return _color; }
            set { _color = value & 0xFFFFFF; }
        }

        public void DisplayVertices()
        {
            boxModel.WriteVertsToConsole();
        }

        public override Vector3[] GetExtents()
        {
            Vector3[] res = new Vector3[8];
            for (int i = 0; i < 8; i++)
                res[i] = TransformPoint(_bounds[i]);
            return res;
        }

        public void DisplayExtents()
        {
            foreach (var vec in GetExtents())
            {
                Console.WriteLine(vec);
            }
        }
        // TODO: fix this.
        //------------------------------------------------------------------------------------------------------------------------
        //														OnScreen
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Painfully, this is NOT as trivial in 3D as in 2D. 
        /// Returns True for the time being.
        /// </summary>
        protected bool OnScreen()
        {
            /*
			Vector2[] bounds = GetExtents();
			float maxX = float.MinValue;
			float maxY = float.MinValue;
			float minX = float.MaxValue;
			float minY = float.MaxValue;
			for (int i=0; i<4; i++) {
				if (bounds[i].x > maxX) maxX = bounds[i].x;
				if (bounds[i].x < minX) minX = bounds[i].x;
				if (bounds[i].y > maxY) maxY = bounds[i].y;
				if (bounds[i].y < minY) minY = bounds[i].y;
			}
			return !( (maxX < game.RenderRange.left) || (maxY < game.RenderRange.top) || (minX >= game.RenderRange.right) || (minY >= game.RenderRange.bottom));
			*/
            return true;
        }
    }
}
