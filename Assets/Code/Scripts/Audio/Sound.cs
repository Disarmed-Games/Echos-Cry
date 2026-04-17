using AudioSystem;
using UnityEngine;

namespace EchosCry
{
    public class Sound 
    {
        public static void Execute(soundEffect sfx, Transform origin, float time)
        {
            SoundEffectManager.Instance.Builder
            .SetSound(sfx)
            .SetSoundPosition(origin.position)
            .SetDelay(time)
            .ValidateAndPlaySound();
        }
    }
}
