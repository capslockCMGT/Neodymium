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
        //cant be fucked to use proper polymorphism on this thing. transformMode it is
        protected ModelRenderer activeRenderer;
        ModelRenderer arrow;
        ModelRenderer torus;
        ModelRenderer blockArrow;
        protected Box gizmoCollider;
        static Editor editor;
        Quaternion forward, up, left;
        int selectedAxis = 0;
        public int transformMode
        {
            get { return _transformMode; }
            set { _transformMode = value;
                if(activeRenderer != null)
                    RemoveChild(activeRenderer);
                switch(_transformMode)
                {
                    default:
                        activeRenderer = arrow;
                        activeRenderer.scaleXYZ = new Vector3(.4f, 0.2f, .2f);

                        gizmoCollider.scaleXYZ = new Vector3(3, .6f, .6f);
                        gizmoCollider.x = -2.5f;
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                }
                AddChild(activeRenderer);
                activeRenderer.AddChild(gizmoCollider);
            }
        }
        int _transformMode = 0;

        //Panel temp1, temp2;
        public TransformGizmo()
        {
            editor = ((Editor)game);
            arrow = new ModelRenderer("editor/arrow.obj", "editor/whitePixel.png");
            gizmoCollider = new Box("editor/whitePixel.png");
            gizmoCollider.visible = false;
            transformMode = 0;

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

            switch(transformMode)
            {
                default:
                    HandleDragMouseTranslate();
                    break;
                case 1:
                    break;
                case 2:
                    break;
            }
        }

        void HandleDragMouseTranslate()
        {
            if (selectedAxis == 0) return;
            if (!Input.GetMouseButton(0))
            {
                editor.uiHandler.UpdateGameObjectPropertyMenu();
                selectedAxis = 0;
                return;
            }
            Vector3 screenposNormal = position;
            Vector3 dir;
            Vector3 localDir;
            switch(selectedAxis)
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
            activeRenderer.rotation = forward;
            if (gizmoCollider.collider.RayCastTest(start, end))
            {
                selectedAxis = 1;
                return true;
            }
            activeRenderer.rotation = up;
            if (gizmoCollider.collider.RayCastTest(start, end))
            {
                selectedAxis = 2;
                return true;
            }
            activeRenderer.rotation = left;
            if (gizmoCollider.collider.RayCastTest(start, end))
            {
                selectedAxis = 3;
                return true;
            }
            selectedAxis = 0;
            return false;
        }

        protected override void RenderSelf(GLContext glContext)
        {
            //this is evil & i cant believe im doing this
            GL.Disable(0xb71);

            activeRenderer.visible = true;
            activeRenderer.rotation = forward;
            activeRenderer.color = (selectedAxis == 1 ? 0x55AAFFu : 0x0000FFu);
            activeRenderer.Render(glContext);
            activeRenderer.rotation = up;
            activeRenderer.color = (selectedAxis == 2 ? 0xAAFF55u : 0x00FF00u);
            activeRenderer.Render(glContext);
            activeRenderer.rotation = left;
            activeRenderer.color = (selectedAxis == 3 ? 0xFF55AAu : 0xFF0000u);
            activeRenderer.Render(glContext);
            activeRenderer.visible = false;

            GL.Enable(0xb71);
        }
    }
}
