using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.UI;
using GXPEngine.Core;
using GXPEngine.OpenGL;

namespace GXPEngine.Editor
{
    public class TransformGizmo : GameObject
    {
        protected ModelRenderer gizmo;
        protected Box gizmoCollider;
        static Editor editor;
        Quaternion forward, up, left;
        int selected = 0;

        //Panel temp1, temp2;
        public TransformGizmo()
        {
            editor = ((Editor)game);
            gizmo = new ModelRenderer("editor/arrow.obj", "editor/whitePixel.png");
            gizmo.scaleXYZ = new Vector3(.4f, 0.2f, .2f);
            gizmo.visible = false;
            AddChild(gizmo);

            gizmoCollider = new Box("editor/whitePixel.png");
            gizmoCollider.scaleXYZ = new Vector3(3,.6f,.6f);
            gizmoCollider.x = -2.5f;
            gizmoCollider.visible = false;
            gizmo.AddChild(gizmoCollider);

            forward = Quaternion.FromRotationAroundAxis(new Vector3(0, 1, 0), Mathf.PI * 1.5f);
            up = Quaternion.FromRotationAroundAxis(new Vector3(0, 0, 1), Mathf.PI * .5f);
            left = Quaternion.FromRotationAroundAxis(new Vector3(0, 1, 0), Mathf.PI * 1f);

            //temp1 = new Panel(15,15);
           // temp2 = new Panel(15,15);
        }

        void Update()
        {
            if (editor.selectedGameobject != null)
            {
                visible = true;
                position = editor.selectedGameobject.globalPosition;
                rotation = editor.selectedGameobject.parent.globalRotation;
            }
            else visible = false;

            HandleDragMouse();
        }

        void HandleDragMouse()
        {
            if (selected == 0) return;
            if (!Input.GetMouseButton(0))
            {
                editor.uiHandler.UpdateGameObjectPropertyMenu();
                selected = 0;
                return;
            }
            Vector3 screenposNormal = position;
            Vector3 dir;
            Vector3 localDir;
            switch(selected)
            {
                case 1:
                    dir = rotation.Forward;
                    localDir = Vector3.forward;
                    break;
                case 2:
                    dir = rotation.Up;
                    localDir = Vector3.up;
                    break;
                default:
                    dir = rotation.Left;
                    localDir = Vector3.left;
                    break;
            }
            screenposNormal += dir;
            screenposNormal = editor.mainCam.GlobalToScreenPoint(screenposNormal) - editor.mainCam.GlobalToScreenPoint(position);
            Vector2 GizmoNormalOnScreen = new Vector2(screenposNormal.x, screenposNormal.y);
            //this is so messed up but it makes a little sense
            GizmoNormalOnScreen /= GizmoNormalOnScreen.MagSq();
            editor.selectedGameobject.position += GizmoNormalOnScreen * Input.mouseVelocity * localDir;
            Console.WriteLine(GizmoNormalOnScreen * Input.mouseVelocity);
        }

        public bool RaycastOnClick(Vector3 start, Vector3 end)
        {
            if(!visible) return false;
            gizmo.rotation = forward;
            if (gizmoCollider.collider.RayCastTest(start, end))
            {
                selected = 1;
                return true;
            }
            gizmo.rotation = up;
            if (gizmoCollider.collider.RayCastTest(start, end))
            {
                selected = 2;
                return true;
            }
            gizmo.rotation = left;
            if (gizmoCollider.collider.RayCastTest(start, end))
            {
                selected = 3;
                return true;
            }
            selected = 0;
            return false;
        }

        protected override void RenderSelf(GLContext glContext)
        {
            //this is evil & i cant believe im doing this
            GL.Disable(0xb71);

            gizmo.visible = true;
            gizmo.rotation = forward;
            gizmo.color = (selected == 1 ? 0x55AAFFu : 0x0000FFu);
            gizmo.Render(glContext);
            gizmo.rotation = up;
            gizmo.color = (selected == 2 ? 0xAAFF55u : 0x00FF00u);
            gizmo.Render(glContext);
            gizmo.rotation = left;
            gizmo.color = (selected == 3 ? 0xFF55AAu : 0xFF0000u);
            gizmo.Render(glContext);
            gizmo.visible = false;

            GL.Enable(0xb71);
        }
    }
}
