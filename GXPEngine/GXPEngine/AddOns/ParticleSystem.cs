using GXPEngine.Core;
using GXPEngine.Editor.Exclusives;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace GXPEngine
{
    public partial class ParticleSystem : GameObject
    {
        public class Force
        {
            public ForceType type;
            public float magnitude = 1f;

            public virtual Vector3 Calculate(Vector3 pos)
            { return Vector3.zero; }
        }


        public class GravityForce : Force
        {
            public Vector3 direction;

            public GravityForce(Vector3 dir)
            {
                type = ForceType.Gravity;
                direction = dir.normalized();
            }
            public override Vector3 Calculate(Vector3 pos)
            {
                return direction * magnitude;
            }
        }

        public class RadialForce : Force
        {
            public Vector3 affectorPos;

            public RadialForce(Vector3 affectorPos)
            {
                type = ForceType.Radial;
                this.affectorPos = affectorPos;
            }
            public override Vector3 Calculate(Vector3 pos)
            {
                float r = (pos - affectorPos).Magnitude();
                return magnitude / r / r / r * (pos - affectorPos);
            }
        }
        protected class Particle : DSCFSprite
        {
            protected GameObject _renderAs = null;
            public GameObject renderAs
            {
                get {  return _renderAs; }
                set { _renderAs = value; }
            }
            public Func<float, float> alphaCurve;
            public ParticleSystem ps;
            public Mode mode = Mode.position;
            public Particle(Texture2D texture) : base(texture, false)
            {
                SceneEditor.AddToUpdate(this);
            }
            public Particle(string filename, bool keepInCache = true, bool addCollider = false) : base(filename, keepInCache, addCollider)
            {
                SceneEditor.AddToUpdate(this);
            }
            public float lifetime;
            public float totaltime;
            public Vector3 spawnPos;
            public Vector3 endPos;

            public Vector3 startSpeed;
            public Vector3 endSpeed;

            public Color startColor;
            public Color endColor;

            public float startSize;
            public float endSize;

            public float startAlpha;
            public float endAlpha;

            public Vector3 speed = Vector3.zero;

            public GameObject cam = null;
            protected override void RenderSelf (GLContext glContext)
            {
                if (renderAs == null)
                    base.RenderSelf(glContext);
                else
                    renderAs.Render(glContext);
            }
            public void Update()
            {
                lifetime += Time.deltaTimeS;
                float fac = lifetime / totaltime;
                //if (cam != null)
                //    rotation = Quaternion.LookTowards(cam.position - position, new Vector3(0, 0, 1));
                switch (mode)
                {
                    case Mode.position:
                        x = Mathf.Lerp(fac, spawnPos.x, endPos.x);
                        y = Mathf.Lerp(fac, spawnPos.y, endPos.y);
                        z = Mathf.Lerp(fac, spawnPos.z, endPos.z);
                        break;
                    case Mode.velocity:
                        speed = Vector3.Lerp(fac, startSpeed, endSpeed);
                        break;
                    case Mode.force:
                        foreach (Force f in ps.forces)
                        {
                            Vector3 force = f.Calculate(position) * Time.deltaTimeS;
                            if (force.Magnitude() > 10)
                                force = force.normalized() * 10f;
                            speed += force;
                        }
                        break;
                }
                position += speed;

                SetColor(
                    (byte)Mathf.Lerp(fac, startColor.R, endColor.R),
                    (byte)Mathf.Lerp(fac, startColor.G, endColor.G),
                    (byte)Mathf.Lerp(fac, startColor.B, endColor.B));

                scale = Mathf.Lerp(fac, startSize, endSize);
                if (alphaCurve == null)
                    alpha = Mathf.Lerp(fac, startAlpha, endAlpha);
                else
                    alpha = alphaCurve(fac);

                if (totaltime < lifetime)
                {
                    Destroy();
                    ps.particles.Remove(this);
                }
            }
        }
        public enum ForceType
        { 
            Gravity = 0,
            Radial = 1,
            Turbulence = 2,
            Custom = 3,
        }
        public enum EmitterType
        {
            rect = 0,
            circle = 1
        }
        public enum Mode
        {
            force = 0,
            position = 1,
            velocity = 2,
        }
        public GameObject worldSpace = null;
        public GameObject renderAs = null;
        public BlendMode blendMode = BlendMode.NORMAL;
        public EmitterType emitterType;
        public Mode mode;

        public Func<float,float> alphaCurve = null;
        public ParticleSystem(string path, float x, float y, float z, EmitterType emitter = EmitterType.circle, Mode mode = Mode.position, GameObject worldSpace = null, GameObject cam = null) : base (false)
        {
            SceneEditor.AddToUpdate(this);
            if (!File.Exists(path)) enabled = false; 
            forces = new List<Force>();
            particles = new List<Particle>();
            this.texturePath = path;
            this.emitterType = emitter;
            this.mode = mode;
            position = new Vector3(x, y, z);

            this.cam = cam;
            if (worldSpace == null)
                this.worldSpace = game;
            else
                this.worldSpace = worldSpace;
        }
        public bool pixelated = false;

        public float radius;
        public Vector3 size;

        public string texturePath = "circle.png";
        protected List<Particle> particles = null;
        public List<Force> forces = null;
        public GameObject cam = null;
        public bool enabled = true;
        public int particleCount { get { return particles.Count; } }

        public float lifetime = 0.5f;
        public float lifetimeDelta = 0.1f;

        public int maxParticleCount = 100;
        public float spawnPeriod = 0.03f;
        private float spawnCooldown = 0;

        public Vector3 startSpeed = Vector3.zero;
        public Vector3 startSpeedDelta = Vector3.zero;
        public Vector3 endSpeed = new Vector3(0, 0.05f, 0);
        public Vector3 endSpeedDelta = new Vector3(0.01f, 0.01f, 0.01f);

        public Vector3 startPos = Vector3.zero;
        public Vector3 startPosDelta = Vector3.zero;
        public Vector3 endPos = new Vector3(0,1,0);
        public Vector3 endPosDelta = new Vector3(0, 1, 0);

        //public Quaternion startRotation = new Quaternion(0, 0, 0, 0);
        //public Quaternion startRotationDelta = new Quaternion(0, 0, 0, 0);
        //public Quaternion endAngle = new Quaternion(0, 0, 0, 0);
        //public Quaternion endAngleDelta = new Quaternion(0, 0, 0, 0);
        public float startAngularSpeed = 0;
        public float endAngularSpeed = 0;

        public Color startColor = Color.White;
        public Color endColor = Color.Blue;

        public float startSize = 0.2f;
        public float startSizeDelta = 0f;
        public float endSize = 0.1f;
        public float endSizeDelta = 0f;

        public float startAlpha = 1f;
        public float endAlpha = 0f;

        public void SpawnParticle()
        {
            Particle p = new Particle(texturePath);
            p.ps = this;
            p.mode = mode;
            p.lifetime = 0;

            p.pixelated = pixelated;
            p.SetOrigin(p.width / 2, p.height / 2);
            //p.startAngle = Utils.Random(startAngle - startAngleDelta, startAngle + startAngleDelta);
            //p.endAngle = Utils.Random(endAngle -endAngleDelta, endAngle + endAngleDelta);
            p.cam = cam;

            p.spawnPos = TransformPoint(Utils.Random(startPos, startPosDelta));
            
            p.position = p.spawnPos;

            p.totaltime = Utils.Random(lifetime - lifetimeDelta, lifetime + lifetimeDelta);
            p.blendMode = blendMode;

            switch(mode)
            {
                case Mode.position:
                    p.endPos = TransformPoint(Utils.Random(endPos, endPosDelta));
                    p.speed = Vector3.zero;
                    break;
                case Mode.velocity:
                    p.startSpeed = Utils.Random(startSpeed, startSpeedDelta);
                    p.endSpeed = Utils.Random(endSpeed, endSpeedDelta);
                    break;
                case Mode.force:
                    p.speed = Utils.Random(startSpeed, startSpeedDelta);
                    break;
            }
            p.startColor = startColor; p.endColor = endColor;
            p.startAlpha = startAlpha; p.endAlpha = endAlpha;
            p.alphaCurve = alphaCurve;
            p.startSize = Utils.Random(startSize- startSizeDelta, startSize + startSizeDelta); p.endSize = Utils.Random(endSize - endSizeDelta, endSize + endSizeDelta);

            p.SetColor(startColor.R, startColor.G, startColor.B);
            if (p.alphaCurve != null)
                p.alpha = alphaCurve(0);
            else
                p.alpha = startAlpha;
            p.size = startSize;
            p.scale = startSize;
            p.renderAs = renderAs;

            particles.Add(p);
            worldSpace.AddChild(p);
        }
        public virtual void Update()
        {
            if (Input.GetKeyDown(Key.N))
            {
                enabled = !enabled;
            }
            if (enabled && particles.Count < maxParticleCount)
            {
                spawnCooldown += Time.deltaTimeS;
                int particlesGenerated = (int)(spawnCooldown / spawnPeriod);
                spawnCooldown %= spawnPeriod;
                for (int i = 0; i < particlesGenerated; i++)
                {
                    SpawnParticle();
                }
                
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            DestroyAllParticles();
        }
        public void DestroyAllParticles()
        {
            for (int i=particles.Count - 1; i>=0; i--)
            {
                particles[i].LateDestroy();
                particles[i].Remove();
            }
            particles.Clear();
        }
    }
}
