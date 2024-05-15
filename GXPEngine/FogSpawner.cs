using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    public class FogSpawner : ParticleSystem
    {
        readonly float[] curvenums = {0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,.5f,0,0};
        public float maxAlpha = .5f;
        bool yuck = true;
        public FogSpawner() : base("neodymium/fogBall.png",0,0,0)
        {
            //renderAs = new ModelRenderer("test models/monki.obj", "test models/bake.png");
            endSize = 1;
            startSize = 1;
            lifetime = 50;
            spawnPeriod = 0.0001f;
            lifetimeDelta = 20;
            startPosDelta = new Core.Vector3(20, 10, 20);
            endPosDelta = new Core.Vector3(20, 10, 20);
            endColor = Color.White;
            maxParticleCount = 100;
            Func<float,float> curve = a => {
                int sample = Mathf.Floor(Mathf.Clamp(a, 0, 1) * (curvenums.Length - 2));
                return maxAlpha*Mathf.Lerp(a*(curvenums.Length - 2) - sample, curvenums[sample], curvenums[sample+1]);
            };
            alphaCurve = curve;
        }
        public override void Update()
        {
            base.Update();
            if(yuck)
            {
                yuck = false;
                for (int i = 0; i < maxParticleCount; i++)
                    SpawnParticle();
                foreach (var p in particles)
                    p.lifetime = Utils.Random(0, p.totaltime);
            }
            //Console.WriteLine(particleCount);
        }
    }
}
