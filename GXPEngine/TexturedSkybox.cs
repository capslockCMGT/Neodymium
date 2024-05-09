using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GXPEngine.Core;
using GXPEngine.OpenGL;

namespace GXPEngine.GXPEngine
{
    public class TexturedSkybox : GameObject
    {
        Pen single = new Pen(Color.White,1);
        Camera cam;
        Bitmap tex;
        EditableBufferRenderer renderer;
        Vector2 resolution;
        public TexturedSkybox(Bitmap tex, int resX, int resY) : base()
        {
            this.tex = tex;
            cam = game.uiManager.mainWindow.camera as Camera;
            game.uiManager.mainWindow.onBeforeRenderAnything += RenderSkybox;
            resolution = new Vector2(resX, resY);
            renderer = new EditableBufferRenderer(new Texture2D(tex));
            GenerateGrid();
        }

        protected override void RenderSelf(GLContext glContext)
        {
        }

        void RenderSkybox(GLContext glContext)
        {
            glContext.PushMatrix(new float[]
            {
                1,0,0,0,
                0,1,0,0,
                0,0,1,0,
                0,0,.5f,1
            });
            UpdateGrid();
            //GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_S, GL.GL_REPEAT);
            renderer.DrawBuffers(glContext);
            glContext.PopMatrix();
            GL.Clear(GL.DEPTH_BUFFER_BIT);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            game.uiManager.mainWindow.onBeforeRenderAnything -= RenderSkybox;
        }

        private void GenerateGrid()
        {
            float rangeX = game.heightRatio;
            float rangeY = 1;
            rangeX *= .5f;
            rangeY *= .5f;

            Quaternion q = cam.globalRotation;
            Vector3 fo = q.Forward;
            Vector3 up = q.Up;
            Vector3 le = q.Left;

            renderer.ClearBuffers();
            for (int x = 0; x < resolution.x; x++)
            {
                for (int y = 0; y < resolution.y; y++)
                {
                    renderer.AddVert((x) / resolution.x * 2 - 1, (y) / resolution.y * 2 - 1, 0f);
                    renderer.AddVert((x+1) / resolution.x * 2 - 1, (y) / resolution.y * 2 - 1, 0f);
                    renderer.AddVert((x+1) / resolution.x * 2 - 1, (y+1) / resolution.y * 2 - 1, 0);
                    renderer.AddVert((x) / resolution.x * 2 - 1, (y+1) / resolution.y * 2 - 1, 0);

                    Vector2 uv = GetUV(x, y);
                    renderer.AddUv(uv.x, uv.y);
                    uv = GetUV(x+1, y);
                    renderer.AddUv(uv.x, uv.y);
                    uv = GetUV(x+1, y+1);
                    renderer.AddUv(uv.x, uv.y);
                    uv = GetUV(x, y+1);
                    renderer.AddUv(uv.x, uv.y);
                }
            }
            renderer.CreateBuffers();
            Vector2 GetUV(int x, int y)
            {
                float xx = Mathf.Lerp(x / (float)tex.Width, -rangeX, rangeX);
                float yy = Mathf.Lerp(y / (float)tex.Height, -rangeY, rangeY);
                Vector3 tot = fo - xx * le - yy * up;
                tot.Normalize();
                return new Vector2(Mathf.Atan2(tot.z, tot.x) / Mathf.PI, tot.y);
            }
            renderer.texture.wrap = true;
        }
        private void UpdateGrid()
        {
            float rangeX = game.heightRatio;
            float rangeY = 1;
            rangeX *= .5f;
            rangeY *= .5f;

            Quaternion q = cam.globalRotation;
            Vector3 fo = q.Forward;
            Vector3 up = q.Up;
            Vector3 le = q.Left;

            for (int x = 0; x < resolution.x; x++)
            {
                for (int y = 0; y < resolution.y; y++)
                {
                    bool seamed = false;
                    Vector2 uvUL = GetUV(x, y);
                    Vector2 uvUR = GetUV(x + 1, y);
                    Vector2 uvDL = GetUV(x, y + 1);
                    Vector2 uvDR = GetUV(x + 1, y + 1);

                    if (uvUL.x > uvUR.x)
                    {
                        uvUR+= new Vector2(1, 0);
                        if (uvUL.x - uvDL.x > 0.5)
                            uvDL += new Vector2(1, 0);
                        if (uvUL.x - uvDR.x > 0.5)
                            uvDR += new Vector2(1, 0);
                    }

                    if (uvDL.x > uvDR.x)
                    {
                        uvDR += new Vector2(1, 0);
                        if (uvDL.x - uvUL.x > 0.5)
                            uvUL += new Vector2(1, 0);
                        if (uvDL.x - uvUR.x > 0.5)
                            uvUR += new Vector2(1, 0);
                    }

                    renderer.SetUV((int)((x * resolution.y + y) * 4), uvUL);
                    renderer.SetUV((int)((x * resolution.y + y) * 4 + 1), uvUR);
                    renderer.SetUV((int)((x * resolution.y + y) * 4 + 2), uvDR);
                    renderer.SetUV((int)((x * resolution.y + y) * 4 + 3), uvDL);
                }
            }
            Vector2 GetUV(int x, int y)
            {
                float xx = Mathf.Lerp((x) / resolution.x, -rangeX, rangeX);
                float yy = Mathf.Lerp((y) / resolution.y, -rangeY, rangeY);
                Vector3 tot = fo - xx * le - yy * up;
                tot.Normalize();
                return new Vector2(Mathf.Atan2(tot.z, tot.x) / Mathf.PI / 2 + 0.5f, tot.y / 2 + 0.5f);
            }
        }
    }
}
