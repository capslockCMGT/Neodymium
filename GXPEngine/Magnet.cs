using GXPEngine.Core;
using GXPEngine.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    public class Magnet : PhysicsMesh
    {
        //how to get a gf, 1 step tutorial:
        public bool isAttracting;
        public Magnet(string modelFilename, string textureFilename, Vector3 pos, bool simulated = true) : base(modelFilename, textureFilename, pos, simulated)
        {
            
        }
        public override void OnCollision(Collision col)
        {
            if (!isAttracting) return;
            base.OnCollision(col);
            PhysicsObject toPick = (PhysicsObject)((col.self is Magnet) ? col.other : col.self);
            if (toPick.simulated)
            {
                Glue(toPick);
                toPick.pos = new Vector3(0, -1.4f, 0);
            }
        }
        public void Update()
        {
            if (Input.GetKeyDown (Key.T))
            isAttracting = !isAttracting;
            if (isAttracting) (renderAs as ModelRenderer).color = 0xffffff;
            else (renderAs as ModelRenderer).color = 0x555555;
        }
    }
}
