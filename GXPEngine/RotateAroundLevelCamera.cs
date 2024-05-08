using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;

namespace GXPEngine
{
    public class RotateAroundLevelCamera : GameObject
    {
        private Camera _cam;
        public Camera cam { get { return _cam; } }
        public float distance = 8;
        Pivot Arm;
        Vector2 screenRotation = new Vector2(0, 0);
        Vector2 mouseVelSmoothed = new Vector2(0, 0);
        bool showCursor;
        public RotateAroundLevelCamera(Camera cam)
        {
            _cam = cam;
            game.AddChild(cam);
            Arm = new Pivot();
            AddChild(Arm);
        }
        void Update()
        {
            if (Input.GetKeyDown(Key.TAB)) showCursor = !showCursor;
            if (Input.GetMouseButton(0)) showCursor = false;
            game.ShowMouse(showCursor);

            Arm.z = distance;

            mouseVelSmoothed += (Input.mouseVelocity - mouseVelSmoothed) * Time.deltaTimeS * 5f;
            screenRotation += mouseVelSmoothed * Time.deltaTimeS;
            if (screenRotation.y > .3f * Mathf.PI) screenRotation.y = Mathf.PI * .299f;
            if (screenRotation.y < -.2f * Mathf.PI) screenRotation.y = Mathf.PI * -.199f;
            screenRotation.x %= 2 * Mathf.PI;

            rotation = Quaternion.FromRotationAroundAxis(Vector3.up, screenRotation.x);
            Rotate(Quaternion.FromRotationAroundAxis(Vector3.left, screenRotation.y));

            _cam.position = Arm.globalPosition;
            _cam.rotation = Arm.globalRotation;
        }
    }
}
