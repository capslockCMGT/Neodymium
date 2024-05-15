using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;

namespace GXPEngine
{
    /// <summary>
    /// DepthSortedCameraFacingSprite - a sprite that always faces the camera.
    /// What a mouthful.
    /// </summary>
    public class DSCFSprite : AnimationSprite
    {
        public float size = 1;
        public DSCFSprite(Texture2D texture, bool addCollider) : base(texture, 1,1,1,addCollider)
        {
        }

        public DSCFSprite(string filename, bool keepInCache = true, bool addCollider = false) : base(filename, 1,1,1, keepInCache, addCollider)
        {
        }

        public DSCFSprite(string filename, int cols, int rows, int frames=-1, bool keepInCache = true, bool addCollider = false) : base(filename, cols, rows, frames, keepInCache, addCollider)
        {
        }
        public DSCFSprite(Texture2D texture, int cols, int rows, int frames = -1, bool addCollider = false) : base(texture, cols, rows, frames, addCollider)
        { }


        protected override void RenderSelf(GLContext glContext)
        {
            Window.ActiveWindow.RegisterDepthSorted(this);
            return;
        }

        public override void RenderDepthSorted(GLContext gLContext, Vector3 cameraSpacePosition)
        {
            float z = ((Camera)Window.ActiveWindow.camera).CameraSpaceZToDepthBufferRange(cameraSpacePosition.z);
            float zinv = 1/(cameraSpacePosition.z + ((Camera)Window.ActiveWindow.camera).projection.near);
            gLContext.PushMatrix(new float[]
            {
                zinv*size, 0,0,0,
                0, -zinv*game.heightRatio*size, 0,0,
                0,0,1,0,
                -cameraSpacePosition.x, -cameraSpacePosition.y, z, 1
            });
            base.RenderSelf(gLContext);
            gLContext.PopMatrix();
        }
    }
}
