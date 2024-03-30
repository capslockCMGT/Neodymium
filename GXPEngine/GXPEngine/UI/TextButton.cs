using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.UI
{
    public class TextButton : Button
    {
        string txt;
        public TextButton(int width, int height, string text, int fontSize = 15) : base(width, height)
        {
            txt = text;
            TextAlign(CenterMode.Min, CenterMode.Center);
            Fill(255);
            TextFont("Noto Sans", fontSize);
            Text(txt, 3, height*.5f);
        }
        public override void TriggerHover()
        {
            base.TriggerHover();
            Clear(200);
            Fill(55);
            Text(txt, 3, height * .5f);
        }
        public override void TriggerUnhover()
        {
            base.TriggerUnhover();
            SetupTexture();
            Fill(255);
            Text(txt, 3, height * .5f);
        }
        public override void TriggerRelease()
        {
            base.TriggerRelease();
            SetupTexture();
            Fill(255);
            Text(txt, 3, height * .5f);
        }
        public override void TriggerClick()
        {
            base.TriggerClick();
            Clear(255);
            Fill(0);
            Text(txt, 3, height * .5f);
        }
    }
}
