using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;

namespace GXPEngine
{
    public class ModelRenderer : GameObject
    {
        BufferRenderer _model;
        Texture2D _texture;

        public uint _color = 0xFFFFFF;
        public ModelRenderer(string modelFilename, string textureFilename)
        {
            if (Game.main == null)
            {
                throw new Exception("Models cannot be created before creating a Game instance.");
            }
            _model = BufferRenderer.GetModel(modelFilename);
            _texture = Texture2D.GetInstance(textureFilename);
        }

        override protected void RenderSelf(GLContext glContext)
        {
            if (game != null)
            {

                if (OnScreen())
                {
                    _model._texture = _texture;
                    glContext.SetColor((byte)((_color >> 16) & 0xFF),
                                       (byte)((_color >> 8) & 0xFF),
                                       (byte)(_color & 0xFF),
                                       (byte)(0xFF));
                    _model.DrawBuffers(glContext);
                    glContext.SetColor(255, 255, 255, 255);
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
