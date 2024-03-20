using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;

namespace GXPEngine.Editor
{
    public class GameobjectProxy : GameObject
    {
        public List<CustomProperty> properties = new List<CustomProperty>();
        public string ObjectType;
        float radius = .25f;
        public GameobjectProxy()
        {
        }

        protected override void RenderSelf(GLContext glContext)
        {
            Gizmos.DrawPlus(Vector3.zero, radius, this, 0xFFFFFFFF);
        }
        protected override Collider createCollider()
        {
            return new BoxCollider3D(this);
        }
        public override Vector3[] GetExtents()
        {
            Vector3[] res = new Vector3[8];
            for (int i = 0; i < 8; i++)
                res[i] = TransformPoint((i & 1) != 0 ? -radius : radius, (i & 2) != 0 ? -radius : radius, (i & 4) != 0 ? -radius : radius);
            return res;
        }
    }
}
