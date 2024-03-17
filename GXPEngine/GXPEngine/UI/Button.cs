using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;

namespace GXPEngine.UI
{
    public class Button : Panel
    {
        public bool enabled = true;
        public delegate void NoArgs();

        public NoArgs OnClick = null;
        public NoArgs OnRelease = null;
        public NoArgs OnHover = null;
        public NoArgs OnUnhover = null;

        public enum Status { CLICKED, HOVER, REST }
        public Status prevStatus = Status.REST;
        public Status status = Status.REST;
        public Button(string path, float x = 0, float y = 0) : base(path, x, y)
        {
            this.x = x;
            this.y = y;
            z= 0;
        }
        public Button(int width, int height, float x = 0, float y = 0) : base(width, height, x, y)
        {
            this.x = x;
            this.y = y;
            Fill(127, 127);
            Stroke(255, 255);
            Rect(width / 2, height / 2, width - 1, height - 1);
        }

        void CheckStatus()
        {
            //Vector3 localM = TransformPoint(Input.mouseX, Input.mouseY, 0);
            Vector3 localM = InverseTransformPoint(Input.mouseX, Input.mouseY, 0);
            //Vector3 localM = new Vector3(Input.mouseX, Input.mouseY, 0);
            //Console.WriteLine(Input.mouseX + "\t" + Input.mouseY);
            prevStatus = status;
            if (0 < localM.x && localM.x < texture.width && 0 < localM.y && localM.y < texture.height)
            {
                if (!Input.GetMouseButton(0))
                    status = Status.HOVER;
                else if (prevStatus == Status.HOVER && Input.GetMouseButtonDown(0))
                    status = Status.CLICKED;
                else if (prevStatus == Status.CLICKED && Input.GetMouseButton(0))
                    status = Status.CLICKED;
            }
            else
            {
                if (prevStatus == Status.CLICKED && Input.GetMouseButton(0))
                    status = Status.CLICKED;
                else
                    status = Status.REST;
            }
        }
        void TriggerEvents()
        {
            switch (status)
            {
                case Status.REST:

                    if (prevStatus == Status.CLICKED)
                        TriggerRelease();
                    if (prevStatus == Status.HOVER)
                        TriggerUnhover();

                    Rest();

                    break;

                case Status.CLICKED:

                    if (prevStatus != Status.CLICKED)
                        TriggerClick();

                    Clicked();

                    break;

                case Status.HOVER:

                    if (prevStatus == Status.HOVER)
                        TriggerHover();

                    Hovered();

                    break;
            }
        }
        public void Update()
        {
            if (enabled)
                CheckStatus();

            TriggerEvents();
        }
        public virtual void TriggerClick()
        {
            color = 0xff00ff00;
            OnClick?.Invoke();
        }
        public virtual void TriggerHover()
        {
            color = 0xffff0000;
            OnHover?.Invoke();

        }
        public virtual void TriggerUnhover()
        {
            color = 0xffffffff;
            OnUnhover?.Invoke();

        }
        public virtual void TriggerRelease()
        {
            color = 0xffffffff;
            OnRelease?.Invoke();

        }


        public virtual void Clicked() { }
        public virtual void Hovered() { }
        public virtual void Rest() { }
    }
}
