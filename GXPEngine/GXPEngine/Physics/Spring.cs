using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.Physics
{
    public class Spring : Constraint
    {
        public PhysicsObject first;
        public PhysicsObject second;
        public float length;
        public float prevLength;
        /// <summary>
        /// how hard it is to squeeze the spring
        /// </summary>
        public float stiffness = 1000f;
        /// <summary>
        /// defines how fast oscillation fade out. Look into damped oscillations for more info. Setting this value to 0 will make it an ideal string.
        /// </summary>
        public float damping = 0.2f;
        public Spring (PhysicsObject g1, PhysicsObject g2, float stiffness = 10f, float length = 0, float damping = 0f) 
        {
            first = g1;
            second = g2;
            prevLength = (g1.TransformPoint(0, 0, 0) - g2.TransformPoint(0, 0, 0)).Magnitude();
            this.length = (length == 0) ? prevLength : length;
            this.stiffness = stiffness;
            this.damping = damping;
        }
        public override void Display()
        {
            Gizmos.DrawLine(first.TransformPoint(0, 0, 0), second.TransformPoint(0, 0, 0));
        }
        public override void Apply(float time)
        {
            float currentLength = (first.TransformPoint(0, 0, 0) - second.TransformPoint(0, 0, 0)).Magnitude();
            float dampingFac = 0;
            if (time > 0)
                dampingFac = (currentLength - prevLength) * damping / time;
            Vector3 dir = (first.TransformPoint(0, 0, 0) - second.TransformPoint(0, 0, 0)) / currentLength;
            Vector3 momentum = (stiffness * (currentLength - length) + dampingFac) * time * dir;
            if (first.simulated)
                first.ApplyMomentum(-momentum);
            if (second.simulated)
                second.ApplyMomentum(momentum);
            prevLength = currentLength;
        }
    }
}
