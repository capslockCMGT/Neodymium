using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;
using GXPEngine.OpenGL;

namespace GXPEngine
{

    /// <summary>
    /// A helper class for SpriteBatches, and possibly other complex objects or collections with larger vertex and uv lists.
    /// </summary>
    public class BufferRenderer
    {
        protected float[] verts;
        protected float[] uvs;
        protected int numberOfVertices; // The number of rendered quads is numberOfVertices/6

        public Texture2D _texture;

        List<float> vertList = new List<float>();
        List<float> uvList = new List<float>();

        public BufferRenderer(Texture2D texture)
        {
            _texture = texture;
        }

        public BufferRenderer()
        {
        }

        public void AddVert(float x, float y, float z)
        {
            vertList.Add(x);
            vertList.Add(y);
            vertList.Add(z);
        }
        public void AddUv(float u, float v)
        {
            uvList.Add(u);
            uvList.Add(v);
        }

        public void CreateBuffers()
        {
            verts = vertList.ToArray();
            uvs = uvList.ToArray();
            numberOfVertices = verts.Length / 3;
        }

        public void DrawBuffers(GLContext glContext)
        {
            _texture.Bind();

            GL.EnableClientState(GL.TEXTURE_COORD_ARRAY);
            GL.EnableClientState(GL.VERTEX_ARRAY);
            GL.TexCoordPointer(2, GL.FLOAT, 0, uvs);
            GL.VertexPointer(3, GL.FLOAT, 0, verts);
            GL.DrawArrays(GL.QUADS, 0, numberOfVertices);
            GL.DisableClientState(GL.VERTEX_ARRAY);
            GL.DisableClientState(GL.TEXTURE_COORD_ARRAY);

            _texture.Unbind();
        }

        public void Dispose()
        {
            // For this backend: nothing needed
        }
    }
}
