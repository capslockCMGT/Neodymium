using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.Physics
{
    public class Rope : Constraint
    {
        public PhysicsObject first;
        public PhysicsObject second;
        public float length;
        public float prevLength;
        /// <summary>
        /// spring is basically like a surface but from opposite direction
        /// </summary>
        public float bounciness = 1000f;
        public Rope (PhysicsObject g1, PhysicsObject g2, float bounciness = 0.5f, float length = 0) 
        {
            first = g1;
            second = g2;
            prevLength = (g1.TransformPoint(0, 0, 0) - g2.TransformPoint(0, 0, 0)).Magnitude();
            this.length = (length == 0) ? prevLength : length;
            this.bounciness= bounciness;
            if (!g1.simulated && !g2.simulated)
                Console.WriteLine("'Rope' constraint cannot be applied, at least one object must be dynamic");
        }
        public override void Display()
        {
            Gizmos.DrawLine(first.TransformPoint(0, 0, 0), second.TransformPoint(0, 0, 0), color: 0xff333322);
        }
        public override void Apply(float time)
        {
            Vector3 dir = (first.TransformPoint(0, 0, 0) - second.TransformPoint(0, 0, 0));
            float currentLength = dir.Magnitude();
            dir/=currentLength;
            float penetration = currentLength - length;
            if (penetration < 0f)
                return;

            Vector3 relativeVelocity = first.velocity - second.velocity;
            //no need to constrain, if the motion is directed to relaxation

            // if both are simulated, we have to preserve mass center
            if (first.simulated && second.simulated)
            {
                first.pos -= dir * penetration * second.mass/(first.mass + second.mass);
                if (relativeVelocity * dir < 0)
                    return;
                Vector3 deltaP = (1 + bounciness) * (relativeVelocity * (first.mass * second.mass) / (first.mass + second.mass) * dir * dir);
                first.ApplyMomentum(-deltaP);
                second.ApplyMomentum(deltaP);
            }
            else if (first.simulated)
            {
                first.pos -= dir * penetration;
                if (relativeVelocity * dir < 0)
                    return;
                Vector3 deltaP = -(1 + bounciness) * first.mass * first.velocity;
                first.ApplyMomentum(deltaP.ProjectN(dir));
            }
            else if (second.simulated)
            {
                second.pos += dir * penetration;
                if (relativeVelocity * dir < 0)
                    return;
                Vector3 deltaP = -(1 + bounciness) * second.mass * second.velocity;
                second.ApplyMomentum(deltaP.ProjectN(dir));
            }
            prevLength = currentLength;
        }
    }
}
