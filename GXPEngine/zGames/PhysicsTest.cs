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
        PhysicsObject floor;
        Rope rope;
        Glue glue;
        Crane crane;

        public PhysicsTest() : base(800, 600, false, true, false, "UnreelEngine")
        {
            cam = new Camera(new ProjectionMatrix(90, 90 * .75f, .1f, 100), true);
            RenderMain = false;
            AddChild(cam);
            cam.SetXY(0, 1, 0);

            SetupScene();

            obj1.pos = new Vector3(3,0,3);
            Lighting.SetLight(0, new Vector3(5, 5, 5), new Vector3(.4f, .2f, .2f), new Vector3(.0f, .2f, .7f));
            Lighting.SetLight(1, new Vector3(-5, -5, -0), new Vector3(.0f, .0f, .0f), new Vector3(.5f, .2f, .0f));
        }
        void Update()
        {
            if (Input.GetKeyDown(Key.TAB)) showCursor = !showCursor;
            game.ShowMouse(showCursor);

            crane.Update();
            Vector3 r = crane.magnet.TransformPoint(Vector3.zero) - obj1.TransformPoint(Vector3.zero);
            float rl = r.Magnitude();
            float k = 10f;
            if (crane.magnet.isAttracting) obj1.AddForce("magnet", new Force(k / rl / rl / rl * r));
            else obj1.AddForce("magnet", new Force(Vector3.zero));
            //crane.magnet.AddForce("magnet", new Force(-k / rl / rl / rl * r));

            PhysicsObject.UpdateAll();
            //rope.Apply(Time.deltaTimeS);
            //rope.Display();
            FirstPersonViewUpdate();
            Gizmos.DrawPlus(new Vector3(0,2,0), 0.1f);
            //Gizmos.DrawLine(new Vector3(0, 2, 0), hook.position, width:10, color: 0xff777777);
            (floor.collider as BoxCollider).DrawExtents();

            if (Input.GetKey(Key.T))
            {
                crane.magnet.Unglue(obj1);
            }

        }
        public void SetupScene()
        {
            obj1 = new PhysicsBox("test models/suzanne.png", Vector3.zero);
            AddChild(obj1);
            //obj1.scale = 0.5f;
            //obj1.velocity = Vector3.one;
            //AddChild(obj1);

            floor = new PhysicsBox("cubeTex.png", Vector3.zero);
            floor.scaleY = 1f;
            floor.scaleX = 10f;
            floor.scaleZ = 10f;
            floor.y = -5;
            floor.simulated = false;
            AddChild(floor);

            //hook = new PhysicsBox("cubeTex.png", new Vector3(0f, 2, 2f));
            //hook.scale = 0.1f;
            //hook.simulated = false;
            //AddChild(hook);
            //rope = new Rope(hook, obj1, 0.1f, 4);

            crane = new Crane(new Vector3(0,-2,0));
            AddChild(crane);
            crane.position += new Vector3(-3, 0, -3);

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
