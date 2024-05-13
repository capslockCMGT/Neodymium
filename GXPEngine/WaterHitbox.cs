using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;

namespace GXPEngine
{
    public class WaterHitbox : Box
    {
        public WaterHitbox() : base("editor/whitePixel.png", true, true)
        {
            collider.isTrigger = true;
            color = 0x0044FF;
        }
        protected override void RenderSelf(GLContext glContext)
        {
            if(game is Editor.Exclusives.SceneEditor)
                base.RenderSelf(glContext);
        }
    }
}
