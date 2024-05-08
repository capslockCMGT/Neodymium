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
        public float distance = 16;
        public bool CamEnabled = false;
        Pivot Arm;
        Vector2 screenRotation = new Vector2(0, .2f);
        Vector2 mouseVelSmoothed = new Vector2(0, 0);
        bool showCursor;
        public RotateAroundLevelCamera(Camera cam)
        {
            _cam = cam;
            game.AddChild(cam);
            Arm = new Pivot();
            AddChild(Arm);

            y = -5f;
            Arm.z = distance;
            _cam.position = Arm.globalPosition;
            _cam.rotation = Arm.globalRotation;
        }
        void Update()
        {
            if (CamEnabled)
            {
                if (Input.GetKeyDown(Key.TAB)) showCursor = !showCursor;
                if (Input.GetMouseButton(0)) showCursor = false;
                game.ShowMouse(showCursor);
                mouseVelSmoothed += (Input.mouseVelocity - mouseVelSmoothed) * Time.deltaTimeS * 5f;

                StartGameAnimation();
            }
            else
            {
                mouseVelSmoothed = new Vector2(-.3f, 0);
            }


            Arm.z = distance;

            screenRotation += mouseVelSmoothed * Time.deltaTimeS;
            if (screenRotation.y > .3f * Mathf.PI) screenRotation.y = Mathf.PI * .299f;
            if (screenRotation.y < -.2f * Mathf.PI) screenRotation.y = Mathf.PI * -.199f;
            screenRotation.x %= 2 * Mathf.PI;

            rotation = Quaternion.FromRotationAroundAxis(Vector3.up, screenRotation.x);
            Rotate(Quaternion.FromRotationAroundAxis(Vector3.left, screenRotation.y));

            _cam.position = Arm.globalPosition;
            _cam.rotation = Arm.globalRotation;
        }
        void StartGameAnimation()
        {
            distance += (8 - distance) * Time.deltaTimeS * 5f;
            y *= 1 - Time.deltaTimeS * 5;
        }
    }
}
