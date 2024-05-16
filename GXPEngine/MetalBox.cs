using GXPEngine.Core;
using GXPEngine.Physics;

namespace GXPEngine
{
    public class MetalBox : PhysicsMesh
    {
        public MetalBox(Vector3 pos) :base ("objects/block.obj", "objects/metal_box.png", pos, true)
        {
            
        }
        public override void OnEnterField(GameObject c)
        {
            base.OnEnterField(c);
            if(c is WaterHitbox)
            {
                AddChild(new SpatialSound(new Sound("Sounds/Block dropped in water.wav")));
            }
        }
    }
}
