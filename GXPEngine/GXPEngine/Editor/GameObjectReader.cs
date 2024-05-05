using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;
using System.Reflection;

namespace GXPEngine.Editor
{
    public static class GameObjectReader
    {
        public static GameObject ReadGameObjectTree(string path)
        {
            using (var stream = File.Open(path, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    reader.ReadChar();
                    return ReadGameObject(reader);
                }
            }
        }
        public static Exclusives.EditorGameObject ReadEditorGameObjectTree(string path)
        {
            using (var stream = File.Open(path, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    reader.ReadChar();
                    return (Exclusives.EditorGameObject)ReadGameObject(reader, true);
                }
            }
        }

        static GameObject ReadGameObject(BinaryReader reader, bool asEditorGO = false)
        {
            string typeName = reader.ReadString();
            Type goType = TypeHandler.GetTypeFromString(typeName);
            if (goType == null) return null;

            reader.ReadChar(); // 't' - transform
            Vector3 pos = ReadVector3(reader);
            Quaternion rot = ReadQuaternion(reader);
            Vector3 scale = ReadVector3(reader);

            reader.ReadChar(); // 'c' - constructor info
            char nextRead = reader.ReadChar();
            List<object> constructorParams = new List<object>();
            while(nextRead == 'p')
            {
                reader.ReadString(); //ditch the names of constructor values - we dont really care (why are we saving these anyway? (oh right code reuse))
                constructorParams.Add(ReadParameter(reader));
                nextRead = reader.ReadChar();
            }

            object[] consParams = constructorParams.ToArray();
            ConstructorInfo consinfo = TypeHandler.GetConstructorFromParams(consParams, goType);
            GameObject result = TypeHandler.BuildFromConstructor(consParams, consinfo.GetParameters(), goType);
            if(result == null) return null;

            result.position = pos;
            result.rotation = rot;
            result.scaleXYZ = scale;

            reader.ReadChar(); // 'f' - fields
            nextRead = reader.ReadChar();
            while (nextRead == 'p')
            {
                FieldInfo f = goType.GetField(reader.ReadString());
                f.SetValue(result, ReadParameter(reader));
                nextRead = reader.ReadChar();
            }

            reader.ReadChar(); // 'p' - properties
            nextRead = reader.ReadChar();
            while (nextRead == 'p')
            {
                PropertyInfo f = goType.GetProperty(reader.ReadString());
                f.SetValue(result, ReadParameter(reader));
                nextRead = reader.ReadChar();
            }

            Exclusives.EditorGameObject edit = null;
            if (asEditorGO)
            {
                edit = new Exclusives.EditorGameObject(result, consinfo, consParams);
                edit.ConstructorParameters = consParams;
                edit.position = pos;
                result.position = Vector3.zero;
                edit.rotation = rot;
                result.rotation = Quaternion.Identity;
                result.scaleXYZ = edit.scaleXYZ;
                edit.scaleXYZ = scale;
            }

            reader.ReadChar(); // '{' - start of kids
            nextRead = reader.ReadChar();
            while(nextRead == 'g')
            {
                GameObject toAdd = ReadGameObject(reader, asEditorGO);
                if(asEditorGO) edit.AddChild(toAdd);
                else result.AddChild(toAdd);
                nextRead = reader.ReadChar();
            }
            reader.ReadChar(); // '}' - end of kids

            if (asEditorGO) return edit;
            else return result;
        }

        static object ReadParameter(BinaryReader reader)
        {
            switch(reader.ReadChar())
            {
                default: //catches ' ' in particular
                    return null;
                case 's':
                    return reader.ReadString();
                case 'b':
                    return reader.ReadBoolean();
                case 'f':
                    return reader.ReadSingle();
                case 'i':
                    return reader.ReadInt32();
                case 'u':
                    return reader.ReadUInt32();
                case 't':
                    return Texture2D.GetInstance(reader.ReadString());
                case 'v':
                    return ReadVector3(reader);
                case 'q': 
                    return ReadQuaternion(reader);
            }
        }
        static Quaternion ReadQuaternion(BinaryReader reader)
        {
            float r = reader.ReadSingle();
            float i = reader.ReadSingle();
            float j = reader.ReadSingle();
            float k = reader.ReadSingle();
            return new Quaternion(r, i, j, k);
        }

        static Vector3 ReadVector3(BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            return new Vector3(x, y, z);
        }
    }
}
