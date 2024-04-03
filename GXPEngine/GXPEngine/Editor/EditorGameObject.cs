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
        float radius = .25f;
        public EditorGameObject(Type objectType, ConstructorInfo constructor) : base("editor/ProxyLogo.png")
        {
            SetOrigin(width * .5f,height * .5f);
            ObjectType = objectType;
            Constructor = constructor;
            ConstructorParams = Constructor.GetParameters();
            ConstructorParameters = new object[ConstructorParams.Length];
            BuildObject();
        }

        void BuildObject()
        {
            if(EditorDisplayObject != null) EditorDisplayObject.Destroy();
            for(int i = 0; i< ConstructorParams.Length; i++)
            {
                if (ConstructorParameters[i] != null) continue;

                Type t = ConstructorParams[i].ParameterType;
                //accepted types:string, float, int, uint, bool, Texture2D
                //if (t == typeof(float))
                //    ConstructorParameters[i] = 0.0f;
                if (t == typeof(string))
                    ConstructorParameters[i] = "default text";
                //if (t == typeof(int))
                //    ConstructorParameters[i] = 0;
                //if (t == typeof(uint))
                //    ConstructorParameters[i] = 0;
                //if (t == typeof(bool))
                //    ConstructorParameters[i] = false;
                //commented values are handled by ConstructorInfo already
                if (t == typeof(Texture2D))
                    ConstructorParameters[i] = Texture2D.GetInstance("editor/defaultCubeTex.png");

                if (ConstructorParams[i].HasDefaultValue) ConstructorParameters[i] = ConstructorParams[i].DefaultValue;
            }
            try
            {
                EditorDisplayObject = (GameObject)Constructor.Invoke(ConstructorParameters);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not display or create gameobject - invalid constructor: {e}");
            }
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
                //dw abt it
                res[i] = TransformPoint((i & 1) != 0 ? -radius : radius, (i & 2) != 0 ? -radius : radius, (i & 4) != 0 ? -radius : radius);
            return res;
        }
    }
}
