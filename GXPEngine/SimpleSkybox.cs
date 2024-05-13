using System.Drawing;
using GXPEngine.Core;
using GXPEngine.OpenGL;

namespace GXPEngine.GXPEngine
{
    public class SimpleSkybox : Canvas
    {
        Pen single = new Pen(Color.White, 1);
        Camera cam;
        public Vector3 SkyColor = new Vector3(.25f,.5f,1);
        public bool renderTransparent;
        public bool ToneMap = true;
        public float GradientIntensity = 3;
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
            pixelated = false;
        }

        protected override void RenderSelf(GLContext glContext)
        {
            //base.RenderSelf(glContext);
        }

        void RenderSkybox(GLContext glContext)
        {
            graphics.Clear(Color.Transparent);
            glContext.PushMatrix(new float[]
            {
                1,0,0,0,
                0,1,0,0,
                0,0,1,0,
                -1,-1,.5f,1
            });
            float rangeX = game.heightRatio;
            float rangeY = 1;
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
            int range01toc(float c)
            {
                c *= 255;
                int res = (int)c;
                if (res > 255) return 255;
                if (res < 0) return 0;
                return res;
            }

            Vector3 ACESfilm(Vector3 x) //a good tonemap can change a man's life
            {
                //taken from https://knarkowicz.wordpress.com/2016/01/06/aces-filmic-tone-mapping-curve/
                const float a = 2.51f;
                const float b = .03f;
                const float c = 2.43f;
                const float d = .59f;
                const float e = .14f;
                float ace(float i)
                {
                    return ((i * (a * i + b)) / (i * (c * i + d) + e));
                }
                return new Vector3(ace(x.x), ace(x.y), ace(x.z));
            } 

            for (int x = 0; x < _texture.width; x++)
            {
                for (int y = 0; y < _texture.height; y++)
                {
                    float xx = Mathf.Lerp(x / (float)_texture.width, -rangeX, rangeX);
                    float yy = Mathf.Lerp(y / (float)_texture.height, -rangeY, rangeY);
                    Vector3 tot = fo - (xx * le) - (yy * up);

                    tot.Normalize();

                    Vector3 rgb = Vector3.zero;
                    float b = Mathf.Pow(GradientIntensity, tot.y);
                    rgb.x = b * SkyColor.x;
                    rgb.y = b * SkyColor.y;
                    rgb.z = b * SkyColor.z;

                    if(ToneMap) rgb = ACESfilm(rgb);

                    int col = range01toc(rgb.z) | (range01toc(rgb.y) << 8) | (range01toc(rgb.x) << 16) | ((renderTransparent?range01toc(b):0xFF) << 24);

                    single.Color = Color.FromArgb(col);

                    graphics.DrawLine(single, x, y, x + .01f, y);
                }
            }
            base.RenderSelf(glContext);
            glContext.PopMatrix();
            GL.Clear(GL.DEPTH_BUFFER_BIT);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            game.uiManager.mainWindow.onBeforeRenderAnything -= RenderSkybox;
        }
    }
}
