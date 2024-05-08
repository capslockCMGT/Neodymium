using GXPEngine.Core;
using System;
using System.Collections.Generic;


namespace GXPEngine.Physics
{
    
    public class PhysicsMesh : PhysicsObject
    {
        public PhysicsMesh(string modelFilename, string textureFilename, Vector3 pos, bool simulated = true) : base(pos, simulated)
        {
            renderAs = new ModelRenderer(modelFilename, textureFilename);
            (collider as BoxCollider).size = new Vector3(1, 1, 1);
        }
        protected override Collider createCollider()
        {
            return new BoxCollider(this);
        }
        public override Vector3[] GetExtents()
        {
            return renderAs.GetExtents();
        }
    }
}
