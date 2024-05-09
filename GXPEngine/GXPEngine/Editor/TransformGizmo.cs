using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.UI;
using GXPEngine.Core;
using GXPEngine.OpenGL;
using System.Security.AccessControl;

namespace GXPEngine.Editor.Exclusives
{
    public class TransformGizmo : GameObject
    {
        //cant be fucked to use proper polymorphism on this thing. transformMode it is
        protected ModelRenderer activeRenderer;
        ModelRenderer arrow;
        ModelRenderer torus;
        ModelRenderer blockArrow;
        protected Box gizmoCollider;
        static SceneEditor editor;
        Quaternion forward, up, left;
        int selectedAxis = 0;
        int hoveredAxis = 0;
        Vector3 startPosition;
        Quaternion startRotation;
        Vector3 startScale;
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
                        activeRenderer = torus;
                        activeRenderer.scaleXYZ = new Vector3(1,1,1);

                        //paper thin collider? monstrously devilish caps lock
                        gizmoCollider.scaleXYZ = new Vector3(.01f, 2f, 2f);
                        gizmoCollider.x = 0f;
                        //gizmoCollider.visible = true;
                        //if it works it works is all im saying
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
            editor = ((SceneEditor)game);
            arrow = new ModelRenderer("editor/arrow.obj", "editor/whitePixel.png");
            torus = new ModelRenderer("editor/torus.obj", "editor/whitePixel.png");
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
                rotation = transformMode != 2 ? editor.selectedGameobject.parent.globalRotation : editor.selectedGameobject.globalRotation;
            }
            else visible = false;
            HandleDragMouse();
        }

        void HandleDragMouse()
        {
            if (selectedAxis == 0) return;
            if (!Input.GetMouseButton(0))
            {
                switch(transformMode)
                {
                    default:
                        EditorActionRegister.SetPositionValue(editor.selectedGameobject.position, editor.selectedGameobject, true, startPosition);
                        Console.WriteLine("set position to " + editor.selectedGameobject.position);
                        break;
                    case 1:
                        EditorActionRegister.SetRotationValue(editor.selectedGameobject.rotation, editor.selectedGameobject, true, startRotation);
                        Console.WriteLine("set rotation to " + editor.selectedGameobject.rotation);
                        break;
                    case 2:
                        EditorActionRegister.SetScaleValue(editor.selectedGameobject.scaleXYZ, editor.selectedGameobject, true, startScale);
                        Console.WriteLine("set scale to " + editor.selectedGameobject.scaleXYZ);
                        break;
                }
                editor.uiHandler.UpdateGameObjectPropertyMenu();
                selectedAxis = 0;
                transformMode = transformMode;
                return;
            }

            switch (transformMode)
            {
                default:
                    HandleDragMouseTranslate();
                    break;
                case 1:
                    HandleDragMouseRotate();
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
        void HandleDragMouseRotate()
        {
            gizmoCollider.scaleXYZ = new Vector3(.01f, 20f, 20f);

            Vector3 prevstart = editor.mainCam.ScreenPointToGlobal((int)Input.prevMousePos.x, (int)Input.prevMousePos.y, 0.001f);
            Vector3 prevend = editor.mainCam.ScreenPointToGlobal((int)Input.prevMousePos.x, (int)Input.prevMousePos.y, 1);
            Vector3 start = editor.mainCam.ScreenPointToGlobal(Input.mouseX, Input.mouseY, 0.001f);
            Vector3 end = editor.mainCam.ScreenPointToGlobal(Input.mouseX, Input.mouseY, 1);

            switch (selectedAxis)
            {
                case 1:
                    activeRenderer.rotation = forward;
                    break;
                case 2:
                    activeRenderer.rotation = up;
                    break;
                default:
                    activeRenderer.rotation = left;
                    break;
            }

            Vector3 n;
            float dist;

            gizmoCollider.collider.RayCast(prevstart, prevend, out dist, out n);
            if (dist > 100) return;
            dist /= editor.mainCam.projection.far;
            Vector3 mpos3dfirst = prevend * dist + prevstart * (1 - dist);

            gizmoCollider.collider.RayCast(start, end, out dist, out n);
            if (dist > 100) return;
            dist /= editor.mainCam.projection.far;
            Vector3 mpos3d = end * dist + start * (1 - dist);

            mpos3d -= position;
            mpos3dfirst -= position;

            Vector3 diff = mpos3d - mpos3dfirst;
            if (diff.MagnitudeSquared() < .0000000001f) return;

            /*Vector3 line = mpos3dfirst;
            float lineLength = line.Magnitude();
            line /= lineLength;
            float secondLineLength = line*diff/lineLength;

            float angle = Mathf.Atan2(secondLineLength, 1);

            editor.selectedGameobject.Rotate(Quaternion.FromRotationAroundAxis(n.normalized(), angle));
            */

            Vector2 mposProj;
            Vector2 mposFProj;
            switch (selectedAxis)
            {
                default:
                    mposProj = new Vector2(mpos3dfirst.x, mpos3dfirst.y);
                    mposFProj = new Vector2(mpos3d.x, mpos3d.y);
                    n = Vector3.forward;
                    break;
                case 2:
                    mposProj = new Vector2(mpos3d.x, mpos3d.z);
                    mposFProj = new Vector2(mpos3dfirst.x, mpos3dfirst.z);
                    n = Vector3.up;
                    break;
                case 3:
                    mposProj = new Vector2(mpos3d.z, mpos3d.y);
                    mposFProj = new Vector2(mpos3dfirst.z, mpos3dfirst.y);
                    n = Vector3.left;
                    break;
            }

            float angle1 = Mathf.Atan2(mposProj.y, mposProj.x);
            float angle2 = Mathf.Atan2(mposFProj.y, mposFProj.x);

            editor.selectedGameobject.rotation = (Quaternion.FromRotationAroundAxis(n.normalized(), angle1 - angle2)) * editor.selectedGameobject.rotation;

            Gizmos.DrawLine(position, mpos3dfirst, null, 0xFFFF0000, 2);
            Gizmos.DrawLine(position, mpos3d, null, 0xFF00FF00, 2);
            Gizmos.DrawLine(mpos3dfirst, mpos3d, null, 0xFF0000FF, 2);
        }
        void HandleDragMouseScale()
        {
            //i just copypasted the translate code. fucking remarkable that this worked
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
            
            if(res)
            {
                startPosition = editor.selectedGameobject.position;
                startRotation = editor.selectedGameobject.rotation;
                startScale = editor.selectedGameobject.scaleXYZ;
            }
            return res;
        }

        public bool TryRaycast(Vector3 start, Vector3 end)
        {
            if (!visible) return false;
            Vector3 n;
            float dist = float.MaxValue;
            float lowest = float.MaxValue;
            bool res = false;
            activeRenderer.rotation = forward;
            res |= gizmoCollider.collider.RayCast(start, end, out dist, out n);
            if (dist < lowest)
            {
                lowest = dist;
                hoveredAxis = 1;
            }
            activeRenderer.rotation = up;
            res |= gizmoCollider.collider.RayCast(start, end, out dist, out n);
            if (dist < lowest)
            {
                lowest = dist;
                hoveredAxis = 2;
            }
            activeRenderer.rotation = left;
            res |= gizmoCollider.collider.RayCast(start, end, out dist, out n);
            if (dist < lowest)
                hoveredAxis = 3;
            if (!res) hoveredAxis = 0;

            return res;
        }

        public void TryRenderingHarder(GLContext glContext)
        {
            if (!visible) return;
            if (editor.selectedGameobject == null) return;


            //this comment is a testament to the fact that this document contains the worst code i have ever written.
            //if you have reached this far, i apologize.
            GameObject camera = Window.ActiveWindow.camera;

            if (camera is Camera)
                glContext.PushMatrix(((Camera)camera).projection.matrix);
            int pushes = 1;
            GameObject current = camera;
            Transformable cameraInverse;
            while (true)
            {
                cameraInverse = current.Inverse();
                glContext.PushMatrix(cameraInverse.matrix);
                pushes++;
                if (current.parent == null)
                    break;
                current = current.parent;
            }

            glContext.PushMatrix(matrix);
            //this is evil & i cant believe im doing this
            GL.Clear(GL.DEPTH_BUFFER_BIT);

            activeRenderer.visible = true;
            activeRenderer.rotation = forward;
            activeRenderer.color = (selectedAxis == 1 ? 0x55AAFFu : hoveredAxis == 1 ? 0x3388EEu : 0x0000DDu);
            activeRenderer.Render(glContext);
            activeRenderer.rotation = up;
            activeRenderer.color = (selectedAxis == 2 ? 0xAAFF55u : hoveredAxis == 2 ? 0x88DD33u : 0x00BB00u);
            activeRenderer.Render(glContext);
            activeRenderer.rotation = left;
            activeRenderer.color = (selectedAxis == 3 ? 0xFF55AAu : hoveredAxis == 3 ? 0xEE3388u : 0xDD0000u);
            activeRenderer.Render(glContext);
            activeRenderer.visible = false;


            //god should kill me
            glContext.PopMatrix();

            for (int i = 1; i < pushes; i++)
            {
                glContext.PopMatrix();
            }

            if (camera is Camera)
                glContext.PopMatrix();
        }
    }
}
