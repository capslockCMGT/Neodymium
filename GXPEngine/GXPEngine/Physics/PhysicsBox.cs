
using GXPEngine.Core;
using System.Collections.ObjectModel;

namespace GXPEngine.Physics
{

    public class PhysicsBox : PhysicsObject
    {
        public PhysicsBox(string filename, Vector3 pos, bool simulated = true) : base(pos, simulated)
        {
            renderAs = new Box(filename);
            (collider as BoxCollider).size = new Vector3(1, 1, 1);
        }
        public PhysicsBox(Texture2D texture, bool addCollider = true) : base(Vector3.zero, false)
        {
            renderAs = new Box(texture);
            (collider as BoxCollider).size = new Vector3(1, 1, 1);
        }
        protected override Collider createCollider()
        {
            return new BoxCollider(this);
        }
        public override Vector3[] GetExtents()
        {
            return (collider as BoxCollider).GetExtents();
        }
    }
}
