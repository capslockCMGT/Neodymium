using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GXPEngine;
using GXPEngine.Core;
using GXPEngine.UI;

namespace GXPEngine.Editor
{
    static class TypeHandler
    {
        public static Type[] GetInstantiableObjects()
        {
            var type = typeof(GameObject);
            var assembly = type.Assembly;
            //filtering out the ones i dont really like being able to add (game derivative classes, editor classes, ui classes etc)
            return assembly.GetTypes().Where(testc => 
            (testc.IsSubclassOf(type)) && 
            (!testc.IsSubclassOf(typeof(Game))) &&
            (testc.Namespace != typeof(Editor).Namespace) && 
            (testc.Namespace != typeof(Button).Namespace) && 
            (testc.Name != typeof(MyGame).Name)
            ).ToArray();
            //In the editor, thou shalt instantiate:
            //Gameobjects
            //Which are not UI objects
            //Or derivative of Game
        }
        public static bool IsValidConstructor(ConstructorInfo consInfo)
        {
            ParameterInfo[] parameters = consInfo.GetParameters();
            bool allowConstructor = true;
            foreach (ParameterInfo paramInfo in parameters)
            {
                //whitelisted types for the constructor (cannot input a straight bitmap in the editor after all! (sorry im not making ms paint for gxp))
                Type paramtype = paramInfo.ParameterType;
                allowConstructor &= 
                    paramInfo.HasDefaultValue ||
                    (paramtype == typeof(float)) ||
                    (paramtype == typeof(int)) ||
                    (paramtype == typeof(bool)) ||
                    (paramtype == typeof(string)) ||
                    (paramtype == typeof(uint)) ||
                    (paramtype == typeof(Texture2D))
                    ;
            }
            return allowConstructor;
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
    }
}
