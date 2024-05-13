using GXPEngine.Core;
using GXPEngine.Physics;
using System.Collections.Generic;

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
        private List<Checkpoint> path;
        public int currentCheckpointPointer = 0;
        public Status status = Status.REST;
        //temporarily for technical reasons current checkpoint = next checkpoint :(
        public Checkpoint currentCheckpoint 
        {
            get { return path[currentCheckpointPointer]; }
        }
        public Checkpoint nextCheckpoint
        {
            get { return path[currentCheckpointPointer+1]; }
        }
        public Player(string modelFilename, string textureFilename, Vector3 pos) : base(modelFilename,textureFilename,pos,true)
        {
            SetMass(0.5f);
            (collider as BoxCollider).size = new Vector3(0.8f, 0.8f, 0.8f);
            path = new List<Checkpoint>();
        }
        /// <summary>
        /// returns true if there is a ground d etected
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
                    Vector3 dir = currentCheckpoint.position - pos;
                    dir.y = 0;
                    float dist = dir.Magnitude();
                    if (dist > scaleX)
                    {
                        dir.Normalize();
                        velocity.x = dir.x * 3;
                        velocity.z = dir.z * 3;
                        rotation = Quaternion.FromRotationAroundAxis(Vector3.up, Mathf.Atan2(dir.x, -dir.z));
                    }
                    velocity.x -= velocity.x * Time.deltaTimeS * 3;
                    velocity.z -= velocity.z * Time.deltaTimeS * 3;
                }
                if (CorrectHeight() && status == Status.REST)
                {
                    velocity.x -= velocity.x * Time.deltaTimeS * 3;
                    velocity.z -= velocity.z * Time.deltaTimeS * 3;
                }
            }
            DrawPath();
            (collider as BoxCollider).DrawExtents();

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

            UpdateCheckpoint();
        }
        public void DrawPath()
        {
           // if (path.Count < 2) return;
            Gizmos.DrawLine(TransformPoint(0,0,0), currentCheckpoint.TransformPoint(0,0,0), color: 0xffffff00);
            for (int i=currentCheckpointPointer; i<path.Count - 1 ; i++)
            {
                Gizmos.DrawLine(path[i].TransformPoint(0, 0, 0), path[i + 1].TransformPoint(0, 0, 0), color: 0xffffff00);
            }
        }
        public void AddCheckpoint(Vector3 pos, int order)
        {
            Checkpoint checkpoint = new Checkpoint(order);
            checkpoint.scale = 0.1f;
            checkpoint.CreateBoxCollider();
            checkpoint.collider.isTrigger = true;
            checkpoint.position = pos;
            path.Add(checkpoint);
        }
        public void AddCheckpoint(Checkpoint c)
        {
            path.Add(c);
        }
        public void UpdateCheckpoint()
        {
            if (HitTest(currentCheckpoint))
            {

                status = Status.REST;
                if (currentCheckpointPointer < path.Count - 1)
                    currentCheckpointPointer++;
            }
        }
    }
}
