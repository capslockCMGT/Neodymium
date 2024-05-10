using System;
using System.Linq;
using System.Reflection;
using GXPEngine.Core;
using GXPEngine.UI;

namespace GXPEngine.Editor
{
    static class TypeHandler
    {
        public static FieldInfo[] GetPublicVariables(Type obj)
        {
            return obj.GetFields().Where(testc => IsAllowedProperty(testc.FieldType) && testc.IsPublic).ToArray();
        }
        public static PropertyInfo[] GetPublicProperties(Type obj)
        {
            return obj.GetProperties().Where(testc => IsAllowedProperty(testc.PropertyType) && testc.CanWrite && testc.CanRead && testc.DeclaringType != typeof(Transformable)).ToArray();
        }
        public static Type GetTypeFromString(string Name)
        {
            return GetInstantiableObjects().FirstOrDefault(x => x.Name == Name);
        }
        public static GameObject BuildFromConstructorParams(object[] constructorParams, Type ObjectType)
        {
            ConstructorInfo info = GetConstructorFromParams(constructorParams, ObjectType);
            return BuildFromConstructor(constructorParams, info.GetParameters(), ObjectType);
        }
        public static ConstructorInfo GetConstructorFromParams(object[] constructorParams, Type ObjectType)
        {
            Type[] types = new Type[constructorParams.Length];
            for (int i = 0; i < constructorParams.Length; i++)
                types[i] = constructorParams[i].GetType();
            return ObjectType.GetConstructor(types);
        }
        public static GameObject BuildFromConstructor(object[] ConstructorParameters, ParameterInfo[] ConstructorParams, Type ObjectType)
        {
            GameObject ret = null;
            for (int i = 0; i < ConstructorParams.Length; i++)
            {
                if (ConstructorParameters[i] != null) continue;

                Type t = ConstructorParams[i].ParameterType;
                ConstructorParameters[i] = GetDefaultPropertyValue(t);

                if (ConstructorParams[i].HasDefaultValue) ConstructorParameters[i] = ConstructorParams[i].DefaultValue;
            }
            try
            {
                ret = (GameObject)Activator.CreateInstance(ObjectType, ConstructorParameters);
            }
            catch 
            {
                //i dont wanna hear it!
                //Console.WriteLine($"Could not display or create gameobject - invalid constructor: {e}");
            }
            return ret;
        }
        public static bool IsValidConstructor(ConstructorInfo consInfo)
        {
            ParameterInfo[] parameters = consInfo.GetParameters();
            bool allowConstructor = true;
            foreach (ParameterInfo paramInfo in parameters)
            {
                //whitelisted types for the constructor (cannot input a straight bitmap in the editor after all! (sorry im not making ms paint for gxp))
                Type paramtype = paramInfo.ParameterType;
                allowConstructor &= paramInfo.HasDefaultValue || IsAllowedProperty(paramtype);
            }
            return allowConstructor;
        }
        public static bool IsAllowedProperty(Type paramtype)
        {
            return  (paramtype == typeof(float)) ||
                    (paramtype == typeof(int)) ||
                    (paramtype == typeof(bool)) ||
                    (paramtype == typeof(string)) ||
                    (paramtype == typeof(uint)) ||
                    (paramtype == typeof(Quaternion)) ||
                    (paramtype == typeof(Vector3)) ||
                    (paramtype == typeof(Texture2D))
                    ;
        }
        public static object GetDefaultPropertyValue(Type property)
        {
            //accepted types:string, float, int, uint, bool, Texture2D, Vector3, Quaternion
            if (property == typeof(string))
                return "default text";
            if (property == typeof(float))
                return 0.0f;
            if (property == typeof(int))
               return 0;
            if (property == typeof(uint))
                return 0u;
            if (property == typeof(bool))
                return false;
            if (property == typeof(Texture2D))
                return Texture2D.GetInstance("editor/defaultCubeTex.png");
            if (property == typeof(Vector3))
                return Vector3.zero;
            if (property == typeof(Quaternion))
                return Quaternion.Identity;
            return null;
        }
        public static string GetConstructorAsText(ConstructorInfo consInfo)
        {
            string constructorText = "(";
            ParameterInfo[] parameters = consInfo.GetParameters();
            foreach (ParameterInfo paramInfo in parameters)
            {
                Type paramtype = paramInfo.ParameterType;
                constructorText += paramtype.Name + " ";
                constructorText += paramInfo.Name;
                if (paramInfo.HasDefaultValue)
                {
                    object value = paramInfo.DefaultValue;
                    if (value != null)
                        constructorText += " = " + value.ToString();
                    else constructorText += " = null";
                }
                constructorText += ", ";
            }
            if (constructorText.Length > 1) constructorText = constructorText.Substring(0, constructorText.Length - 2);
            return constructorText;
        }
        static Type[] instantiableObjects;
        public static Type[] GetInstantiableObjects()
        {
            if(instantiableObjects != null) return (Type[])instantiableObjects.Clone();
            var type = typeof(GameObject);
            var assembly = type.Assembly;
            //filtering out the ones i dont really like being able to add (game derivative classes, editor classes, ui classes etc)
            instantiableObjects = assembly.GetTypes().Where(testc => 
            (testc.IsSubclassOf(type)) && 
            (!testc.IsSubclassOf(typeof(Game))) &&
            (testc.Namespace != typeof(TypeHandler).Namespace) &&
            (testc.Namespace != typeof(Exclusives.SceneEditor).Namespace) &&
            (testc.Namespace != typeof(Button).Namespace) && 
            (testc != typeof(GameStarter)) &&
            testc != typeof(Game)
            ).ToArray();

            //In the editor, thou shalt instantiate:
            //Gameobjects
            //Which are not UI objects (sry)
            //Or games
            return (Type[])instantiableObjects.Clone();
        }
    }
}
