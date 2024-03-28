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
        float _contentHeight;
        float _contentWidth;
        public Panel(string path, float x = 0, float y = 0) : base(path)
        {
            this.x = x;
            this.y = y;
            z = 0;
        }
        public Panel(int width, int height, float x = 0, float y = 0, bool invisible = false) : base(new Bitmap(width, height))
        {
            this.x = x;
            this.y = y;
            z = 0;
            if(invisible) ClearTransparent();
            else SetupTexture();
        }
        public virtual void Update()
        {
            foreach (Panel child in GetChildren())
            {
                child.Update();
            }
        }
        protected virtual void SetupTexture()
        {
            ClearTransparent();
            Stroke(255, 255);
            Fill(127, 127, 127, 127);
            Rect(width / 2, height / 2, width - 1, height - 1);
        }

        public void OrganiseChildrenVertical(float marginVertical = 5, float marginHorizontal = 5, CenterMode centerVertical = CenterMode.Min, CenterMode centerHorizontal = CenterMode.Min)
        {
            float currentY = marginVertical;
            float vertAlignment = 0;
            _contentHeight = 0;
            _contentWidth = 0;

            switch (centerVertical)
            {
                case CenterMode.Min:
                    vertAlignment = 0;
                    break;
                case CenterMode.Max:
                    vertAlignment = 1;
                    currentY = height-marginVertical;
                    break;
                case CenterMode.Center:
                    vertAlignment = 0f;
                    currentY = 0;
                    break;
            }

            for(int i = 0; i<GetChildCount(); i++)
            {
                GameObject obj = GetChildren(false)[(centerVertical == CenterMode.Max ? GetChildCount() - i -1 : i)];
                if (!(obj is Sprite)) continue;
                Sprite sprite = (Sprite)obj;
                switch (centerHorizontal)
                {
                    case CenterMode.Min:
                        sprite.SetOrigin(-marginHorizontal, vertAlignment * sprite.height);
                        break;
                    case CenterMode.Max:
                        sprite.SetOrigin(sprite.width + marginHorizontal - width, vertAlignment * sprite.height);
                        break;
                    default:
                        sprite.SetOrigin(sprite.width * .5f, vertAlignment * sprite.height);
                        sprite.x = width * .5f;
                        break;
                }
                sprite.y = currentY;
                _contentHeight += sprite.height+marginVertical;
                _contentWidth = Mathf.Max(sprite.width, _contentWidth);

                switch (centerVertical)
                {
                    case CenterMode.Min:
                        currentY += marginVertical + sprite.height;
                        break;
                    case CenterMode.Max:
                        currentY -= marginVertical + sprite.height;
                        break;
                    default:
                        currentY += marginVertical + sprite.height;
                        break;
                }
            }

            if (centerVertical == CenterMode.Center)
            {
                float offset = (height - _contentHeight)*.5f;
                foreach (GameObject obj in GetChildren(false))
                {
                    obj.y += offset;
                }
            }
        }

        public void OrganiseChildrenHorizontal(float marginHorizontal = 5, float marginVertical = 5, CenterMode centerHorizontal = CenterMode.Min, CenterMode centerVertical = CenterMode.Min)
        {
            float currentX = marginHorizontal;
            float horAlignment = 0;
            _contentHeight = 0;
            _contentWidth = 0;

            switch (centerHorizontal)
            {
                case CenterMode.Min:
                    horAlignment = 0;
                    break;
                case CenterMode.Max:
                    horAlignment = 1;
                    currentX = width - marginHorizontal;
                    break;
                case CenterMode.Center:
                    horAlignment = 0f;
                    currentX = 0;
                    break;
            }

            for (int i = 0; i < GetChildCount(); i++)
            {
                GameObject obj = GetChildren(false)[(centerVertical == CenterMode.Max ? GetChildCount() - i - 1 : i)];
                if (!(obj is Sprite)) continue;
                Sprite sprite = (Sprite)obj;
                switch (centerVertical)
                {
                    case CenterMode.Min:
                        sprite.SetOrigin(horAlignment * sprite.width, - marginVertical);
                        break;
                    case CenterMode.Max:
                        sprite.SetOrigin(horAlignment * sprite.width, sprite.height + marginVertical - height);
                        break;
                    default:
                        sprite.SetOrigin(horAlignment * sprite.width, sprite.height * .5f);
                        sprite.y = height * .5f;
                        break;
                }
                sprite.x = currentX;
                _contentHeight = Mathf.Max(sprite.height, _contentHeight);
                _contentWidth += sprite.width + marginHorizontal;

                switch (centerHorizontal)
                {
                    case CenterMode.Min:
                        currentX += marginHorizontal + sprite.width;
                        break;
                    case CenterMode.Max:
                        currentX -= marginHorizontal + sprite.width;
                        break;
                    default:
                        currentX += marginHorizontal + sprite.width;
                        break;
                }
            }

            if (centerHorizontal == CenterMode.Center)
            {
                float offset = (width - _contentWidth) * .5f;
                foreach (GameObject obj in GetChildren(false))
                {
                    obj.x += offset;
                }
            }
        }
    }
}
