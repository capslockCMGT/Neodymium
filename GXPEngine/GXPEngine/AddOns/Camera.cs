using System.Security.Policy;
using GXPEngine.Core; // For Vector2

namespace GXPEngine {
	/// <summary>
	/// A Camera gameobject, that owns a rectangular render window, and determines the focal point, rotation and scale
	/// of what's rendered in that window.
	/// (Don't forget to add this as child somewhere in the hierarchy.)
	/// </summary>
	public class Camera : GameObject {
		public Window RenderTarget {
			get {
				return _renderTarget;
			}
		}
		Window _renderTarget;
		public ProjectionMatrix projection;

		/// <summary>
		/// Creates a camera game object and a sub window to render to.
		/// Add this camera as child to the object you want to follow, or 
		/// update its coordinates directly in an update method.
		/// </summary>
		/// <param name="projection">The camera's projection matrix.</param>
		/// <param name="clearBackground">If the background should be made black before rendering the scene.</param>
		public Camera(ProjectionMatrix projection, bool clearBackground=true) {
			this.projection = projection;
			_renderTarget = new Window (-game.width / 2, -game.height / 2, game.width, game.height, this, clearBackground);
			game.OnAfterRender += _renderTarget.RenderWindow;
            game.uiManager.AssignWindow(_renderTarget);
        }

		/// <summary>
		/// Returns whether a screen point (such as received from e.g. Input.mouseX/Y) is in the camera's window
		/// </summary>
		public bool ScreenPointInWindow(int screenX, int screenY) {
			return
				screenX >= _renderTarget.windowX &&
				screenX <= _renderTarget.windowX + _renderTarget.width &&
				screenY >= _renderTarget.windowY &&
				screenY <= _renderTarget.windowY + _renderTarget.height;
		}

        /// <summary>
        /// Translates a point from camera space to global space, taking the camera transform into account.
        /// The input should be a point in screen space (coordinates between 0 and game.width/height), 
        /// that is covered by the camera window (use ScreenPointInWindow to check).
        /// You can combine this for instance with HitTestPoint and Input.mouseX/Y to check whether the
        /// mouse hits a sprite that is shown in the camera's window.
        /// </summary>
        /// <param name="screenX">The x coordinate of a point in screen space (like Input.mouseX) </param>
        /// <param name="screenY">The y coordinate of a point in screen space (like Input.mouseY) </param>
        /// <param name="depth">The depth of the point in screen space, ranges from 0 to 1, meaning near and far plane of the camera respectively. </param>
        /// <returns>Global space coordinates (to be used e.g. with HitTestPoint) </returns>
        public Vector3 ScreenPointToGlobal(int screenX, int screenY, float depth = 0) {
			return TransformPoint(ScreenPointToLocal(screenX, screenY, depth));
			//this doesnt work. only if camera is child of game
		}
		public Vector3 ScreenPointToLocal(int screenX, int screenY, float depth = 0)
		{
            Vector3 camSpace = new Vector3(screenX / (float)game.width * -2 + 1, screenY / (float)game.height * 2 - 1, -projection.near - projection.far * depth);
            camSpace.x /= projection.matrix[0];
            camSpace.y /= projection.matrix[5];
            camSpace.x *= camSpace.z;
            camSpace.y *= camSpace.z;
			return camSpace;
        }
        /// <summary>
        /// Translates a point from global space to the screen, taking camera transform into account.
		/// If its outside of the frustum, it'll land *somewhere*.
		/// X and Y are screen coordinates ranging from 0 to width/height.
		/// Z is depth ranging from -1 to 1.
        /// </summary>
        public Vector3 GlobalToScreenPoint(Vector3 point)
		{
			Vector3 camSpace = GlobalToCameraSpace(point);
            //why x keeps being negative in cam space is a mystery to me
            //the joys of no good graphics library
            camSpace.x = (1-camSpace.x)*.5f*game.width;
			camSpace.y = (camSpace.y+1)*.5f*game.height;
			/*if (camSpace.z < 0) // you can replace this test with min render distance and max render distance for more efficiency
			{
				//point is behind the screen so it shouldnt appear on it
				// (this shit below is ugly ik)
				camSpace.x = float.MaxValue;
				camSpace.y = float.MaxValue;
            }*/
            return camSpace;
		}

        /// <summary>
        /// Translates a point from global space to the camera space, taking camera transform into account.
        /// If its outside of the frustum, it'll land *somewhere*.
        /// X and Y are screen coordinates ranging from -1 to 1.
        /// Z is depth ranging from 0 to far-near.
        /// </summary>
        public Vector3 GlobalToCameraSpace(Vector3 point)
        {
            Vector3 camSpace = InverseTransformPoint(point);
            camSpace.x *= projection.matrix[0];
            camSpace.y *= projection.matrix[5];
            camSpace.z *= projection.matrix[10];
			float tz = 1 / -camSpace.z;
            camSpace.x *= tz;
            camSpace.y *= tz;
			camSpace.z += projection.matrix[14];
            return camSpace;
        }

		public float CameraSpaceZToDepthBufferRange(float z)
		{
            //thanjk you https://learnopengl.com/Advanced-OpenGL/Depth-testing
			//if only i had knownn before
            return (1 / z - 1 / projection.near) / (1 / projection.far - 1 / projection.near);
        }

		protected override void OnDestroy() {
			game.OnAfterRender -= _renderTarget.RenderWindow;
		}
	}
}
