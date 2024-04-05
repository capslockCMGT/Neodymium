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
        public static NoArgs AnyButtonOnClick = null;

        public enum Status { CLICKED, HOVER, REST }
        public Status prevStatus = Status.REST;
        public Status status = Status.REST;
        public Button(string path, float x = 0, float y = 0) : base(path, x, y)
        {
        }
        public Button(int width, int height, float x = 0, float y = 0, bool invisible = false) : base(width, height, x, y, invisible)
        {
        }

        void CheckStatus()
        {
            prevStatus = status;
            if (HitTest(Input.mouseX, Input.mouseY))
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
        protected bool HitTest(int x, int y)
        {
            Vector3 local = InverseTransformPoint(x, y, 0);
            if (local.x > _bounds.right) return false;
            if (local.x < _bounds.left) return false;
            if (local.y > _bounds.bottom) return false;
            if (local.y < _bounds.top) return false;
            return true;
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

                    if (prevStatus == Status.CLICKED)
                        TriggerRelease();

                    Hovered();

                    break;
            }
        }
        public override void Update()
        {
            base.Update();
            if (enabled)
                CheckStatus();

            TriggerEvents();
        }
        public virtual void TriggerClick()
        {
            //color = 0xff00ff00;
            OnClick?.Invoke();
            AnyButtonOnClick?.Invoke();
        }

        public virtual void TriggerRelease()
        {
            //color = 0xffffffff;
            OnRelease?.Invoke();

        }
        public virtual void TriggerHover()
        {
            //color = 0xffff0000;
            OnHover?.Invoke();

        }
        public virtual void TriggerUnhover()
        {
            //color = 0xffffffff;
            OnUnhover?.Invoke();

        }


        protected virtual void Clicked() { }
        protected virtual void Hovered() { }
        protected virtual void Rest() { }
    }
}
