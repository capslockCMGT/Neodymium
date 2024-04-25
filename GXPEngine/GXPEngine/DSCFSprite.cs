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
    public class DSCFSprite : Sprite
    {
        public DSCFSprite(Texture2D texture, bool addCollider) : base(texture, addCollider)
        {
        }

        public DSCFSprite(string filename, bool keepInCache = true, bool addCollider = false) : base(filename, keepInCache, addCollider)
        {

        }

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
                scaleX*zinv, 0,0,0,
                0, -scaleY*zinv*game.heightRatio, 0,0,
                0,0,1,0,
                -cameraSpacePosition.x, -cameraSpacePosition.y, z, 1
            });
            base.RenderSelf(gLContext);
            gLContext.PopMatrix();
        }
    }
}
