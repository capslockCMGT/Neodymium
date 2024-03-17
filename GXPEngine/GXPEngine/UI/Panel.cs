using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.UI
{
    public class Panel : EasyDraw
    {
        public Panel(string path, float x = 0, float y = 0) : base(path)
        {
            this.x = x;
            this.y = y;
        }
        public Panel(int width, int height, float x = 0, float y = 0) : base(new Bitmap(width, height))
        {
            this.x = x;
            this.y = y;
            Fill(127, 127);
            Stroke(255, 255);
            Rect(width / 2, height / 2, width - 1, height - 1);
        }
    }
}
