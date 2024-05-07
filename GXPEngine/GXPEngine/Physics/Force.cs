using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.Physics
{
    public class Force
    {
        /// <summary>
        /// vector from the force origin to the mass center of the object
        /// </summary>
        public Vector3 r;
        public Vector3 force;

        public Force(Vector3 force, Vector3 r)
        {
            this.force = force;
            this.r = r;
        }
        public Force(Vector3 force) : this(force, new Vector3(0,0,0))
        {
        }
    }
}
