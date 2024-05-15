using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Physics;
using GXPEngine.Core;

namespace GXPEngine
{
    public class Bucket : PhysicsMesh
    {
        static List<Box> hitboxes = new List<Box>();
        public static void AddHitbox(Box box)
        { hitboxes.Add(box); }
        public static void RemoveHitbox(Box box)
        { hitboxes.Remove(box); }
        bool filledWithWater;
        public Bucket(string modelName, string textureName) : base(modelName, textureName, Vector3.zero)
        {
            renderAs.scale = .8f;
            renderAs.y -= 1f;
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            foreach(Box box in hitboxes)
            {
                if (!box.collider.HitTest(collider)) continue;
                if (box is WaterHitbox && !filledWithWater)
                {
                    filledWithWater = true;
                }
                if (box is LavaHitbox && filledWithWater)
                {
                    (box as LavaHitbox).TurnIntoObsidian();
                }
            }
        }
    }
}
