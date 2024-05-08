using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    public class WaterHitbox : Box
    {
        public WaterHitbox() : base("editor/whitePixel.png", true, true)
        {
            collider.isTrigger = true;
            color = 0x0044FF;
        }
    }
}
