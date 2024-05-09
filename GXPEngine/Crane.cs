using GXPEngine.Core;
using GXPEngine.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    public class Crane
    {
        public PhysicsObject trunk;
        public PhysicsObject cabin;
        public PhysicsObject hook;
        public PhysicsObject magnet;
        public Rope rope;

        //magnet has 3 freedom degrees, which can be represented in cylindric coordinate system
        float phi = 0, r = 0, y = 0;
        float rMin = 2f, rMax = 6f, yMin = 1f, yMax = 5f;
        float cabinCenterOffset;
        public Crane(Vector3 groundPos)
        {
            float trunkLength = 8f;
            float cabinLongEng = rMax+1;
            float cabinShortEnd = 1;
            cabinCenterOffset = (cabinLongEng + cabinShortEnd) / 2 - cabinShortEnd;

            trunk = new PhysicsBox("cubeTex.png", groundPos + new Vector3(0, trunkLength / 2, 0),false);
            cabin = new PhysicsBox("cubeTex.png", groundPos + new Vector3(0, trunkLength, cabinCenterOffset), false);

            trunk.scaleX = 0.5f;
            trunk.scaleZ = 0.5f;
            trunk.scaleY = trunkLength/2;

            cabin.scaleZ = (cabinLongEng + cabinShortEnd)/2;
            cabin.scaleX = 0.6f;
            cabin.scaleY = 0.4f;

            float hookElevation = cabin.scaleY;
            hook = new PhysicsBox("cubeTex.png", groundPos + new Vector3(0, trunkLength - hookElevation, rMax), false);
            magnet = new PhysicsMesh("test models/monki.obj", "test models/suzanne.png", hook.pos - new Vector3(0, rMin, 0));

            hook.scale = 0.4f;
            hook.scaleY /= 3;

            magnet.scale = 0.4f;
            rope = new Rope(hook, magnet, 0.1f);
        }
        public void AddToGame(Game game)
        {
            game.AddChild(trunk);
            game.AddChild(cabin);
            game.AddChild(hook);
            game.AddChild(magnet);
        }
        public void Update()
        {
            rope.Apply(Time.deltaTimeS);
            rope.Display();
            if (Input.GetKey(Key.E))
            {
                hook.position += new Vector3(hook.position.z * Time.deltaTimeS, 0, -hook.position.x * Time.deltaTimeS);
                RotateCabin(Time.deltaTimeS);
            }
            if (Input.GetKey(Key.Q))
            {
                hook.position -= new Vector3(hook.position.z * Time.deltaTimeS, 0, -hook.position.x * Time.deltaTimeS);
                RotateCabin(-Time.deltaTimeS);
            }

            if (Input.GetKey(Key.Z))
            {
                hook.position += new Vector3(hook.position.x * Time.deltaTimeS / 2, 0, hook.position.z * Time.deltaTimeS / 2);

            }
            if (Input.GetKey(Key.X))
            {
                hook.position -= new Vector3(hook.position.x * Time.deltaTimeS / 2, 0, hook.position.z * Time.deltaTimeS / 2);

            }
            if (Input.GetKey(Key.R))
            {
                if (rope.length > 1)
                    rope.length -= Time.deltaTimeS;

            }
            if (Input.GetKey(Key.F))
            {
                rope.length += Time.deltaTimeS;

            }
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
        }
    }
}
