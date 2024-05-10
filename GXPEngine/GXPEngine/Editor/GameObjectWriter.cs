using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;

namespace GXPEngine.Editor
{
    public static class GameObjectWriter
    {
        public static void WriteEditorGameObjectTree(Exclusives.EditorGameObject tree, string fileLocation)
        {
            using(var stream = File.Open(fileLocation, FileMode.Create))
            {
                using(BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    WriteEditorGameObject(tree, writer);
                }
            }
        }
        static void WriteEditorGameObject(Exclusives.EditorGameObject obj, BinaryWriter writer) 
        {
            writer.Write('g'); //the gameobject
            writer.Write(obj.ObjectType.Name); //the type
            WriteTransform(obj, writer);
            writer.Write('c'); //constructor - variable length, so finish with '\n'
            for(int i = 0; i<obj.ConstructorParams.Length; i++)
                WriteParameter(obj.ConstructorParams[i].Name, obj.ConstructorParameters[i], obj.ConstructorParams[i].ParameterType, writer);
            writer.Write('\n');
            writer.Write('f'); //fields - variable length, so finish with '\n'
            foreach (FieldInfo field in obj.fields)
                WriteParameter(field.Name, field.GetValue(obj.EditorDisplayObject), field.FieldType,writer);
            writer.Write('\n');
            writer.Write('p'); //properties - variable length, so finish with '\n'
            foreach (PropertyInfo property in obj.properties)
                WriteParameter(property.Name, property.GetValue(obj.EditorDisplayObject), property.PropertyType, writer);
            writer.Write('\n');
            writer.Write('{');//the kids go here
            foreach(GameObject kid in obj.GetChildren())
                if (kid is Exclusives.EditorGameObject) WriteEditorGameObject((Exclusives.EditorGameObject)kid, writer);
            writer.Write('}');//end of kids
            writer.Write('\n');//end of this gameobject
        }
        static void WriteTransform(Transformable transform, BinaryWriter writer)
        {
            writer.Write('t');
            WriteVector3(transform.position, writer);
            WriteQuaternion(transform.rotation, writer);
            WriteVector3(transform.scaleXYZ, writer);
        }
        static void WriteParameter(string name, object parameter, Type objectType, BinaryWriter writer)
        {
            writer.Write('p');
            writer.Write(name);
            if(!TypeHandler.IsAllowedProperty(objectType))
            {
                //this should never ever happen and if it does someones in trouble (me)
                writer.Write(' ');
                return;
            }
            if(parameter == null)
                parameter = TypeHandler.GetDefaultPropertyValue(objectType);
            if(objectType == typeof(string))
            {
                writer.Write('s');
                writer.Write((string)parameter);
            }
            if(objectType == typeof(bool))
            {
                writer.Write('b');
                writer.Write((bool)parameter);
            }
            if(objectType == typeof(float))
            {
                writer.Write('f');
                writer.Write((float)parameter);
            }
            if(objectType == typeof(int))
            {
                writer.Write('i');
                writer.Write((int)parameter);
            }
            if(objectType == typeof(uint))
            {
                writer.Write('u');
                writer.Write((uint)parameter);
            }
            if(objectType == typeof(Texture2D))
            {
                writer.Write('t');
                writer.Write(((Texture2D)parameter).filename);
            }
            if(objectType == typeof(Vector3))
            {
                writer.Write('v');
                WriteVector3((Vector3)parameter, writer);
            }
            if (objectType == typeof(Quaternion))
            {
                writer.Write('q');
                WriteQuaternion((Quaternion)parameter, writer);
            }
        }
        static void WriteVector3(Vector3 vec, BinaryWriter writer)
        {
            writer.Write(vec.x);
            writer.Write(vec.y);
            writer.Write(vec.z);
        }
        static void WriteQuaternion(Quaternion quat, BinaryWriter writer)
        {
            writer.Write(quat.r);
            writer.Write(quat.i);
            writer.Write(quat.j);
            writer.Write(quat.k);
        }
    }
}
