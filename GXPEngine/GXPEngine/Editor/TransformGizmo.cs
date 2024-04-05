using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;
using GXPEngine.OpenGL;

namespace GXPEngine.Editor
{
    public class TransformGizmo : GameObject
    {
        protected ModelRenderer gizmo;
        static Editor editor;
        Quaternion forward, up, left;
        public TransformGizmo()
        {
            editor = ((Editor)game);
            gizmo = new ModelRenderer("editor/arrow.obj", "editor/whitePixel.png");
            forward = new Quaternion();
            up = Quaternion.FromRotationAroundAxis(new Vector3(0, 0, 1), Mathf.PI * .5f);
            left = Quaternion.FromRotationAroundAxis(new Vector3(0, 1, 0), Mathf.PI * .5f);
        }

        protected override void RenderSelf(GLContext glContext)
        {
            GL.Disable(0xb71);

            gizmo.rotation = forward;
            gizmo.color = 0x0000FF;
            gizmo.Render(glContext);
            gizmo.rotation = up;
            gizmo.color = 0x00FF00;
            gizmo.Render(glContext);
            gizmo.rotation = left;
            gizmo.color = 0xFF0000;
            gizmo.Render(glContext);

            GL.Enable(0xb71);
        }
    }
}
