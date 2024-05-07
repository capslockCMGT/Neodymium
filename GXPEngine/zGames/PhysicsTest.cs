using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;
using GXPEngine.Physics;
using static System.Net.Mime.MediaTypeNames;

namespace GXPEngine
{
    internal class PhysicsTest : Game
    {
        bool showCursor;
        Camera cam;

        PhysicsObject obj1;
        PhysicsObject floor;
        public PhysicsTest() : base(800, 600, false, true, false, "actual cool test scene")
        {
            cam = new Camera(new ProjectionMatrix(90, 90 * .75f, .1f, 100), true);
            RenderMain = false;
            AddChild(cam);
            cam.SetXY(0, 1, 0);

            SetupScene();
        }
        void Update()
        {
            if (Input.GetKeyDown(Key.TAB)) showCursor = !showCursor;
            game.ShowMouse(showCursor);
            PhysicsObject.UndateAll();

            FirstPersonViewUpdate();
            Console.WriteLine(obj1.pos);
        }
        public void SetupScene()
        {
            obj1 = new PhysicsObject("cubeTex.png",Vector3.zero);
            obj1.scale = 0.5f;
            AddChild(obj1);

            floor = new PhysicsObject("cubeTex.png", Vector3.zero);
            floor.scaleY = 0.2f;

            floor.y = -5;
            floor.simulated = false;
            AddChild(floor);
        }
        public void FirstPersonViewUpdate()
        {
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
