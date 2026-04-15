using AudioSystem;
using UnityEngine;

public class PlayerHitQuality : MonoBehaviour
{
    [SerializeField] soundEffect _excellentSFX;
    [SerializeField] soundEffect _goodSFX;
    [SerializeField] soundEffect _missSFX;
    [SerializeField] InputTranslator _translator;

    private void OnEnable()
    {
        if (_translator == null) return;
        _translator.OnPrimaryActionEvent += DetermineHitQualitySound;
        _translator.OnSecondaryActionEvent += DetermineHitQualitySound;
        _translator.OnSpecialAttackEvent += DetermineHitQualitySound;
        //_translator.OnDashEvent += DetermineHitQualitySound;
    }
    private void OnDisable()
    {
        if (_translator == null) return;
        _translator.OnPrimaryActionEvent -= DetermineHitQualitySound;
        _translator.OnSecondaryActionEvent -= DetermineHitQualitySound;
        _translator.OnSpecialAttackEvent -= DetermineHitQualitySound;
        //_translator.OnDashEvent -= DetermineHitQualitySound;
    }
    private void PlayHitQualitySound(soundEffect sfx)
    {
        if (sfx == null) return;
        SoundEffectManager.Instance.Builder
            .SetSound(sfx)
            .SetSoundPosition(transform.position)
            .ValidateAndPlaySound();
    }
    private void DetermineHitQualitySound(bool isPressed)
    {
        if (SpamPrevention.InputLocked) return;
        if (!isPressed) return;

        switch (TempoConductor.Instance.CurrentHitQuality)
        {
            case TempoConductor.HitQuality.Excellent:
                PlayHitQualitySound(_excellentSFX);
                break;
            case TempoConductor.HitQuality.Good:
                PlayHitQualitySound(_goodSFX);
                break;
            case TempoConductor.HitQuality.Miss:
                PlayHitQualitySound(_missSFX);
                break;
            default:
                break;
        }
    }
}
