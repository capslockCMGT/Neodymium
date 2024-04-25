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
                        activeRenderer = blockArrow;
                        activeRenderer.scaleXYZ = new Vector3(.2f, 0.2f, .2f);

                        gizmoCollider.scaleXYZ = new Vector3(6, .6f, .6f);
                        gizmoCollider.x = -5f;
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
            blockArrow = new ModelRenderer("editor/blockArrow.obj", "editor/whitePixel.png");
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
            HandleDragMouse();
        }

        void HandleDragMouse()
        {
            if (selectedAxis == 0) return;
            if (!Input.GetMouseButton(0))
            {
                editor.uiHandler.UpdateGameObjectPropertyMenu();
                selectedAxis = 0;
                return;
            }

            switch (transformMode)
            {
                default:
                    HandleDragMouseTranslate();
                    break;
                case 1:
                    break;
                case 2:
                    HandleDragMouseScale();
                    break;
            }
        }
        void HandleDragMouseTranslate()
        {
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
        }
        void HandleDragMouseScale()
        {
            //i could just copypaste the translate code. fucking remarkable that this worked
            Vector3 screenposNormal = position;
            Vector3 dir;
            Vector3 localDir;
            switch (selectedAxis)
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
            GizmoNormalOnScreen /= GizmoNormalOnScreen.MagSq();
            editor.selectedGameobject.scaleXYZ += GizmoNormalOnScreen * Input.mouseVelocity * localDir;
        }

        public bool RaycastOnClick(Vector3 start, Vector3 end)
        {
            if(!visible) return false;
            Vector3 n;
            float dist = float.MaxValue;
            float lowest = float.MaxValue;
            bool res = false;
            activeRenderer.rotation = forward;
            res |= gizmoCollider.collider.RayCast(start, end, out dist, out n);
            if(dist < lowest)
            {
                lowest = dist;
                selectedAxis = 1;
            }
            activeRenderer.rotation = up;
            res |= gizmoCollider.collider.RayCast(start, end, out dist, out n);
            if (dist < lowest)
            {
                lowest = dist;
                selectedAxis = 2;
            }
            activeRenderer.rotation = left;
            res |= gizmoCollider.collider.RayCast(start, end, out dist, out n);
            if (dist < lowest)
                selectedAxis = 3;
            if (!res) selectedAxis = 0;
            return res;
        }

        protected override void RenderSelf(GLContext glContext)
        {
            //this is evil & i cant believe im doing this
            GL.Clear(0x100);

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
        }
    }
}
