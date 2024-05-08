using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;

namespace GXPEngine
{
    public class Neodymium : Game
    {
        Camera cam;
        bool showCursor;
        public Neodymium() : base(1200, 900, false) 
        {
            cam = new Camera(new ProjectionMatrix(120, 90, .1f, 100));
            AddChild(cam);
            AddChild(Editor.GameObjectReader.ReadGameObjectTree("level1.gxp3d"));
        }
        void Update()
        {
            FirstPersonViewUpdate();
        }
        void FirstPersonViewUpdate()
        {
            if (Input.GetKeyDown(Key.TAB)) showCursor = !showCursor;
            if (!showCursor && Input.GetMouseButtonDown(0)) showCursor = false;
            game.ShowMouse(showCursor);

            float msex = Input.mouseX / 800f * Mathf.PI;
            float msey = Input.mouseY / 600f * Mathf.PI;
            cam.rotation = (Quaternion.FromRotationAroundAxis(0, 1, 0, msex));
            cam.Rotate(Quaternion.FromRotationAroundAxis(1, 0, 0, msey));
            //cam.Rotate(Quaternion.FromRotationAroundAxis(cam.TransformDirection(-1, 0, 0), msey));

            //minecraft creative mode controls
            if (Input.GetKey(Key.D))
                cam.Move(Time.deltaTimeS, 0, 0);
            if (Input.GetKey(Key.A))
                cam.Move(-Time.deltaTimeS, 0, 0);
            if (Input.GetKey(Key.W))
            {
                Vector3 delta = cam.TransformDirection(0, 0, 1);
                delta.y = 0;
                delta = delta.normalized() * (Time.deltaTimeS);
                cam.position -= delta;
            }
            if (Input.GetKey(Key.S))
            {
                Vector3 delta = cam.TransformDirection(0, 0, 1);
                delta.y = 0;
                delta = delta.normalized() * (Time.deltaTimeS);
                cam.position += delta;
            }
            if (Input.GetKey(Key.LEFT_SHIFT))
            {
                Vector3 delta = cam.TransformDirection(0, 1, 0);
                delta.x = 0; delta.z = 0;
                delta = delta.normalized() * (Time.deltaTimeS);
                cam.position -= delta;
            }
            if (Input.GetKey(Key.SPACE))
            {
                Vector3 delta = cam.TransformDirection(0, 1, 0);
                delta.x = 0; delta.z = 0;
                delta = delta.normalized() * (Time.deltaTimeS);
                cam.position += delta;
            }
        }
    }
}
