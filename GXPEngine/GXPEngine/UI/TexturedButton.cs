using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;
using GXPEngine.UI;

namespace GXPEngine.UI
{
    public class TexturedButton : Button
    {
        Texture2D Resting;
        Texture2D Hovering;
        Texture2D Pressed;
        public TexturedButton(string texture, string textureHover = null, string texturePressed = null, float x = 0, float y = 0) : base(texture, x, y)
        {
            Resting = Texture2D.GetInstance(texture);
            if(textureHover != null) Hovering = Texture2D.GetInstance(textureHover);
            if(texturePressed != null) Pressed = Texture2D.GetInstance(texturePressed);
        }
        public TexturedButton(Texture2D texture, Texture2D hovering, Texture2D pressed, float x = 0, float y = 0) : base(texture.width, texture.height, x, y)
        {
            Resting = texture;
            Hovering = hovering;
            Pressed = pressed;
        }

        protected override void Rest()
        {
            base.Rest();
            _texture = Resting;
        }
        protected override void Hovered()
        {
            base.Hovered();
            if (Hovering != null)
            _texture = Hovering;
        }
        protected override void Clicked()
        {
            base.Clicked();
            if (Pressed != null)
            _texture = Pressed;
        }
    }
}
