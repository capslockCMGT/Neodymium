using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;
using GXPEngine.OpenGL;

namespace GXPEngine.GXPEngine
{
    public class SimpleSkybox : Canvas
    {
        Pen single = new Pen(Color.White,1);
        Camera cam;
        public SimpleSkybox(int width, int height) : base(width, height, false)
        {
            this.width = 2;
            this.height = 2;
            //SetOrigin(1, 1);
            //if (!(game.uiManager.mainWindow.camera is Camera))
            //{
            //    Destroy();
            //    return;
            //}
            cam = game.uiManager.mainWindow.camera as Camera;
            game.uiManager.mainWindow.onBeforeRenderAnything += RenderSkybox;
        }

        protected override void RenderSelf(GLContext glContext)
        {
            base.RenderSelf(glContext);
        }

        void RenderSkybox(GLContext glContext)
        {
            _graphics.Clear(Color.White);
            glContext.PushMatrix(new float[]
            {
                1,0,0,0,
                0,1,0,0,
                0,0,1,0,
                -1,-1,.5f,1
            });
            Vector3 topLeftCorner = cam.ScreenPointToLocal(0, 0);
            Vector3 bottomRightCorner = cam.ScreenPointToLocal(game.width, game.height);
            float rangeX = bottomRightCorner.x - topLeftCorner.x;
            float rangeY = bottomRightCorner.y - topLeftCorner.y;
            rangeX = 1;
            rangeY = 1;
            rangeX *= .5f;
            rangeY *= .5f;
            Quaternion q = cam.globalRotation;
            Vector3 fo = q.Forward;
            Vector3 up = q.Up;
            Vector3 le = q.Left;
            int range11toc(float c)
            {
                c += 1;
                c *= 127;
                int res = (int)c;
                res &= 0xFF;
                return res;
            }
            for (int x = 0; x<_texture.width;  x++)
            {
                for(int y = 0; y<_texture.height; y++)
                {
                    float xx = Mathf.Lerp(x / (float)_texture.width, -rangeX, rangeX);
                    float yy = Mathf.Lerp(y / (float)_texture.height, -rangeY, rangeY);
                    Vector3 tot = fo - xx*le - yy*up;
                    tot.Normalize();
                    int col = (range11toc(tot.y) << 8) | (range11toc(tot.x) << 16) | (0xFF << 24);
                    //float b = tot.normalized().z;
                    single.Color = Color.FromArgb(col);
                    //Console.WriteLine(single.Color);
                    graphics.DrawLine(single, x, y, x+1, y);
                }
            }
            Console.WriteLine(q);
            base.RenderSelf(glContext);
            glContext.PopMatrix();
            GL.Clear(0x100);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            game.uiManager.mainWindow.onBeforeRenderAnything -= RenderSkybox;
        }
    }
}
