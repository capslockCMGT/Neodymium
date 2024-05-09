using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;
using GXPEngine.OpenGL;

namespace GXPEngine.AddOns
{
    public static class Lighting
    {
        static bool _enabled;
        public static bool enabled { get { return _enabled; } }
        public static void Enable() { GL.Enable(GL.LIGHTING); _enabled = true; }
        public static void Disable() { GL.Disable(GL.LIGHTING); _enabled = false; }
        public static void SetLight(int light, Vector3 lightPosition, Vector3 ambientLightColor, Vector3 lightColor)
        {
            bool alreadyEnabled = enabled;
            if(!enabled) Enable();
            GL.Enable(GL.LIGHT0 + light);
            GL.LightModeli(GL.LIGHT_MODEL_LOCAL_VIEWER, 1);
            GL.LightModeli(GL.LIGHT_MODEL_TWO_SIDE, 0);
            GL.Lightfv(GL.LIGHT0 + light, GL.AMBIENT, new float[] { ambientLightColor.x, ambientLightColor.y, ambientLightColor.z, 1f });
            GL.Lightfv(GL.LIGHT0 + light, GL.DIFFUSE, new float[] { lightColor.x, lightColor.y, lightColor.z, 1f});
            GL.Lightfv(GL.LIGHT0 + light, GL.POSITION, new float[] { lightPosition.x, lightPosition.y, lightPosition.z, 1f });
            if(!alreadyEnabled)Disable();
        }
        public static void DisableLight(int light)
        {
            bool alreadyEnabled = enabled;
            if (!enabled) Enable();
            GL.Disable(GL.LIGHT0+light);
            if (!alreadyEnabled) Disable();
        }
    }
}
