using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace GXPEngine.UI
{
    public class TextPanel : Panel
    {
        string txt;
        public TextPanel(int width, int height, string text, int fontSize = 15, bool addBackground = true) : base(width, height, invisible:!addBackground)
        {
            txt = text;
            TextAlign(CenterMode.Min, CenterMode.Center);
            Fill(255);
            TextFont("Noto Sans", fontSize);
            Text(txt, 3, height * .5f);
        }
    }
}
