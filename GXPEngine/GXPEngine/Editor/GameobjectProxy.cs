using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;

namespace GXPEngine.Editor
{
    public class GameobjectProxy : DSCFSprite
    {
        public List<CustomProperty> properties = new List<CustomProperty>();
        public Type ObjectType;
        float radius = .25f;
        public GameobjectProxy() : base("editor/ProxyLogo")
        {
            SetOrigin(width * .5f,height * .5f);
        }

        protected override void RenderSelf(GLContext glContext)
        {
            Vector3 baseScale = scaleXYZ;
            scaleXYZ *= .1f/game.width;
            base.RenderSelf(glContext);
            scaleXYZ = baseScale;
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
