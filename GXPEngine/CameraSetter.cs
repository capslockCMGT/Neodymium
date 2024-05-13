using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;

namespace GXPEngine
{
    public class CameraSetter : GameObject
    {
        float dist, heigh, low, high;
        public CameraSetter(float distance = 16, float height = 3, float lowAngle = -.1f, float highAngle = .3f) 
        {
            dist = distance;
            heigh = height;
            low = lowAngle;
            high = highAngle;

            if (!(game is Editor.Exclusives.SceneEditor))
                LateDestroy();
            if(game is Neodymium)
            {
                (game as Neodymium).Camera.distance = distance;
                (game as Neodymium).Camera.height = height;
                (game as Neodymium).Camera.maxAngleLow = lowAngle;
                (game as Neodymium).Camera.maxAngleHigh = highAngle;
            }
        }
        void Update()
        {
            Remove();
        }
        protected override void RenderSelf(GLContext glContext)
        {
            if (!(game is Editor.Exclusives.SceneEditor))
                return;
            float xp = dist;
            float zp = 0;
            for(float i = 0; i < 2*Mathf.PI; i+=.03f)
            {
                float px = Mathf.Cos(i)*dist;
                float pz = Mathf.Sin(i)*dist;
                Gizmos.DrawLine(pz, heigh, px, zp, heigh, xp, null, 0xFF005500, 2);
                xp = px;
                zp = pz;
            }
            xp = Mathf.Cos(low) * dist;
            zp = Mathf.Sin(low) * dist;
            for (float j = Mathf.PI*low+.03f; j < Mathf.PI*high; j += .03f)
            {
                float px = Mathf.Cos(j) * dist;
                float pz = Mathf.Sin(j) * dist;
                Gizmos.DrawLine(px, heigh + pz, 0, xp, heigh + zp, 0, null, 0xFF005500, 2);
                xp = px;
                zp = pz;
            }
        }
    }
}
