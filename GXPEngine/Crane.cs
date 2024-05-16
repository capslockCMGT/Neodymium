using GXPEngine.Core;
using GXPEngine.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    public class Crane : GameObject
    {
        public PhysicsObject trunk;
        public PhysicsObject cabin;
        public PhysicsObject hook;
        public Magnet magnet;
        public Rope rope;
        //magnet has 3 freedom degrees, which can be represented in cylindric coordinate system
        float phi = 0, r = 0, y = 0;
        // for values below I use representation in cylindric coordinates vec3 (value_phi, value_r, value_y) correspondingly (phi in rads)
        public Vector3 lowerLimit = new Vector3(0, 2, 2);
        public Vector3 upperLimit = new Vector3(0, 9.6f, 8);
        public Vector3 speedLimits = new Vector3 (1, 0.02f, 0.02f);
        public Vector3 acceleration = new Vector3(3, 0.5f, 0.1f);
        public Vector3 velocity;

        float cabinCenterOffset;
        float trunkLength;

        SpatialSound turningCrane;
        SpatialSound movingCable;
        public Crane(Vector3 groundPos)
        {
            trunkLength = 8f;
            float cabinLongEnd = upperLimit.y + 1;
            float cabinShortEnd = cabinLongEnd/4;
            cabinCenterOffset = (cabinLongEnd + cabinShortEnd) / 2 - cabinShortEnd;

            trunk = new PhysicsMesh("crane/trunk.obj", "crane/trunk.png", groundPos + new Vector3(0, trunkLength / 2, 0),false);
            cabin = new PhysicsMesh("crane/cabin.obj", "crane/cabin.png", groundPos + new Vector3(0, trunkLength/2, cabinCenterOffset), false);
            float hookElevation = cabin.scaleY;
            hook = new PhysicsMesh("crane/hook.obj", "crane/hook.png", groundPos + new Vector3(0, trunkLength - hookElevation, upperLimit.y), false);
            magnet = new Magnet("crane/magnet.obj", "crane/magnet.png", hook.pos & scaleXYZ - new Vector3(0, lowerLimit.y, 0));

            magnet.scale = 0.2f;

            rope = new Rope(hook, magnet, 0.1f);

            r = upperLimit.y;
            y = (lowerLimit.z + upperLimit.z) / 2;

            AddChildren();

            (trunk.collider as BoxCollider).size = new Vector3(0.5f, trunkLength/2, 0.5f);
            (cabin.collider as BoxCollider).size = new Vector3(0.65f, 0.65f, (cabinLongEnd + cabinShortEnd) / 2);
            (hook.collider as BoxCollider).size = new Vector3(0.6f, 0.3f, 0.6f);
            (magnet.collider as BoxCollider).size = new Vector3(3f, 3f, 1f) * 0.7f;
            (magnet.collider as BoxCollider).offset = new Vector3(0, -2, 0);
            magnet.SetMass(0.16f);

            turningCrane = new SpatialSound(new Sound("Sounds/Crane turning.wav", true), 0.1f);
            movingCable = new SpatialSound(new Sound("Sounds/Cable length increasing decreasing.wav", true), 0.1f);
            cabin.AddChild(turningCrane);
            hook.AddChild(movingCable);
        }
        public void AddChildren()
        {
            AddChild(trunk);
            AddChild(cabin);
            AddChild(hook);
            AddChild(magnet);
        }
        public void Update()
        {
            rope.Apply(Time.deltaTimeS);
            rope.Display();

            if (Neodymium.controlsEnabled)
            {
                //phi
                if (Input.GetKey(Key.E))
                {
                    velocity.x += acceleration.x * Time.deltaTimeS;
                    if (velocity.x > speedLimits.x) velocity.x = speedLimits.x;
                }
                if (Input.GetKey(Key.Q))
                {
                    velocity.x -= acceleration.x * Time.deltaTimeS;
                    if (velocity.x < -speedLimits.x) velocity.x = -speedLimits.x;
                }
                if (!Input.GetKey(Key.E) && !Input.GetKey(Key.Q))
                    velocity.x -= velocity.x * Time.deltaTimeS * 3;



                //r
                if (Input.GetKey(Key.D))
                {
                    velocity.y += acceleration.y * Time.deltaTimeS;
                    if (velocity.y > speedLimits.y) velocity.y = speedLimits.y;

                }
                if (Input.GetKey(Key.A))
                {
                    velocity.y -= acceleration.y * Time.deltaTimeS;
                    if (velocity.y < -speedLimits.y) velocity.y = -speedLimits.y;
                }
                if (!Input.GetKey(Key.D) && !Input.GetKey(Key.A))
                    velocity.y -= velocity.y * Time.deltaTimeS * 3;



                //y
                if (Input.GetKey(Key.S))
                {
                    velocity.z += acceleration.z * Time.deltaTimeS;
                    if (velocity.z > speedLimits.z) velocity.z = speedLimits.z;
                }
                if (Input.GetKey(Key.W))
                {
                    velocity.z -= acceleration.z * Time.deltaTimeS;
                    if (velocity.z < -speedLimits.z) velocity.z = -speedLimits.z;
                }
                if (!Input.GetKey(Key.S) && !Input.GetKey(Key.W))
                    velocity.z -= velocity.z * Time.deltaTimeS * 3;

            }


            r += velocity.y;
            if (r < lowerLimit.y) r = lowerLimit.y;
            if (r > upperLimit.y) r = upperLimit.y;

            RotateCabin(Time.deltaTimeS * velocity.x);

            y += velocity.z;
            if (y < lowerLimit.z) y = lowerLimit.z;
            if (y > upperLimit.z) y = upperLimit.z;
            rope.length = y * scaleY;



            //shitty air friction
            magnet.velocity -= magnet.velocity * Time.deltaTimeS * 0.3f;
            //fym shitty? this is literally exactly what air friction should look like oh wait yeah that sucks actually
            //not that bad tho

            turningCrane.halfVolumeDistance = (Mathf.Abs(velocity.x / speedLimits.x) + Mathf.Abs(velocity.y / speedLimits.y)) * 5;
            movingCable.halfVolumeDistance = Mathf.Abs(velocity.z / speedLimits.z) * 10;

            //(trunk.collider as BoxCollider).DrawExtents();
            //(cabin.collider as BoxCollider).DrawExtents();
            //(hook.collider as BoxCollider).DrawExtents();
            //(magnet.collider as BoxCollider).DrawExtents();

        }
        /// <summary>
        /// angle is given in radians
        /// </summary>
        public void RotateCabin(float angle)
        {
            phi += angle;
            if (phi < -Mathf.PI)
                phi += Mathf.PI * 2;
            if (phi > Mathf.PI)
                phi -= Mathf.PI * 2;
            Vector3 dir = new Vector3(Mathf.Sin(phi), 1, Mathf.Cos(phi));
            Vector3 offset = trunk.position ;
            cabin.position = offset + (dir & new Vector3 (cabinCenterOffset, trunkLength/2, cabinCenterOffset));
            cabin.rotation = Quaternion.FromRotationAroundAxis(Vector3.up, -phi);
            hook.rotation = cabin.rotation;
            hook.position = dir & new Vector3 (r, cabin.y - cabin.scaleY, r);
        }
    }
}
