using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.Physics
{
    public abstract class Constraint
    {
        /// <summary>
        /// Applies the constraint forces to the body
        /// </summary>
        /// <param name="time"></param>
        public abstract void Apply(float time);
        public abstract void Display();
    }
}
