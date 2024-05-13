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
        float _distance = 20;
        public float distance = 20;
        public float height = 3;
        public float maxAngleLow = -.1f;
        public float maxAngleHigh = .3f;
        float _maxAngleLow = -.1f;
        float _maxAngleHigh = .3f;
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

            y = 0f;
            Arm.z = _distance;
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
                LerpToLevel();
            }
            else
            {
                mouseVelSmoothed = new Vector2(-.3f, 0);
            }

            Arm.z = _distance;

            screenRotation += mouseVelSmoothed * Time.deltaTimeS;
            if (screenRotation.y > _maxAngleHigh * Mathf.PI) screenRotation.y = Mathf.PI * .99f * _maxAngleHigh;
            if (screenRotation.y < _maxAngleLow * Mathf.PI) screenRotation.y = Mathf.PI * .99f * _maxAngleLow;
            screenRotation.x %= 2 * Mathf.PI;

            rotation = Quaternion.FromRotationAroundAxis(Vector3.up, screenRotation.x);
            Rotate(Quaternion.FromRotationAroundAxis(Vector3.left, screenRotation.y));

            _cam.position = Arm.globalPosition;
            _cam.rotation = Arm.globalRotation;
        }
        void LerpToLevel()
        {
            _distance += (distance - _distance) * Time.deltaTimeS * 5f;
            y += (height - y)*Time.deltaTimeS * 5;
            _maxAngleHigh += (maxAngleHigh - _maxAngleHigh) * Time.deltaTimeS * 5f;
            _maxAngleLow += (maxAngleLow - _maxAngleLow ) * Time.deltaTimeS * 5f;
        }
    }
}
