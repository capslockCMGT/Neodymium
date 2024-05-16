using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;

namespace GXPEngine
{
    public class Checkpoint : DSCFSprite
    {
        static List<Checkpoint> Checkpoints = new List<Checkpoint>();
        protected int order;
        public Checkpoint(int order) : base("flag_green.png")
        {
            CreateBoxCollider();
            collider.isTrigger = true;
            Checkpoints.Add(this);
            this.order = order;
            SetOrigin(width/2, height/2);
            size = 0.002f;
        }

        public static void AddCheckpointsToPlayer(Player player)
        {
            if (player == null) return;
            Checkpoints.Sort(delegate(Checkpoint a, Checkpoint b) { return a.order.CompareTo(b.order); });
            foreach(Checkpoint a in Checkpoints)
            {
                player.AddCheckpoint(a);
                if (a.order == Checkpoints.Count - 1)
                    a._texture = new Texture2D("flag_red.png");
            }
            Checkpoints.Clear();
        }

        protected override void RenderSelf(GLContext glContext)
        {
            base.RenderSelf(glContext);
            //Gizmos.DrawBox(Vector3.zero, new Vector3(1,1,1), this, 0xFFFFFFFF, 1);
        }
    }
}
