using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    public class LavaHitbox : Box
    {
        public LavaHitbox() : base("editor/whitePixel.png",true,true)
        {
            collider.isTrigger = true;
            color = 0xFFAA00;
        }
    }
}
