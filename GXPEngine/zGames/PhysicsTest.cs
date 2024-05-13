using System;
using GXPEngine.AddOns;
using GXPEngine.Core;
using GXPEngine.OpenGL;
using GXPEngine.Physics;

namespace GXPEngine
{
    internal class PhysicsTest : Game
    {
        bool showCursor;
        Camera cam;

        PhysicsBox obj1;
        PhysicsObject hook;
        PhysicsObject floor, platform1, platform2;
        Rope rope;
        Glue glue;
        Crane crane;
        Player robot;

        public PhysicsTest() : base(800, 600, false, true, false, "UnreelEngine")
        {
            cam = new Camera(new ProjectionMatrix(90, 90 * .75f, .1f, 100), true);
            RenderMain = false;
            AddChild(cam);
            cam.SetXY(0, 1, 0);

            SetupScene();


            crane.magnet.AddAttract(obj1);
            //crane.magnet.AddAttract(robot);
        }
        void Update()
        {
            if (Input.GetKeyDown(Key.TAB)) showCursor = !showCursor;
            game.ShowMouse(showCursor);

            crane.Update();

            PhysicsObject.UpdateAll();
            FirstPersonViewUpdate();
            Gizmos.DrawPlus(new Vector3(0,2,0), 0.1f);
            (robot.collider as BoxCollider).DrawExtents();

        }
        public void SetupScene()
        {
            obj1 = new PhysicsBox("test models/suzanne.png", new Vector3(3, 0, 3));
            obj1.rotation = Quaternion.FromEulers(new Vector3(-0.05f, -0.05f, -0.05f));
            AddChild(obj1);

            floor = new PhysicsBox("cubeTex.png", Vector3.zero);
            floor.scaleY = 1f;
            floor.scaleX = 10f;
            floor.scaleZ = 10f;
            floor.y = -5;
            floor.simulated = false;
            AddChild(floor);

            platform1 = new PhysicsBox("cubeTex.png", new Vector3(-3, -2.5f, -1.5f),false);
            platform1.scaleY = 0.5f;
            AddChild(platform1);

            platform2 = new PhysicsBox("cubeTex.png", new Vector3(-3, -2.5f, 3), false);
            platform2.scaleY = 0.5f;
            AddChild(platform2);

            crane = new Crane(new Vector3(0,-2,0));
            AddChild(crane);
            crane.position += new Vector3(-3, 0, -3);

            robot = new Player("robot/model.obj", "robot/texture.png", new Vector3(-3, 0, 3));
            robot.SetMass(0.01f);
            AddChild(robot);
            robot.AddCheckpoint(new Vector3(-3, -0.5f, -1), 1);
            //robot.AddCheckpoint(new Vector3(5, 0, 5));

            Lighting.SetLight(0, new Vector3(5, 5, 5), new Vector3(.4f, .2f, .2f), new Vector3(.0f, .2f, .7f));
            Lighting.SetLight(1, new Vector3(-5, -5, -0), new Vector3(.0f, .0f, .0f), new Vector3(.5f, .2f, .0f));
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
                cam.Move(Time.deltaTimeS * 3, 0, 0);
            if (Input.GetKey(Key.A))
                cam.Move(-Time.deltaTimeS * 3, 0, 0);
            if (Input.GetKey(Key.W))
            {
                Vector3 delta = cam.TransformDirection(0, 0, 1);
                delta.y = 0;
                delta = delta.normalized() * (Time.deltaTimeS * 3);
                cam.position -= delta;
            }
            if (Input.GetKey(Key.S))
            {
                Vector3 delta = cam.TransformDirection(0, 0, 1);
                delta.y = 0;
                delta = delta.normalized() * (Time.deltaTimeS * 3);
                cam.position += delta;
            }
            if (Input.GetKey(Key.LEFT_SHIFT))
            {
                Vector3 delta = cam.TransformDirection(0, 1, 0);
                delta.x = 0; delta.z = 0;
                delta = delta.normalized() * (Time.deltaTimeS * 3);
                cam.position -= delta;
            }
            if (Input.GetKey(Key.SPACE))
            {
                Vector3 delta = cam.TransformDirection(0, 1, 0);
                delta.x = 0; delta.z = 0;
                delta = delta.normalized() * (Time.deltaTimeS * 3);
                cam.position += delta;
            }
        }
    }
}
