using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GXPEngine.Physics
{
    
    public class PhysicsMesh : PhysicsObject
    {
        public PhysicsMesh(string modelFilename, string textureFilename, Vector3 pos, bool simulated = true) : base(pos, simulated)
        {
            renderAs = new ModelRenderer(modelFilename, textureFilename);
            (collider as BoxCollider).size = new Vector3(1, 1, 1);
        }
        protected override Collider createCollider()
        {
            return new BoxCollider(this);
        }
        public override Vector3[] GetExtents()
        {
            return (collider as BoxCollider).GetExtents();
        }
        public override void OnEnterField(GameObject c)
        {
            base.OnEnterField(c);
            if (c is WaterHitbox)
            {
                WaterHitbox wh = (WaterHitbox)c;
                ParticleSystem waterParticles = new ParticleSystem("neodymium/bucket/water drip.png",0,0,0,mode:ParticleSystem.Mode.force);
                waterParticles.startPosDelta = (collider as BoxCollider).size & new Vector3(1, 0.5f, 1);
                waterParticles.startSpeed = wh.flow * 0.1f;
                waterParticles.startSpeedDelta = new Vector3(0.05f, 0.05f, 0.05f);
                waterParticles.lifetime = 0.5f;
                waterParticles.spawnPeriod = 0.01f;
                waterParticles.lifetimeDelta = 0.1f;
                waterParticles.forces.Add(new ParticleSystem.GravityForce(new Vector3(0,gravity,0)));
                waterParticles.name = "waterParticles";
                //waterParticles.startSize = 0.001f;
                //waterParticles.endSize = 0.0005f;
                waterParticles.endColor = Color.White;

                ModelRenderer particle = new ModelRenderer("trail.obj", "neodymium/Liquids/Water.png");
                particle.color = 0xffffffff;
                waterParticles.renderAs = particle;

                AddChild(waterParticles);
            }
        }
        public override void OnFieldRemain(GameObject c)
        {
            base.OnFieldRemain(c);
            if (c is WaterHitbox)
            {
                WaterHitbox wh = (WaterHitbox)c;
            }
        }
        public override void OnLeaveField(GameObject c)
        {
            base.OnLeaveField(c);
            if (c is WaterHitbox)
            {
                WaterHitbox wh = (WaterHitbox)c;
                GameObject particles = containsWaterParticles();
                if (particles != null)
                    particles.Remove();
            }
        }
        GameObject containsWaterParticles()
        {
            List<GameObject> children = GetChildren();
            for (int i = children.Count - 1; i >= 0; i--)
            {
                if (children[i].name == "waterParticles")
                    return children[i];
            }
            return null;
        }
    }
}
