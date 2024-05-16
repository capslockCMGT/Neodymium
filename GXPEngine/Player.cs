using GXPEngine.Core;
using GXPEngine.Physics;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GXPEngine
{
    public class Player : PhysicsMesh
    {
        public enum Status
        {
            REST,
            MOVE,
            DEAD,
            FALLEN
        }

        private float elevation = 0.3f;
        private List<Checkpoint> path;
        private ParticleSystem trail;
        public int currentCheckpointPointer = 0;
        public Status status = Status.REST;
        public delegate void NoArgs();
        public event NoArgs finished;
        SpatialSound movingAudio;
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
            trail = new ParticleSystem("cubeTex.png", 0, -1, 0.5f, mode:ParticleSystem.Mode.velocity);
            trail.startPosDelta = new Vector3(0.5f, 0, 0);
            trail.spawnPeriod = 0.02f;
            trail.enabled = false;
            trail.renderAs = new ModelRenderer("trail.obj","editor/whitePixel.png");
            trail.startSize = 0.2f * scaleX;
            trail.endSize = 0.1f * scaleX;
            trail.endSpeed =  new Vector3(0,0.01f,0);
            AddChild(trail);
            movingAudio = new SpatialSound(new Sound("Sounds/robot movement 2.wav", true, true), .1f);
            AddChild(movingAudio);
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
                Check(verts[0] + TransformDirection(new Vector3(0.2f, 0.2f, 0.2f)));
                Check(verts[1] + TransformDirection(new Vector3(0.2f, 0.2f, -0.2f)));
                Check(verts[4] + TransformDirection(new Vector3(-0.2f, 0.2f, 0.2f)));
                Check(verts[5] + TransformDirection(new Vector3(-0.2f, 0.2f, -0.2f)));

                //this mf might get stuck midplatform </3
                Check((verts[0] + verts[5]) / 2);

                void Check(Vector3 vert)
                {
                    if (other.collider.RayCast(vert + new Vector3(0, 0, 0), vert - Vector3.up * elevation, out dist, out normal))
                    {
                        //Console.WriteLine(normal.y);
                        if ((other is Magnet)) return;
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
                    trail.enabled = true;
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
                    trail.startSpeed = - dir * 0.01f;
                }
                if (CorrectHeight() && status == Status.REST)
                {
                    trail.enabled = false;
                    velocity.x -= velocity.x * Time.deltaTimeS * 3;
                    velocity.z -= velocity.z * Time.deltaTimeS * 3;
                }
            }
            if (status == Status.DEAD)
                trail.enabled = false;
            if (Input.GetKeyDown(Key.ENTER) && Neodymium.controlsEnabled)
            {
                if (status == Status.REST)
                    status = Status.MOVE;
                else if (status == Status.MOVE)
                {
                    velocity.x = 0; velocity.z = 0;
                    status = Status.REST;
                }
            }

            movingAudio.halfVolumeDistance = Mathf.Min(10, (velocity.x * velocity.x + velocity.z * velocity.z));
            UpdateCheckpoint();
            if (y < -10 && status != Status.FALLEN)
            {
                status = Status.FALLEN;
                AddChild(new SpatialSound(new Sound("Sounds/Ody falling off mountain.wav")));
            }
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
                else
                {
                    finished?.Invoke();
                    finished = null;
                }
            }
        }

        public override void OnEnterField(GameObject c)
        {
            base.OnEnterField(c);
            if (c is WaterHitbox)
                status = Status.DEAD;
        }
        public override void OnFieldRemain(GameObject c)
        {
            base.OnFieldRemain(c);
            if (c is WaterHitbox)
            {
                WaterHitbox wh = (WaterHitbox)c;
                AddForce("water", new Force(wh.flow));
            }
        }
        public override void OnLeaveField(GameObject c)
        {
            base.OnLeaveField(c);
            if (c is WaterHitbox)
                RemoveForce("water");
        }
    }
}
