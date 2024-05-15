using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;

namespace GXPEngine
{
    public class Waterfall : Sprite
    {
        public int tiles = 1;
        public float period = 1;
        float[] uvs;
        public Waterfall() : base("neodymium/Liquids/Water.png")
        {
            Editor.Exclusives.SceneEditor.AddToUpdate(this);
            _texture.wrap = true;
            uvs = GetUVs();
        }
        public Waterfall(bool lava = false) : base(lava? "neodymium/Liquids/Lava.png" : "neodymium/Liquids/Water.png")
        {
            Editor.Exclusives.SceneEditor.AddToUpdate(this);
            _texture.wrap = true;
            uvs = GetUVs();
        }

        void Update()
        {
            for(int i = 1; i < _uvs.Length; i+=2) 
            {
                _uvs[i] = uvs[i]*tiles+Time.timeS*period;
            }
        }

        //tell me you have too much cpu without telling me you have too much cpu
        protected override void RenderSelf(GLContext glContext)
        {
            Window.ActiveWindow.onRenderTransparent += RenderTransparent;
        }

        void RenderTransparent(GLContext glContext)
        {
            GameObject current = this;
            List<GameObject> parentstack = new List<GameObject>();
            while (true)
            {
                parentstack.Add(current);
                if (current.parent == null)
                    break;
                current = current.parent;
            }
            for(int i = parentstack.Count; i-- > 0;) 
                glContext.PushMatrix(parentstack[i].matrix);
            base.RenderSelf(glContext);
            for (int i = parentstack.Count; i-- > 0;)
                glContext.PopMatrix();
        }
    }
}
