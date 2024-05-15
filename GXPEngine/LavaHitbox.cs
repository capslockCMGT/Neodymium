using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;
using GXPEngine.Physics;

namespace GXPEngine
{
    public class LavaHitbox : Box
    {
        public LavaHitbox() : base("editor/whitePixel.png",true,true)
        {
            collider.isTrigger = true;
            color = 0xFFAA00;
            Bucket.AddHitbox(this);
        }
        protected override void RenderSelf(GLContext glContext)
        {
            if (game is Editor.Exclusives.SceneEditor)
                base.RenderSelf(glContext);
        }
        void Update()
        {
            GameObject[] slop = GetCollisions();
            foreach(var j in slop)
            {
                if(!(j is PhysicsObject)) continue;
                if (!(j as PhysicsObject).simulated || j is Magnet) continue;
                j.Destroy();
            }
        }
        public void TurnIntoObsidian()
        {
            Obsidian obs = FindObjectOfType<Obsidian>();
            if (obs == null) return;
            obs.Enable();
            obs.renderAs.visible = true;
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            Bucket.RemoveHitbox(this);
        }
    }
}
