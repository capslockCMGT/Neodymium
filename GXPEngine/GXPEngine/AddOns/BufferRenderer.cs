using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;
using System.IO;
using GXPEngine.OpenGL;
using System.Collections;

namespace GXPEngine
{

    /// <summary>
    /// A helper class for SpriteBatches, and possibly other complex objects or collections with larger vertex and uv lists.
    /// </summary>
    public class BufferRenderer
    {
        private static Hashtable LoadCache = new Hashtable();

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

        public BufferRenderer(string model)
        {
            BuildFromObj(model);
        }

        public static BufferRenderer GetModel(string filename)
        {
            BufferRenderer model = LoadCache[filename] as BufferRenderer;
            if(model == null)
            {
                model = new BufferRenderer(filename);
                LoadCache[filename] = model;
            }
            return model;
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

            //glContext.SetColor(0xff, 0xff, 0xff, 0x99);
            GL.EnableClientState(GL.TEXTURE_COORD_ARRAY);
            GL.EnableClientState(GL.VERTEX_ARRAY);
            GL.TexCoordPointer(2, GL.FLOAT, 0, uvs);
            GL.VertexPointer(3, GL.FLOAT, 0, verts);
            GL.DrawArrays(GL.QUADS, 0, numberOfVertices);
            GL.DisableClientState(GL.VERTEX_ARRAY);
            GL.DisableClientState(GL.TEXTURE_COORD_ARRAY);

            _texture.Unbind();
        }

        public void WriteVertsToConsole()
        {
            for (int i = 0; i < verts.Length; i+=3)
            {
                Console.WriteLine(verts[i] + ", " + verts[i+1] + ", " + verts[i+2]);
            }
            Console.WriteLine("_____________________________");
        }
        public void Dispose()
        {
            // For this backend: nothing needed
        }

        private void BuildFromObj(string model)
        {
            List<Vector3> verts = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> vertIndeces = new List<int>();
            List<int> uvIndeces = new List<int>();

            if (!File.Exists(model))
                throw new Exception("Model could not be found at filepath " + model);
            if (model.Substring(model.Length-4) != ".obj")
                throw new Exception("Specified model is not in Wavefront (.obj) format. Model in question: "+ model);

            StreamReader reader = new StreamReader(model);
            string line = reader.ReadLine();
            while (line != null)
            {
                string[] words;
                if(line.Length>1)
                switch (line.Substring(0,2))
                {
                    default:
                        break;
                    case "v ":
                        //data is a vertex, assuming 3 floats
                        words = line.Split(' ');
                        if (words.Length != 4) break;

                        float x,y,z;
                        float.TryParse(words[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out x);
                        float.TryParse(words[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out y);
                        float.TryParse(words[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out z);
                        verts.Add(new Vector3(x, y, z));

                        break;
                    case "vn":
                        //data is a normal - is ignored (given this class doesnt have normals for now)
                        break;
                    case "vt":
                        //data is a vertex, assuming 3 floats
                        words = line.Split(' ');
                        if (words.Length != 3) break;

                        float u, v;
                        float.TryParse(words[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out u);
                        float.TryParse(words[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out v);
                        uvs.Add(new Vector2(u, v));

                        break;
                    case "f ":
                        //data is vert and UV indeces, assuming 4 sets of 3 ints
                        //the third int is the normal index, but we dont care about it and will be dropping it
                        words = line.Split(' ');
                        if (words.Length != 5) break;
                        for(int i = 1; i<5; i++)
                        {
                            string[] ints = words[i].Split('/');
                            int vertIndex, uvIndex;
                            int.TryParse(ints[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out vertIndex);
                            int.TryParse(ints[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out uvIndex);
                            //in .obj, indeces start at 1. because of course they do.
                            vertIndeces.Add(vertIndex-1);
                            uvIndeces.Add(uvIndex-1);
                        }

                        break;
                }
                line = reader.ReadLine();
            }
            reader.Close();
            for(int i = 0; i<vertIndeces.Count; i++)
            {
                Vector3 vertex = verts[vertIndeces[i]];
                Vector2 uv = uvs[uvIndeces[i]];
                AddVert(vertex.x, vertex.y, vertex.z);
                AddUv(uv.x, uv.y);
            }
            CreateBuffers();
            /*for(int i = 0; i < verts.Count; i++)
            {
                Console.WriteLine(verts[i]);
            }
            for(int i = 0; i < uvs.Count; i++)
            {
                Console.WriteLine(uvs[i]);
            }*/
        }
    }
}
