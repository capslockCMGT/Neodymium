using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    public class Player : GameObject
    {
        BoxCollider collider;
        public List<Collider> colliders;
        Camera cam;
        Vector3 camOffset = new Vector3(0, 0, 0);
        Vector3 size = new Vector3(0.1f, 0.5f, 0.1f);
        Vector3 velocity = new Vector3(0, 0, 0);
        Vector3 groundNormal = Vector3.up;
        public bool grounded = false;
        public override Vector3[] GetExtents()
        {
            Vector3[] res = new Vector3[]
            {
                new Vector3(-size.x, size.y, size.z),
                new Vector3( size.x, size.y, size.z),
                new Vector3( size.x,-size.y, size.z),
                new Vector3(-size.x,-size.y, size.z),
                new Vector3(-size.x, size.y,-size.z),
                new Vector3( size.x, size.y,-size.z),
                new Vector3( size.x,-size.y,-size.z),
                new Vector3(-size.x,-size.y,-size.z)
            };
            for (int i = 0; i < 8; i++)
                res[i] = TransformPoint(res[i]);
            return res;
        }
        public Player() : base() 
        {
            collider = new BoxCollider(this);
            colliders = new List<Collider>();
        }
        public void AssignCamera(Camera cam)
        {
            this.cam = cam;
        }
        public void Update()
        {
            position += velocity * Time.deltaTimeS;
            Vector3[] extents = GetExtents();
            if (cam!= null)
            {
                cam.position = TransformPoint(camOffset);
            }
            if (extents[2].y < -2)
            {
                y += -extents[2].y-2;
                grounded = true;
            }
            foreach (Collider coll in colliders)
            {
                Collision collision = collider.GetCollisionInfo(coll);
                if (collision != null)
                {
                    position -= collision.normal * collision.penetrationDepth;
                    if (collision.normal.y < -0.5f)
                    {
                        groundNormal = -collision.normal;
                        grounded = true;
                    }
                }
                else
                {
                    float distance = float.MaxValue;
                    float minDist = float.MaxValue;
                    Vector3 normal = Vector3.zero;
                    if (extents[2].y > -2 && grounded)
                    {
                        if (coll.RayCast(extents[2], extents[2] - Vector3.up, out distance, out normal))
                            if (minDist > distance) { minDist = distance; }
                        if (coll.RayCast(extents[3], extents[3] - Vector3.up, out distance, out normal))
                            if (minDist > distance) { minDist = distance; }
                        if (coll.RayCast(extents[6], extents[6] - Vector3.up, out distance, out normal))
                            if (minDist > distance) { minDist = distance; }
                        if (coll.RayCast(extents[7], extents[7] - Vector3.up, out distance, out normal))
                            if (minDist > distance) { minDist = distance; }
                        if (minDist > 0.01f) grounded = false;
                    }
                }
            }
            ControlsUpdate();
            if (grounded)
            {
                if (Input.GetKeyDown(Key.SPACE))
                {
                    velocity.y += 5;
                    grounded = false;
                }
                else
                    velocity.y = 0;
            }
            else
            {
                groundNormal = Vector3.up;
                velocity.y -= 10 * Time.deltaTimeS;
            }
        }
        public void ControlsUpdate()
        {
            //minecraft creative mode controls
            if (Input.GetKey(Key.D))
                position += (TransformDirection(new Vector3(0, 0, -1)) ^ groundNormal).normalized() * Time.deltaTimeS;
            if (Input.GetKey(Key.A))
                position += (TransformDirection(new Vector3(0, 0, 1)) ^ groundNormal).normalized() * Time.deltaTimeS;
            Console.WriteLine(groundNormal);
            if (Input.GetKey(Key.W))
                position += (TransformDirection(new Vector3(-1, 0, 0)) ^ groundNormal).normalized() * Time.deltaTimeS;
            if (Input.GetKey(Key.S))
                position += (TransformDirection(new Vector3(1, 0, 0)) ^ groundNormal).normalized() * Time.deltaTimeS;
        }
        public void DrawBB()
        {
            if (collider != null)
            {
                Gizmos.DrawBox(0, 0, 0, size.x, size.y, size.z, this, color: 0xff00ffff);
            }
        }
    }
}
