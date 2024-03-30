using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace GXPEngine.Editor
{
    public class BoxProxy : Box
    {
        public ConstructorInfo Constructor;
        public Type ObjectType;
        public BoxProxy() : base("editor/defaultCubeTex.png")
        {

        }
    }
}
