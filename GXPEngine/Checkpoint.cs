using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;

namespace GXPEngine
{
    public class Checkpoint : Box
    {
        static List<Checkpoint> Checkpoints = new List<Checkpoint>();
        protected int order;
        public Checkpoint(int order) : base("editor/whitePixel.png")
        {
            collider.isTrigger = true;
            Checkpoints.Add(this);
            this.order = order;
        }

        public static void AddCheckpointsToPlayer(Player player)
        {
            if (player == null) return;
            Checkpoints.Sort(delegate(Checkpoint a, Checkpoint b) { return a.order.CompareTo(b.order); });
            foreach(Checkpoint a in Checkpoints)
            {
                player.AddCheckpoint(a);
            }
            Checkpoints.Clear();
        }

        protected override void RenderSelf(GLContext glContext)
        {
            //Gizmos.DrawBox(Vector3.zero, new Vector3(1,1,1), this, 0xFFFFFFFF, 1);
        }
    }
}
