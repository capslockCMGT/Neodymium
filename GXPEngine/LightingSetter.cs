using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.AddOns;
using GXPEngine.Core;

namespace GXPEngine
{
    public class LightingSetter : GameObject
    {
        //only 3 lights to make it not as monstrous
        public LightingSetter(
            Vector3 position1, Vector3 ambientColor1, Vector3 color1, 
            Vector3 position2 = default, Vector3 ambientColor2 = default, Vector3 color2 = default, 
            Vector3 position3 = default, Vector3 ambientColor3 = default, Vector3 color3 = default) {
            Lighting.SetLight(0, position1, ambientColor1, color1);
            Lighting.SetLight(1, position2, ambientColor2, color2);
            Lighting.SetLight(2, position3, ambientColor3, color3);
        }
        void Update()
        {
            Destroy();
        }
    }
}
