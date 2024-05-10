using GXPEngine.OpenGL;
using GXPEngine.Core;
using GXPEngine.UI;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using GXPEngine.AddOns;

namespace GXPEngine {
	/// <summary>
	/// A class that can be used to create "sub windows" (e.g. mini-map, splitscreen, etc).
	/// This is not a gameobject. Instead, subscribe the RenderWindow method to the main game's 
	/// OnAfterRender event.
	/// </summary>
	public class Window {
		/// <summary>
		/// The x coordinate of the window's left side
		/// </summary>
		public int windowX {
			get {
				return _windowX;
			}
			set {
				_windowX = value;
				_dirty = true;
			}
		}
		/// <summary>
		/// The y coordinate of the window's top
		/// </summary>
		public int windowY {
			get {
				return _windowY;
			}
			set {
				_windowY = value;
				_dirty = true;
			}
		}
		/// <summary>
		/// The x coordinate of the window center
		/// </summary>
		public float centerX {
			get {
				return _windowX + _width / 2f;
			}
		}
		/// <summary>
		/// The y coordinate of the window center
		/// </summary>
		public float centerY {
			get {
				return _windowY + _height / 2f;
			}
		}
		/// <summary>
		/// The window's width
		/// </summary>
		public int width {
			get {
				return _width;
			}
			set {
				_width = value;
				_dirty = true;
			}
		}
		/// <summary>
		/// The window's height
		/// </summary>
		public int height {
			get {
				return _height;
			}
			set {
				_height = value;
				_dirty = true;
			}
		}

		/// <summary>
		/// The game object (which should be in the hierarchy!) that determines the focus point, rotation and scale
		/// of the viewport window.
		/// </summary>
		public GameObject camera;

		// private variables:
		int _windowX, _windowY;
		int _width, _height;
		bool _dirty=true;
		bool _clear;

		public Transformable window;
		public delegate void RenderCall(GLContext glContext);
		public RenderCall onAfterDepthSortedRender = null;
		public RenderCall onBeforeRenderAnything = null;
		public RenderCall onRenderTransparent = null;

		public static Window ActiveWindow
		{
			get { return _ActiveWindow; }
		}
		private static Window _ActiveWindow = null;
		private List<GameObjAndPos> depthSortedObjects = new List<GameObjAndPos>();

		private class GameObjAndPos
		{
			public GameObject obj;
			public Vector3 pos;
			public GameObjAndPos(GameObject obj, Vector3 pos)
			{
				this.obj = obj;
				this.pos = pos;
			}
		}
		public void RegisterDepthSorted(GameObject toRegister)
		{
			toRegister.registeredDepthSorted = true;

            foreach (GameObjAndPos obj in depthSortedObjects)
			{
				if (obj.obj != toRegister) continue;
				return;
			}
			depthSortedObjects.Add(new GameObjAndPos(toRegister, new Vector3()));
        }
		/// <summary>
		/// Creates a render window in the rectangle given by x,y,width,height.
		/// The camera determines the focal point, rotation and scale of this window.
		/// </summary>
        public Window(int x, int y, int width, int height, GameObject camera, bool clearBackground=true) {
			_windowX = x;
			_windowY = y;
			_width = width;
			_height = height;
			this.camera = camera;
			_clear = clearBackground;
			window = new Transformable ();
		}

		/// <summary>
		/// To render the scene in this window, subscribe this method to the main game's OnAfterRender event.
		/// </summary>
		public void RenderWindow(GLContext glContext)
        {
            _ActiveWindow = this;

			if (_clear && camera.InHierarchy()) GL.Clear(GL.COLOR_BUFFER_BIT);

            if (_dirty) {
				window.x = _windowX + _width / 2;
				window.y = _windowY + _height / 2;
				_dirty = false;
            }
            onBeforeRenderAnything?.Invoke(glContext);
			GL.MatrixMode(GL.PROJECTION);
            glContext.PushMatrix (window.matrix);


            if (camera is Camera)
                glContext.PushMatrix(((Camera)camera).projection.matrix);

            int pushes = 1;
			GameObject current = camera;
			Transformable cameraInverse;
			while (true) {
				cameraInverse = current.Inverse ();
				glContext.PushMatrix (cameraInverse.matrix);
				pushes++;
				if (current.parent == null)
					break;
				current = current.parent;
			}
			GL.MatrixMode(GL.MODELVIEW);
			if (current is Game)
			{// otherwise, the camera is not in the scene hierarchy, so render nothing - not even a black background
				Game main =Game.main;
				
				var oldRange = main.RenderRange;
				SetRenderRange();
				main.SetViewport (_windowX, _windowY, _width, _height, false);

                current.Render (glContext);

                onRenderTransparent?.Invoke(glContext);
				onRenderTransparent = null;
				main.SetViewport ((int)oldRange.left, (int)oldRange.top, (int)oldRange.width, (int)oldRange.height);
            }
			GL.MatrixMode(GL.PROJECTION);
			for (int i=1; i<pushes; i++) {
				glContext.PopMatrix ();
            }

            if (camera is Camera)
                glContext.PopMatrix();
            
            glContext.PopMatrix();
			GL.MatrixMode (GL.MODELVIEW);
			if (current is Game)
            {
				RenderDepthSortedObjects(glContext);
				onAfterDepthSortedRender?.Invoke(glContext);
				RenderUI(glContext);
            }
            _ActiveWindow = null;
        }

		void RenderDepthSortedObjects(GLContext glContext)
        {
            for (int i = depthSortedObjects.Count-1; i > -1; i--)
            {
				if (!depthSortedObjects[i].obj.registeredDepthSorted || !depthSortedObjects[i].obj.InHierarchy() ) depthSortedObjects.RemoveAt(i);
				else
				{ 
					depthSortedObjects[i].pos = ((Camera)camera).GlobalToCameraSpace(depthSortedObjects[i].obj.TransformPoint(0, 0, 0));
					if (depthSortedObjects[i].pos.z < .1f) depthSortedObjects.RemoveAt(i);
                }
            }
            depthSortedObjects.Sort(delegate (GameObjAndPos a, GameObjAndPos b)
            {
                return (b.pos.z).CompareTo(a.pos.z);
            });
            foreach (GameObjAndPos a in depthSortedObjects)
            {
                a.obj.RenderDepthSorted(glContext, a.pos);
            }
        }

		void RenderUI(GLContext glContext)
        {
            /* (i dunno, this is probably hardcode but I cannot think of any other way)
            * 
            * basically mapping from -1..1, -1..1 range of the window
            * to 0..width, 0..height
            */
            glContext.PushMatrix(new float[]
            {
                    2f/_width,0,0,0,
                    0,-2f/_height,0,0,
                    0,0,1,0,
                    -1f,1f,0,1
            });
            Game main = Game.main;
            var oldRange = main.RenderRange;
            SetRenderRange();
            main.SetViewport(_windowX, _windowY, _width, _height, true);
            GL.Clear(GL.DEPTH_BUFFER_BIT);
			GL.Disable(GL.DEPTH_TEST);
            main.uiManager.Render(glContext);
            main.SetViewport((int)oldRange.left, (int)oldRange.top, (int)oldRange.width, (int)oldRange.height, true);
            glContext.PopMatrix();
			GL.Enable(GL.DEPTH_TEST);
        }

		void SetRenderRange() {
			Vector3[] worldSpaceCorners = new Vector3[4];
			worldSpaceCorners[0] = camera.TransformPoint(-_width/2, -_height/2, 0);
			worldSpaceCorners[1] = camera.TransformPoint(-_width/2,  _height/2, 0);
			worldSpaceCorners[2] = camera.TransformPoint( _width/2,  _height/2, 0);
			worldSpaceCorners[3] = camera.TransformPoint( _width/2, -_height/2, 0);

			float maxX = float.MinValue;
			float maxY = float.MinValue;
			float minX = float.MaxValue;
			float minY = float.MaxValue;
			for (int i=0; i<4; i++) {
				if (worldSpaceCorners[i].x > maxX) maxX = worldSpaceCorners[i].x;
				if (worldSpaceCorners[i].x < minX) minX = worldSpaceCorners[i].x;
				if (worldSpaceCorners[i].y > maxY) maxY = worldSpaceCorners[i].y;
				if (worldSpaceCorners[i].y < minY) minY = worldSpaceCorners[i].y;
			}

			Game.main.RenderRange = new Rectangle (minX, minY, maxX - minX, maxY - minY);
		}
	}
}
