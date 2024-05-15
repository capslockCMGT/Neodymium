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
        public BlendMode blendMode;
        public bool transparent;

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

        protected void RenderModel(GLContext glContext)
        {
            if (game != null)
            {

                if (OnScreen())
                {
                    if (blendMode != null) blendMode.enable();
                    _model.texture = _texture;
                    _model.pixelated = pixelated;
                    glContext.SetColor((byte)((color >> 16) & 0xFF),
                                       (byte)((color >> 8) & 0xFF),
                                       (byte)(color & 0xFF),
                                       (byte)(0xFF));
                    _model.DrawBuffers(glContext);
                    if (blendMode != null) BlendMode.NORMAL.enable();
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
        protected override void RenderSelf(GLContext glContext)
        {
            if (transparent)
            {

                GL.Disable(GL.DEPTH_TEST);
                RenderModel(glContext);
                GL.Enable(GL.DEPTH_TEST);

            }
                //Window.ActiveWindow.onRenderTransparent += RenderTransparent;
            else
                RenderModel(glContext);
        }

        void RenderTransparent(GLContext glContext)
        {
            GL.Disable(GL.DEPTH_TEST);
            GameObject current = this;
            List<GameObject> parentstack = new List<GameObject>();
            while (true)
            {
                parentstack.Add(current);
                if (current.parent == null)
                    break;
                current = current.parent;
            }
            for (int i = parentstack.Count; i-- > 0;)
                glContext.PushMatrix(parentstack[i].matrix);
            RenderModel(glContext);
            for (int i = parentstack.Count; i-- > 0;)
                glContext.PopMatrix();
            GL.Enable(GL.DEPTH_TEST);
        }
    }
}
