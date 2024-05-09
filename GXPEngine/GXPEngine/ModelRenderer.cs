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
                    GL.Enable(GL.LIGHTING);
                    GL.Enable(GL.GL_LIGHT0);
                    //Vector3 lightpos = InverseTransformPoint(-1,0,0);
                    Vector3 lightpos = new Vector3(0,1,0);
                    GL.LightModeli(GL.LIGHT_MODEL_LOCAL_VIEWER, 1);
                    GL.LightModeli(GL.LIGHT_MODEL_TWO_SIDE, 0);
                    //GL.Normal3f(0, 1, 0);
                    GL.Lightfv(GL.GL_LIGHT0, GL.AMBIENT, new float[] { 1f, 1f, 1f, 1f });
                    GL.Lightfv(GL.GL_LIGHT0, GL.POSITION, new float[] { lightpos.x, lightpos.y, lightpos.z, 1f });

                    _model.DrawBuffers(glContext);
                    glContext.SetColor(255, 255, 255, 255);

                    GL.Disable(GL.LIGHTING);
                    GL.Disable(GL.GL_LIGHT0);
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
