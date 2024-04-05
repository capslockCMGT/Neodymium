using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;

namespace GXPEngine.Editor
{
    public class EditorGameObject : DSCFSprite
    {
        ConstructorInfo Constructor;
        public Type ObjectType;
        ParameterInfo[] ConstructorParams;
        object[] ConstructorParameters;
        GameObject EditorDisplayObject;
        protected static Vector3[] _bounds;
        float radius = .25f;
        public EditorGameObject(Type objectType, ConstructorInfo constructor) : base("editor/ProxyLogo.png", addCollider:true)
        {
            SetOrigin(width * .5f,height * .5f);
            ObjectType = objectType;
            Constructor = constructor;
            ConstructorParams = Constructor.GetParameters();
            ConstructorParameters = new object[ConstructorParams.Length];
            BuildObject();
            if(_bounds == null) CreateBounds();
        }

        void BuildObject()
        {
            if(EditorDisplayObject != null) EditorDisplayObject.Destroy();
            EditorDisplayObject = TypeHandler.BuildFromConstructor(ConstructorParameters, ConstructorParams, ObjectType);
            if(EditorDisplayObject != null) AddChild(EditorDisplayObject);
        }
        public override void RenderDepthSorted(GLContext glContext, Vector3 slop)
        {
            if (this == ((Editor)game).selectedGameobject)
                _texture = Texture2D.GetInstance("editor/SelectedMarker.png");
            else _texture = Texture2D.GetInstance("editor/ProxyLogo.png");
            
            Vector3 baseScale = scaleXYZ;
            scaleXYZ = baseScale*(32.0f/game.width);
            scaleZ = baseScale.z;
            base.RenderDepthSorted(glContext, slop);
            scaleXYZ = baseScale;
        }
        protected override Collider createCollider()
        {
            return new BoxCollider(this);
        }
        public override Vector3[] GetExtents()
        {
            Vector3[] res = new Vector3[8];
            for (int i = 0; i < 8; i++)
                //dw abt it
                res[i] = TransformPoint(_bounds[i]*radius);
            return res;
        }
        void CreateBounds()
        {
            _bounds = new Vector3[8];
            for (int i = 0; i < 6; i++)
            {
                int sel = i % 3;
                int dir = i > 2 ? 1 : -1;
                Vector3 normal = new Vector3(sel == 1 ? dir : 0, sel == 0 ? dir : 0, sel == 2 ? dir : 0);
                Vector3 x = new Vector3(sel == 2 ? 1 : 0, 0, sel != 2 ? 1 : 0);
                Vector3 y = new Vector3(sel == 0 ? 1 : 0, sel != 0 ? 1 : 0, 0);
                Vector3 res;
                res = normal - x - y;
                if (sel == 1) _bounds[(dir == 1 ? 1 : 0) * 4 + 0] = res;
                res = normal + x - y;
                if (sel == 1) _bounds[(dir == 1 ? 1 : 0) * 4 + 1] = res;
                res = normal + x + y;
                if (sel == 1) _bounds[(dir == 1 ? 1 : 0) * 4 + 2] = res;
                res = normal - x + y;
                if (sel == 1) _bounds[(dir == 1 ? 1 : 0) * 4 + 3] = res;
            }
        }
    }
}
