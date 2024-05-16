using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;

namespace GXPEngine
{
    public class SpatialSound : GameObject
    {
        public static float globalVolume = 1;
        SoundChannel channel;
        float falloff;
        public float halfVolumeDistance
        {
            get { return Mathf.Sqrt(1 / falloff); }
            set { falloff = 1 / (value * value); }
        }
        public SpatialSound(Sound theSound, float halfVolumeDist = 10)
        {
            halfVolumeDistance = halfVolumeDist;
            if (!AudioListener.exists) return;
            channel = theSound.Play(volume:GetVolume(), pan:GetPan());
        }

        void Update()
        {
            if(channel == null || !channel.IsPlaying)
            {
                Destroy();
                return;
            }
            if (!AudioListener.exists) return;
            channel.Volume = GetVolume();
            channel.Pan = GetPan();
        }
        float listenerDistSq { get { return listenerRelpos.MagnitudeSquared(); } }
        Vector3 listenerRelpos { get { return globalPosition - AudioListener.pos; } }
        float GetVolume()
        {
            return globalVolume / (falloff * listenerDistSq + 1);
        }
        float GetPan()
        {
            return Mathf.Clamp(AudioListener.leftVector * listenerRelpos.normalized(),-1,1);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            channel?.Stop();
        }
    }
}
