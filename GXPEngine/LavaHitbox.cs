using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;

namespace GXPEngine
{
    public class LavaHitbox : Box
    {
        public LavaHitbox() : base("editor/whitePixel.png",true,true)
        {
            collider.isTrigger = true;
            color = 0xFFAA00;
        }
        protected override void RenderSelf(GLContext glContext)
        {
            if (game is Editor.Exclusives.SceneEditor)
                base.RenderSelf(glContext);
        }
    }
}
