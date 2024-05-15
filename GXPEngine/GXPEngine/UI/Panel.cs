using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace GXPEngine.UI
{
    public class Panel : EasyDraw
    {
        protected float _contentHeight;
        public bool isMask;
        //float[] _mask;
        public float ContentHeight
        {
            get { return _contentHeight; }
        }
        float _contentWidth;
        public Panel(string path, float x = 0, float y = 0) : base(path)
        {
            this.x = x;
            this.y = y;
            z = 0;
        }
        /// <summary>
        /// Work in progress
        /// </summary>
        /// <returns></returns>
        public Vector2[] GetMask()
        {
            Vector2[] ret = GetExtents2D();
            if (parent == null)
            {
                alpha= 1.0f;
                return ret;
            }
                //return parent.GetMask(ret.x, ret.y, ret.z);
            return ret;
        }
        internal override float[] GetArea()
        {
            
            return new float[8] {
                _bounds.left, _bounds.top,
                _bounds.right, _bounds.top,
                _bounds.right, _bounds.bottom,
                _bounds.left, _bounds.bottom
            };
        }
        public Panel(int width, int height, float x = 0, float y = 0, bool invisible = false) : base(new Bitmap(width, height))
        {
            this.x = x;
            this.y = y;
            _contentWidth= width;
            _contentHeight= height;
            z = 0;
            if(invisible) ClearTransparent();
            else SetupTexture();
        }
        public virtual void Update()
        {
            foreach (GameObject child in GetChildren())
            {
                if(child is Panel)
                ((Panel)child).Update();
            }
        }
        public Panel ResizedToContent(int marginHorizontal = 5, int marginVertical = 5)
        {
            Panel sub = new Panel((int)_contentWidth + marginHorizontal, (int)_contentHeight + marginVertical);
            initializeFromTexture(sub.texture);
            return this;
        }
        public void ResizeToContent(int marginHorizontal = 5, int marginVertical = 5)
        {
            Resize((int)_contentWidth + marginHorizontal, (int)(_contentHeight) + marginVertical);
        }
        public void Resize(int width, int height, bool visible = true)
        {
            Panel sub = new Panel(width, height, invisible:!visible);
            initializeFromTexture(sub.texture.Clone());
            sub.LateDestroy();
        }
        protected virtual void SetupTexture()
        {
            ClearTransparent();
            Stroke(255, 255);
            Fill(64, 64, 64, 155);
            Rect(width / 2, height / 2, width - 1, height - 1);
        }

        public virtual void OrganiseChildrenVertical(float marginVertical = 5, float marginHorizontal = 5, CenterMode centerVertical = CenterMode.Min, CenterMode centerHorizontal = CenterMode.Min)
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
                        sprite.SetOrigin(-marginHorizontal, vertAlignment * sprite.height * sprite.scaleY);
                        break;
                    case CenterMode.Max:
                        sprite.SetOrigin(sprite.width + marginHorizontal - width, vertAlignment * sprite.scaleY);
                        break;
                    default:
                        sprite.SetOrigin(sprite.width * .5f, vertAlignment * sprite.height * sprite.scaleY);
                        sprite.x = width * .5f;
                        break;
                }
                sprite.y = currentY;
                _contentHeight += sprite.height * sprite.scaleY + marginVertical;
                _contentWidth = Mathf.Max(sprite.width * sprite.scaleX + marginHorizontal, _contentWidth);

                if(centerVertical == CenterMode.Max)
                    currentY -= marginVertical + sprite.height * sprite.scaleY;
                else currentY += marginVertical + sprite.height * sprite.scaleY; 
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
                _contentHeight = Mathf.Max(sprite.height+marginVertical, _contentHeight);
                _contentWidth += sprite.width + marginHorizontal;

                if(centerHorizontal == CenterMode.Max)
                    currentX -= marginHorizontal + sprite.width;
                else currentX += marginHorizontal + sprite.width;
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
