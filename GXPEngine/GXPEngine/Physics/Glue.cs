using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.Physics
{
    public class Glue : Constraint
    {
        PhysicsObject first, second;
        Vector3 offset;
        public Glue(PhysicsObject g1, PhysicsObject g2, Vector3 offset)
        {
            first= g1;
            second= g2;
            this.offset = offset;
            if (!g1.simulated && !g2.simulated)
                Console.WriteLine("'Glue' constraint cannot be applied, at least one object must be dynamic");
            first.Ignore(second);
            second.Ignore(first);
        }
        ~Glue() {
            first.Unignore(second);
            second.Unignore(first);
        }
        public override void Apply(float time)
        {
            Vector3 p = first.momentum + second.momentum;
            Console.WriteLine(first.mass);
            Console.WriteLine(second.mass);
            if (first.simulated && second.simulated)
            {
                second.pos = first.pos + offset;
                first.velocity = p / (first.mass + second.mass);
                second.velocity = first.velocity;
            }
            else if (first.simulated)
            {
                first.position = second.position + offset;
                first.velocity = Vector3.zero;
            }
            else if (second.simulated)
            {
                second.position = first.position - offset;
                second.velocity = Vector3.zero;
            }
        }
        public override void Display()
        {
        }
    }
}
