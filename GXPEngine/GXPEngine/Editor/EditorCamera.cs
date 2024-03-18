using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine;
using GXPEngine.Core;

namespace GXPEngine.Editor
{
    public class EditorCamera : GameObject
    {
        private Camera actualCam;
        private Vector2 screenRotation = new Vector2(0,0);
        private Vector2 mousePosition;
        private Vector2 mouseVelocity;
        public EditorCamera(float FOV = 90, float near = .1f, float far = 100)
        {
            actualCam = new Camera(new ProjectionMatrix(FOV, (FOV * game.height) / game.width, near, far));
            AddChild(actualCam);
        }

        void Update()
        {
            SetRotation();
        }

        void SetRotation()
        {
            if (Input.GetMouseButton(1))
            {
                mouseVelocity = new Vector2(Input.mouseX, Input.mouseY) - mousePosition;

                screenRotation += mouseVelocity * Time.deltaTimeS;
                if (screenRotation.y > Mathf.PI) screenRotation.y = Mathf.PI * .999f;
                if (screenRotation.y < -Mathf.PI) screenRotation.y = Mathf.PI * -.999f;

                screenRotation.x %= 2 * Mathf.PI;
            }

            mousePosition = new Vector2(Input.mouseX, Input.mouseY);

            rotation = Quaternion.FromRotationAroundAxis(Vector3.up, screenRotation.x);
            actualCam.rotation = Quaternion.FromRotationAroundAxis(Vector3.left, screenRotation.y);
        }
    }
}
