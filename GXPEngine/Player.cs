using GXPEngine.Core;
using GXPEngine.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    public class Player : PhysicsMesh
    {
        public enum Status
        {
            REST,
            MOVE,
            DEAD
        }

        private float elevation = 0.1f;
        private List<GameObject> path;
        public int currentCheckpointPointer = 0;
        public Status status = Status.REST;
        public GameObject currentCheckpoint 
        {
            get { return path[currentCheckpointPointer]; }
        }
        public GameObject nextCheckpoint
        {
            get { return path[currentCheckpointPointer+1]; }
        }
        public Player(string modelFilename, string textureFilename, Vector3 pos) : base(modelFilename,textureFilename,pos,true)
        {
            SetMass (0.5f);
            (collider as BoxCollider).size = new Vector3(0.8f, 0.8f, 0.8f);
            path = new List<GameObject>();
            AddCheckpoint(pos);
        }
        /// <summary>
        /// returns true if there is a ground ddetected
        /// </summary>
        /// <returns></returns>
        public bool CorrectHeight()
        {
            float dist = 0;
            float minDist = float.MaxValue;
            foreach (PhysicsObject other in collection)
            {
                if (other == null || other == this || toIgnore.Contains(other))
                    continue;
                Vector3[] verts = GetExtents();
                Vector3 normal;
                Check(verts[0] + new Vector3(0.001f, 0.001f, 0.001f));
                Check(verts[1] + new Vector3(0.001f, 0.001f, -0.001f));
                Check(verts[4] + new Vector3(-0.001f, 0.001f, 0.001f));
                Check(verts[5] + new Vector3(-0.001f, 0.001f, -0.001f));
                void Check(Vector3 vert)
                {
                    if (other.collider.RayCast(vert + new Vector3(0, 0.0001f, 0), vert - Vector3.up, out dist, out normal))
                    {
                        if (dist < minDist && normal.y > 0.5f)
                            minDist = dist;
                    }
                }
            }
            if (minDist < elevation)
            {
                if (velocity.y < 0)
                    velocity.y = 0;
                pos.y += elevation - minDist;
                return true;
            }
            return false;
        }
        public void Update()
        {
            if (!dependant)
            {
                if (CorrectHeight() && status == Status.MOVE)
                {
                    Vector3 dir = nextCheckpoint.position - pos;
                    dir.y = 0;
                    float dist = dir.Magnitude();
                    if (dist > scaleX)
                    {
                        dir.Normalize();
                        velocity.x = dir.x * 3;
                        velocity.z = dir.z * 3;
                        rotation = Quaternion.FromRotationAroundAxis(Vector3.up, Mathf.Atan2(dir.x, -dir.z));
                    }
                }
            }
            velocity -= velocity * Time.deltaTimeS * 2;
            DrawPath();

            if (Input.GetKeyDown(Key.ENTER))
            {
                if (status == Status.REST)
                    status = Status.MOVE;
                else if (status == Status.MOVE)
                {
                    velocity.x = 0; velocity.z = 0;
                    status = Status.REST;
                }
            }
        }
        public void DrawPath()
        {
            Gizmos.DrawLine(position, nextCheckpoint.position, color: 0xffffff00);
            for (int i=currentCheckpointPointer + 1; i<path.Count - 1 ; i++)
            {
                Gizmos.DrawLine(path[i].position, path[i + 1].position, color: 0xffffff00);
            }
        }
        public void AddCheckpoint(Vector3 pos)
        {
            Pivot checkpoint = new Pivot();
            checkpoint.scale = 0.1f;
            checkpoint.CreateBoxCollider();
            checkpoint.collider.isTrigger = true;
            checkpoint.position = pos;
            path.Add(checkpoint);
        }
    }
}
