using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.UI
{
    public class TextObjectButton : TextButton
    {
        object _object;
        public delegate void ObjArg(object obj);
        public ObjArg ObjOnClick = null;
        public TextObjectButton(int width, int height, string text, object Object, int fontsize = 15) : base(width, height, text, fontsize)
        {
            _object = Object;
        }

        protected override void Clicked()
        {
            base.Clicked();
            ObjOnClick(_object);
        }
    }
}
