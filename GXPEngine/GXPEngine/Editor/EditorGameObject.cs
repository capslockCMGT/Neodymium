using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;

namespace GXPEngine.Editor
{
    public class EditorGameObject : DSCFSprite
    {
        ConstructorInfo Constructor;
        Type ObjectType;
        object[] ConstructorParameters;
        GameObject EditorDisplayObject;
        float radius = .25f;
        public EditorGameObject(Type objectType, ConstructorInfo constructor) : base("editor/ProxyLogo.png")
        {
            SetOrigin(width * .5f,height * .5f);
            ObjectType = objectType;
            Constructor = constructor;
            ConstructorParameters = new object[Constructor.GetParameters().Length];
            BuildObject();
        }

        void BuildObject()
        {
            if(EditorDisplayObject != null) EditorDisplayObject.Destroy();
            EditorDisplayObject = (GameObject)Constructor.Invoke(ConstructorParameters);
            AddChild(EditorDisplayObject);
        }
        public override void RenderDepthSorted(GLContext glContext, Vector3 slop)
        {
            if (this == ((Editor)game).selectedGameobject)
                _texture = Texture2D.GetInstance("editor/SelectedMarker.png");
            else _texture = Texture2D.GetInstance("editor/ProxyLogo.png");
            
            Vector3 baseScale = scaleXYZ;
            scaleXYZ = baseScale*(16.0f/game.width);
            base.RenderDepthSorted(glContext, slop);
            scaleXYZ = baseScale;
        }
        protected override Collider createCollider()
        {
            return new BoxCollider3D(this);
        }
        public override Vector3[] GetExtents()
        {
            Vector3[] res = new Vector3[8];
            for (int i = 0; i < 8; i++)
                res[i] = TransformPoint((i & 1) != 0 ? -radius : radius, (i & 2) != 0 ? -radius : radius, (i & 4) != 0 ? -radius : radius);
            return res;
        }
    }
}
