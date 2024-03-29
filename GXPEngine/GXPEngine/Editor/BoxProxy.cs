using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.Editor
{
    public class BoxProxy : Box
    {
        public List<CustomProperty> properties = new List<CustomProperty>();
        public Type ObjectType;
        public BoxProxy() : base("editor/defaultCubeTex.png")
        {

        }
    }
}
