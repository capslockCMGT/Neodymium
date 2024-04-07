using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GXPEngine
{
    public class Player : GameObject
    {
        BoxCollider collider;
        public List<Collider> colliders;
        Camera cam;
        Vector3 size = new Vector3(0.1f, 0.5f, 0.1f);
        Vector3 velocity = new Vector3(0, 0, 0);
        Vector3 groundNormal = Vector3.up;
        public bool grounded = false;

        float cameraTilt, targetTilt = 0.1f;
        Vector3 camOffset = new Vector3(0, 0, 0);
        Vector3 targetOffset = new Vector3(0.3f,0.1f,0);
        bool xChanged;

        Timer tiltTimer;
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
            tiltTimer = new Timer();
            tiltTimer.SetLaunch(0.5f);
            tiltTimer.autoReset= true;
            tiltTimer.OnTimerEnd += ChangeShakeDirection;
        }


        public void AssignCamera(Camera cam)
        {
            this.cam = cam;
        }
        public void Update()
        {
            position += velocity * Time.deltaTimeS;
            Vector3[] extents = GetExtents();
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
            if (cam != null)
                cam.position = TransformPoint(camOffset);
            if (grounded)
            {
                if (Input.GetKeyDown(Key.SPACE))
                {
                    velocity.y += 3;
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
            float msex = Input.mouseX / 800f * Mathf.PI;
            float msey = Input.mouseY / 600f * Mathf.PI;
            cam.rotation = (Quaternion.FromRotationAroundAxis(0, 1, 0, msex));
            rotation = cam.rotation;
            cam.Rotate(Quaternion.FromRotationAroundAxis(1, 0, 0, msey));
            //minecraft creative mode controls
            if (Input.GetKey(Key.D))
                position += (TransformDirection(new Vector3(0, 0, -1)) ^ groundNormal).normalized() * Time.deltaTimeS;
            if (Input.GetKey(Key.A))
                position += (TransformDirection(new Vector3(0, 0, 1)) ^ groundNormal).normalized() * Time.deltaTimeS;
            if (Input.GetKey(Key.W))
                position += (TransformDirection(new Vector3(-1, 0, 0)) ^ groundNormal).normalized() * Time.deltaTimeS;
            if (Input.GetKey(Key.S))
                position += (TransformDirection(new Vector3(1, 0, 0)) ^ groundNormal).normalized() * Time.deltaTimeS;
            if ((Input.GetKey(Key.D) || Input.GetKey(Key.A) || Input.GetKey(Key.W) || Input.GetKey(Key.S)) && grounded)
                CameraShake();
            else
            {
                camOffset = Vector3.Lerp(Time.deltaTimeS, camOffset, Vector3.zero);
                cameraTilt = Mathf.Lerp(Time.deltaTimeS,cameraTilt, 0);
                cam.Rotate(Quaternion.FromRotationAroundAxis(new Vector3(0, 0, 1), cameraTilt));
            }
                
        }
        public void DrawBB()
        {
            if (collider != null)
            {
                Gizmos.DrawBox(0, 0, 0, size.x, size.y, size.z, this, color: 0xff00ffff);
            }
        }
        public void CameraShake()
        {
            if (tiltTimer.time < tiltTimer.interval / 6 || tiltTimer.time > tiltTimer.interval * 2 / 3)
                targetOffset.y = -0.2f;
            else
                targetOffset.y = +0.2f;


            camOffset.x = Mathf.Lerp(Time.deltaTimeS, camOffset.x, targetOffset.x);
            camOffset.y = Mathf.Lerp(Time.deltaTimeS, camOffset.y, targetOffset.y);
            cameraTilt = camOffset.x * 0.7f;
            cam.Rotate(Quaternion.FromRotationAroundAxis(new Vector3(0, 0, 1), cameraTilt));
        }
        public void ChangeShakeDirection()
        {
            targetOffset.x = -targetOffset.x;
        }
    }
}
