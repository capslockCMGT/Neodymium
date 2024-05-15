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
        public Vector3 flow = new Vector3(0, 0, 1);
        public WaterHitbox() : base("editor/whitePixel.png", true, true)
        {
            collider.isTrigger = true;
            color = 0x0044FF;
            Bucket.AddHitbox(this);
        }
        protected override void RenderSelf(GLContext glContext)
        {
            if(game is Editor.Exclusives.SceneEditor)
                base.RenderSelf(glContext);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            Bucket.RemoveHitbox(this);
        }
        public void Update()
        {
            foreach ( GameObject go in GetCollisions())
            {
                if (go is Player)
                {
                    Player player = (Player)go;
                    player.status = Player.Status.DEAD;
                    player.AddForce("water", new Physics.Force(flow));
                }
            }
        }
    }
}
