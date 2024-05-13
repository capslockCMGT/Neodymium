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
        float cabinCenterOffset;
        // for values below I use representation in cylindric coordinates vec3 (value_phi, value_r, value_y) speeds correspondingly (phi in rads)
        public Vector3 lowerLimit = new Vector3(0, 2, 2);
        public Vector3 upperLimit = new Vector3(0, 8, 14);
        public Vector3 speedLimits = new Vector3 (1, 0.01f, 0.01f);
        public Vector3 acceleration = new Vector3(3, 0.5f, 0.1f);
        public Vector3 velocity;

        public Crane(Vector3 groundPos)
        {
            float trunkLength = 8f;
            float cabinLongEnd = upperLimit.y + 1;
            float cabinShortEnd = cabinLongEnd/4;
            cabinCenterOffset = (cabinLongEnd + cabinShortEnd) / 2 - cabinShortEnd;

            trunk = new PhysicsBox("cubeTex.png", groundPos + new Vector3(0, trunkLength / 2, 0),false);
            cabin = new PhysicsBox("cubeTex.png", groundPos + new Vector3(0, trunkLength, cabinCenterOffset), false);

            trunk.scaleX = 0.5f;
            trunk.scaleZ = 0.5f;
            trunk.scaleY = trunkLength/2;

            cabin.scaleZ = (cabinLongEnd + cabinShortEnd)/2;
            cabin.scaleX = 0.6f;
            cabin.scaleY = 0.4f;

            float hookElevation = cabin.scaleY;
            hook = new PhysicsBox("cubeTex.png", groundPos + new Vector3(0, trunkLength - hookElevation, upperLimit.y), false);
            magnet = new Magnet("crane/magnet.obj", "crane/magnet.png", hook.pos - new Vector3(0, lowerLimit.y, 0));
            magnet.rotation = Quaternion.FromRotationAroundAxis(Vector3.left,-Mathf.PI/2);
            magnet.scale = 0.2f;
            (magnet.collider as BoxCollider).size = new Vector3(3f, 1f, 3f) * 0.7f;
            magnet.mass = 0.16f;

            hook.scale = 0.4f;
            hook.scaleY /= 3;

            rope = new Rope(hook, magnet, 0.1f);

            r = upperLimit.y;
            y = (lowerLimit.z + upperLimit.z) / 2;

            AddChildren();
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
            (magnet.collider as BoxCollider).DrawExtents();
            //phi
            rope.Apply(Time.deltaTimeS);
            rope.Display();
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

            r += velocity.y;
            if (r<lowerLimit.y) r = lowerLimit.y;
            if (r>upperLimit.y) r = upperLimit.y;

            RotateCabin(Time.deltaTimeS * velocity.x);



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

            y += velocity.z;
            if (y < lowerLimit.z) y = lowerLimit.z;
            if (y > upperLimit.z) y = upperLimit.z;
            rope.length = y;



            //shitty air friction
            magnet.velocity -= magnet.velocity * Time.deltaTimeS * 0.3f;
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
            cabin.position = offset + (dir & new Vector3 (cabinCenterOffset, trunk.scaleY, cabinCenterOffset));
            cabin.rotation = Quaternion.FromRotationAroundAxis(Vector3.up, -phi);
            hook.rotation = cabin.rotation;
            hook.position = dir & new Vector3 (r, cabin.y - cabin.scaleY, r);
        }
    }
}
