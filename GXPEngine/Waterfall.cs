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
        public Waterfall() : base("waterfall.png")
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
    }
}
