using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    public class ControlsHint : DSCFSprite
    {
        int bind;
        bool shown;
        bool fading = false;
        DSCFSprite extra;
        public ControlsHint(string filename, int bind, bool shown = true, DSCFSprite extra = null) : base(filename)
        {
            this.shown = shown;
            this.bind = bind;
            this.extra = extra;
            SetOrigin(width/2, height/2);
        }

        public ControlsHint(string filename, int bind1, int bind2, bool shown = true) : base(filename)
        {
            this.shown = shown;
            this.bind = bind1 + (bind2 << 9);
        }
        public void Update()
        {
            if (check(bind))
                fading = true;
            if (fading)
                alpha -= Time.deltaTimeS * 2;
            if (alpha < 0f)
            {
                alpha = 0f;
                fading= false;
                shown= false;
            }

            bool check(int binds)
            {
                while (binds != 0)
                {
                    if (Input.GetKeyDown(binds & 0b111111111)) return true;
                    binds >>= 9;
                }
                return false;
            }

            foreach (GameObject go in GetChildren())
            {
                if (go is Sprite)
                    (go as Sprite).alpha = alpha;
            }
        }
        protected override void RenderSelf(GLContext glContext)
        {
            if (shown)
            {
                base.RenderSelf(glContext);

            }
        }
    }
}
