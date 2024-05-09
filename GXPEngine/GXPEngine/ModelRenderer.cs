using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;
using GXPEngine.OpenGL;

namespace GXPEngine
{
    public class ModelRenderer : GameObject
    {
        BufferRenderer _model;
        Texture2D _texture;

        public uint color = 0xFFFFFF;
        public bool pixelated = Game.main.PixelArt;
        public ModelRenderer(string modelFilename, string textureFilename)
        {
            if (Game.main == null)
            {
                throw new Exception("Models cannot be created before creating a Game instance.");
            }
            _model = BufferRenderer.GetModel(modelFilename);
            _texture = Texture2D.GetInstance(textureFilename);
        }
        public ModelRenderer(BufferRenderer model, string textureFilename)
        {
            if (Game.main == null)
            {
                throw new Exception("Models cannot be created before creating a Game instance.");
            }
            _model = model;
            _texture = Texture2D.GetInstance(textureFilename);
        }

        override protected void RenderSelf(GLContext glContext)
        {
            if (game != null)
            {

                if (OnScreen())
                {
                    _model.texture = _texture;
                    _model.pixelated = pixelated;
                    glContext.SetColor((byte)((color >> 16) & 0xFF),
                                       (byte)((color >> 8) & 0xFF),
                                       (byte)(color & 0xFF),
                                       (byte)(0xFF));



                    //FINALLY LIGHTING
                    GL.Enable(0xb50);
                    GL.Enable(0x4000);
                    //Vector3 lightpos = InverseTransformPoint(-1,0,0);
                    Vector3 lightpos = new Vector3(0,1,0);
                    GL.LightModeli(0xb51, 1);
                    GL.LightModeli(0xb52, 0);
                    //GL.Normal3f(0, 1, 0);
                    GL.Lightfv(0x4000, 0x1200, new float[] { 1f, 1f, 1f, 1f });
                    GL.Lightfv(0x4000, 0x1203, new float[] { lightpos.x, lightpos.y, lightpos.z, 1f });

                    _model.DrawBuffers(glContext);
                    glContext.SetColor(255, 255, 255, 255);

                    GL.Disable(0xb50);
                    GL.Disable(0x4000);
                }
            }
        }

        // TODO: fix this.
        //------------------------------------------------------------------------------------------------------------------------
        //														OnScreen
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Again, does not work for now.
        /// </summary>
        protected bool OnScreen()
        {
            return true;
        }
    }
}
