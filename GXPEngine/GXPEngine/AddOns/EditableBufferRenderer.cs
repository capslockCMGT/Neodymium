using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using GXPEngine.Core;
using GXPEngine.OpenGL;

namespace GXPEngine
{

    /// <summary>
    /// A helper class for SpriteBatches, and possibly other complex objects or collections with larger vertex and uv lists.
    /// </summary>
    public class EditableBufferRenderer : BufferRenderer
    {
        private static Hashtable LoadCache = new Hashtable();

        List<float> vertList = new List<float>();
        List<float> uvList = new List<float>();

        public EditableBufferRenderer(Texture2D texture)
        {
            this.texture = texture;
        }

        public EditableBufferRenderer()
        {

        }

        public EditableBufferRenderer(string model) : base(model)
        {

        }
        public void ClearBuffers()
        {
            vertList.Clear();
            uvList.Clear();
            CreateBuffers();
        }

        public int GetVertCount()
        {
            return verts.Length/3;
        }

        public Vector3 GetVertex(int index)
        {
            return new Vector3(verts[3 * index], verts[3 * index + 1], verts[3 * index + 2]);
        }
        public void SetVertex(int index, Vector3 value)
        {
            verts[3 * index] = value.x;
            verts[3 * index + 1] = value.y;
            verts[3 * index + 2] = value.z;
        }

        public Vector2 GetUV(int index)
        {
            return new Vector2(uvs[2 * index], uvs[2 * index + 1]);
        }
        public void SetUV(int index, Vector2 value)
        {
            uvs[2 * index] = value.x;
            uvs[2 * index + 1] = value.y;
        }
    }
}
