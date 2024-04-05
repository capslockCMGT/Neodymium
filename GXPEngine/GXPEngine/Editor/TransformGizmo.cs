using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;

namespace GXPEngine.Editor
{
    public class TransformGizmo : GameObject
    {
        protected BufferRenderer gizmo;
        static Editor editor;

        public TransformGizmo()
        {
            editor = ((Editor)game);
            gizmo = new BufferRenderer("editor/arrow.obj");
            gizmo.texture = Texture2D.GetInstance("editor/whitePixel.png");
        }

        protected override void RenderSelf(GLContext glContext)
        {
            glContext.PushMatrix(new float[]
            {
                 1f,  0,  0,  0,
                  0, 1f,  0,  0,
                  0,  0, 1f,  0,
                  0,  0,  0,  1
            });
            glContext.SetColor(0, 0xFF, 0, 0xFF);
            gizmo.DrawBuffers(glContext);
            glContext.SetColor(0xFF, 0xFF, 0xFF, 0xFF);
            glContext.PopMatrix();
        }
    }
}
