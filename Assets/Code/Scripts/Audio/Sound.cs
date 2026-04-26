using AudioSystem;
using UnityEngine;

namespace EchosCry
{
    public class Sound 
    {
        public static void PlaySFX(soundEffect sfx, Transform origin, float delay)
        {
            SoundEffectManager.Instance.Builder
            .SetSound(sfx)
            .SetSoundPosition(origin.position)
            .SetDelay(delay)
            .ValidateAndPlaySound();
        }
        public static void PlayHitSound(SFXConfig sounds, TempoConductor.HitQuality currentHit, Transform origin)
        {
            switch (currentHit)
            {
                case TempoConductor.HitQuality.Excellent:
                    PlaySFX(sounds.ExcellentSFX, origin, 0);
                    break;
                case TempoConductor.HitQuality.Good:
                    PlaySFX(sounds.GoodSFX, origin, 0);
                    break;
                case TempoConductor.HitQuality.Miss:
                default:
                    PlaySFX(sounds.MissSFX, origin, 0);
                    break;
            }
        }
    }
}
