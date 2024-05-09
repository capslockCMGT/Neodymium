using GXPEngine.Core;
using GXPEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.UI
{
    public class SliderPanel : Button
    {
        Vector2 mouseClickPos = new Vector2(0,0);
        Vector2 initialButtonPos = new Vector2(0,0);
        Panel bar;
        Panel area;
        Panel content;
        Button barButton;
        public SliderPanel(int width, int height, float x = 0, float y = 0, bool invisible = false) : base(width, height, x, y, true)
        {
            area = new Panel(width, height,0,0,false);
            area.parent = this;
            content = new Panel(width, height+100, 0, 0, false);
            area.AddChild(content);

            area.OrganiseChildrenVertical(0, 0);
        }
        /// <summary>
        /// x = 0 and y = 0 would place the slider at the right top angle of the panel
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetSliderBar(int width, int height, float x = 0, float y = 0)
        {
            bar = new Panel(width, height, this.width-width + x, y);
            bar.parent = this;
            barButton = new Button(width, (int) ((content.height > area.height) ? height * area.height / content.height : height) , 0, 0);
            bar.AddChild(barButton);
            barButton.OnClick += SaveMousePosition;
        }
        public override void OrganiseChildrenVertical(float marginVertical = 5, float marginHorizontal = 5, CenterMode centerVertical = CenterMode.Min, CenterMode centerHorizontal = CenterMode.Min)
        {
            content.OrganiseChildrenVertical(marginVertical, marginHorizontal, centerVertical, centerHorizontal);
            content.ResizeToContent();
        }

        public override void Render(GLContext glContext)
        {
            var oldRange = game.RenderRange;
            var c = GetArea();
            Vector3 truepos = TransformPoint(0, 0, 0);
            game.SetViewport((int)(truepos.x-c[0]), (int)(truepos.y-c[3]), (int)width, (int)height, false);
            base.Render(glContext);
            game.SetViewport((int)oldRange.left, (int)oldRange.top, (int)oldRange.width, (int)oldRange.height);
        }

        /// <summary>
        /// Adds child to content instead of this in order to not interrupt the structure
        /// Please dont use AddChild
        /// </summary>
        /// <param name="child"></param>
        public override void AddChild(GameObject child)
        {
            content.AddChild(child);
        }
        public override void Update()
        {
            base.Update();
            if (barButton != null && bar != null)
            {
                barButton.height = (int)((content.height > area.height) ? bar.height * area.height / content.height : bar.height);
                if (barButton.status == Button.Status.CLICKED)
                {
                    barButton.color = 0xff00aaaa;

                    float deltaY = -barButton.y;
                    barButton.y = initialButtonPos.y + Input.mouseY - mouseClickPos.y;
                    if (barButton.y < 0) barButton.y = 0;
                    if (barButton.y > bar.height - barButton.height) barButton.y = bar.height - barButton.height;
                    deltaY += barButton.y;

                    foreach (GameObject child in area.GetChildren())
                    {
                        child.y -= deltaY / area.height * area.ContentHeight * 2;
                    }
                }
                if (barButton.status == Button.Status.HOVER) barButton.color = 0xff00ffff;
                if (barButton.status == Button.Status.REST) barButton.color = 0xffffffff;

            }
        }
        public void SaveMousePosition()
        {
            mouseClickPos = new Vector2(Input.mouseX, Input.mouseY);
            initialButtonPos = new Vector2(barButton.x, barButton.y);
        }
        public void ResizeContent(int width, int height)
        {
            content.Resize(width, height);
        }

        /// <summary>
        /// Sets position of the slider. 0 for the uppest, 1 for the lowest positions
        /// </summary>
        /// <param name="fac"></param>
        public void SetSliderPosition(float fac = 0)
        {
            if (barButton != null && bar != null)
            {
                float deltaY = -barButton.y;
                barButton.y = (bar.height - barButton.height) * fac;
                deltaY += barButton.y;

                foreach (GameObject child in area.GetChildren())
                {
                    child.y -= deltaY / area.height * area.ContentHeight;
                }
            }
        }
    }
}
