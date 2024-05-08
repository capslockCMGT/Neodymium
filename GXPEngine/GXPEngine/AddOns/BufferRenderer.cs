using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GXPEngine.Core;
using GXPEngine.OpenGL;

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
        protected float[] normals;
        protected int numberOfVertices; // The number of rendered quads is numberOfVertices/6

        public Texture2D texture;
        public bool pixelated = Game.main.PixelArt;

        List<float> vertList = new List<float>();
        List<float> uvList = new List<float>();
        List<float> normalList = new List<float>();

        public BufferRenderer(Texture2D texture)
        {
            this.texture = texture;
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
            if (model == null)
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
        public void AddNormal(float x, float y, float z)
        {
            normalList.Add(x);
            normalList.Add(y);
            normalList.Add(z);
        }

        public void CreateBuffers()
        {
            verts = vertList.ToArray();
            uvs = uvList.ToArray();
            normals = normalList.ToArray();
            numberOfVertices = verts.Length / 3;
        }

        public void DrawBuffers(GLContext glContext)
        {

            texture.Bind();

            GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, pixelated ? GL.NEAREST : GL.LINEAR);
            GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MAG_FILTER, pixelated ? GL.NEAREST : GL.LINEAR);
            //glContext.SetColor(0xff, 0xff, 0xff, 0x99);
            GL.EnableClientState(GL.TEXTURE_COORD_ARRAY);
            GL.EnableClientState(GL.VERTEX_ARRAY);
            GL.EnableClientState(GL.NORMAL_ARRAY);
            GL.TexCoordPointer(2, GL.FLOAT, 0, uvs);
            GL.VertexPointer(3, GL.FLOAT, 0, verts);
            GL.NormalPointer(GL.FLOAT, 0, normals);
            GL.DrawArrays(GL.QUADS, 0, numberOfVertices);
            GL.DisableClientState(GL.VERTEX_ARRAY);
            GL.DisableClientState(GL.NORMAL_ARRAY);
            GL.DisableClientState(GL.TEXTURE_COORD_ARRAY);

            texture.Unbind();
        }

        public void WriteVertsToConsole()
        {
            for (int i = 0; i < verts.Length; i += 3)
            {
                Console.WriteLine(verts[i] + ", " + verts[i + 1] + ", " + verts[i + 2]);
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
            List<Vector3> normals = new List<Vector3>();
            List<int> vertIndeces = new List<int>();
            List<int> uvIndeces = new List<int>();
            List<int> normalIndeces = new List<int>();

            if (!File.Exists(model))
                throw new Exception("Model could not be found at filepath " + model);
            if (model.Substring(model.Length - 4) != ".obj")
                throw new Exception("Specified model is not in Wavefront (.obj) format. Model in question: " + model);

            StreamReader reader = new StreamReader(model);
            string line = reader.ReadLine();
            while (line != null)
            {
                string[] words;
                if (line.Length < 2)
                {
                    line = reader.ReadLine();
                    continue;
                }
                switch (line.Substring(0, 2))
                {
                    default:
                        break;
                    case "v ":
                        //data is a vertex, assuming 3 floats
                        words = line.Split(' ');
                        if (words.Length != 4) break;

                        float x, y, z;
                        float.TryParse(words[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out x);
                        float.TryParse(words[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out y);
                        float.TryParse(words[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out z);
                        verts.Add(new Vector3(x, y, z));

                        break;

                    case "vn":
                        //data is a normal - is ignored (given this class doesnt have normals for now)
                        words = line.Split(' ');
                        if (words.Length != 4) break;

                        float nx, ny, nz;
                        float.TryParse(words[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out nx);
                        float.TryParse(words[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out ny);
                        float.TryParse(words[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out nz);
                        normals.Add(new Vector3(nx, ny, nz));

                        break;
                    case "vt":
                        //data is a vertex, assuming 3 floats
                        words = line.Split(' ');
                        if (words.Length != 3) break;

                        float u, v;
                        float.TryParse(words[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out u);
                        float.TryParse(words[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out v);
                        //invert v cause it is upside down idk
                        uvs.Add(new Vector2(u, 1-v));

                        break;
                    case "f ":

                        //data is vert and UV indeces, assuming 3 or 4 sets of 3 ints
                        //the third int is the normal index, but we dont care about it and will be dropping it
                        words = line.Split(' ');
                        if (words.Length != 5 && words.Length != 4)
                            break;
                        int fallBackVert=1, fallBackUv=1, fallBackNormal = 1;
                        for (int i = 1; i < 5; i++)
                        {
                            int vertIndex, uvIndex, normalIndex;
                            if (i < words.Length)
                            {
                                string[] ints = words[i].Split('/');
                                int.TryParse(ints[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out vertIndex);
                                int.TryParse(ints[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out uvIndex);
                                int.TryParse(ints[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out normalIndex);
                                fallBackVert = vertIndex;
                                fallBackUv = uvIndex;
                                fallBackNormal = normalIndex;
                            }
                            else
                            {
                                vertIndex = fallBackVert;
                                uvIndex = fallBackUv;
                                normalIndex = fallBackNormal;
                            }
                            //in .obj, indeces start at 1. because of course they do.
                            vertIndeces.Add(vertIndex - 1);
                            uvIndeces.Add(uvIndex - 1);
                            normalIndeces.Add(normalIndex - 1);
                        }

                        break;
                }
                line = reader.ReadLine();
            }
            reader.Close();
            for (int i = 0; i < vertIndeces.Count; i++)
            {
                Vector3 vertex = verts[vertIndeces[i]];
                Vector2 uv = uvs[uvIndeces[i]];
                Vector3 normal = normals[normalIndeces[i]];

                AddVert(vertex.x, vertex.y, vertex.z);
                AddUv(uv.x, uv.y);
                AddNormal(normal.x, normal.y, normal.z);
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
