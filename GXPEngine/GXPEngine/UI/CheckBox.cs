using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.UI
{
    public class CheckBox : Button
    {
        public delegate void boolChanged(bool changed);
        public boolChanged OnSwitch = null;
        bool _state;
        public bool state
        {
            get { return _state; }
            set { 
                _state = value; 
                SetupTexture();
            }
        }
        public CheckBox(int width, int height, float x = 0, float y = 0) : base(width, height, x, y, false)
        {

        }
        public override void TriggerRelease()
        {
            state = !state;
            OnSwitch?.Invoke(state);
            base.TriggerRelease();
        }
        protected override void SetupTexture()
        {
            base.SetupTexture();
            if(_state)
            {
                graphics.DrawLine(pen, width * .2f, height * .5f, width * .5f, height * .8f);
                graphics.DrawLine(pen, width * .5f, height * .8f, width * .8f, height * .2f);
                color = 0xAAFFAA;
            }
            else
            {
                graphics.DrawLine(pen, width * .2f, height * .2f, width * .8f, height * .8f);
                graphics.DrawLine(pen, width * .2f, height * .8f, width * .8f, height * .2f);
                color = 0xFFAAAA;
            }
        }
    }
}
