using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;

namespace GXPEngine
{
    public class AudioListener : GameObject
    {
        static AudioListener instance;
        public static Vector3 pos { get { return instance.globalPosition; } }
        public static bool exists { get { return instance != null; } }
        public static Vector3 leftVector { get { return instance.globalRotation.Left; } }
        public AudioListener() 
        { 
            instance?.Destroy();
            instance = this;
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            instance = instance == this ? null : instance;
        }
    }
}
