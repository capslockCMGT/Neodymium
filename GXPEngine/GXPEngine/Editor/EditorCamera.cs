using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine;
using GXPEngine.Core;
using GXPEngine.UI;

namespace GXPEngine.Editor.Exclusives
{
    public class EditorCamera : Camera
    {
        //private Camera actualCam;
        private Vector2 screenRotation = new Vector2(0,0);
        public EditorCamera(float FOV = 90, float near = .1f, float far = 100) : base(new ProjectionMatrix(FOV, (FOV * Game.main.height) / Game.main.width, near, far))
        {

        }
        //{
        //    actualCam = new Camera(new ProjectionMatrix(FOV, (FOV * game.height) / game.width, near, far));
        //    AddChild(actualCam);
        //}

        void Update()
        {
            game.uiManager.AssignWindow(RenderTarget);
            UpdateRotation();
            if (!Input.GetMouseButton(1) || InputField.AnyTyping)
                return;
            UpdatePosition();
        }

        //public Vector3 ScreenPointToGlobal(int screenX, int screenY, float depth)
        //{
        //    return actualCam.ScreenPointToGlobal(screenX, screenY, depth);
        //}

        void UpdatePosition()
        {
            //this breaks if EditorCamera is a child of anything (rotated or scaled). Good thing that its only allowed in the editor
            float vel = Time.deltaTimeS;
            if (Input.GetKey(Key.LEFT_SHIFT)) vel *= 4;
            if (Input.GetKey(Key.W) || Input.GetKey(Key.S))
            {
                float vec = Input.GetKey(Key.S) ? vel : 0;
                vec -= Input.GetKey(Key.W) ? vel : 0;
                position += TransformDirection(0, 0, vec);
            }
            if (Input.GetKey(Key.A) || Input.GetKey(Key.D))
            {
                float vec = Input.GetKey(Key.D) ? vel : 0;
                vec -= Input.GetKey(Key.A) ? vel : 0;
                position += TransformDirection(vec, 0, 0);
            }
            if (Input.GetKey(Key.SPACE) || Input.GetKey(Key.LEFT_CTRL))
            {
                float vec = Input.GetKey(Key.SPACE) ? vel : 0;
                vec -= Input.GetKey(Key.LEFT_CTRL) ? vel : 0;
                y += vec;
            }
        }

        void UpdateRotation()
        {
            if (Input.GetMouseButton(1))
            {
                screenRotation += Input.mouseVelocity * Time.deltaTimeS;
                if (screenRotation.y > .5f*Mathf.PI) screenRotation.y = Mathf.PI * .499f;
                if (screenRotation.y < -.5f * Mathf.PI) screenRotation.y = Mathf.PI * -.499f;

                screenRotation.x %= 2 * Mathf.PI;

                rotation = Quaternion.FromRotationAroundAxis(Vector3.up, screenRotation.x);
                Rotate( Quaternion.FromRotationAroundAxis(Vector3.left, screenRotation.y));
            }
        }
    }
}
