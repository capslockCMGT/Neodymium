using GXPEngine.Core;
using GXPEngine.Physics;

namespace GXPEngine
{
    public class MetalBox : PhysicsMesh
    {
        public MetalBox(Vector3 pos) :base ("objects/block.obj", "objects/metal_box.png", pos, true)
        {
            
        }
    }
}
