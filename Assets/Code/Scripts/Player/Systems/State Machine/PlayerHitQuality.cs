using EchosCry;
using UnityEngine;

public class PlayerHitQuality : MonoBehaviour
{
    [SerializeField] Player player;

    private void OnEnable()
    {
        if (player.InputTranslator == null) return;
        player.InputTranslator.OnPrimaryActionEvent += HandlePrimary;
        player.InputTranslator.OnSecondaryActionEvent += HandleSecondary;
        player.InputTranslator.OnSpecialAttackEvent += HandleSpecial;
    }
    private void OnDisable()
    {
        if (player.InputTranslator == null) return;
        player.InputTranslator.OnPrimaryActionEvent -= HandlePrimary;
        player.InputTranslator.OnSecondaryActionEvent -= HandleSecondary;
        player.InputTranslator.OnSpecialAttackEvent -= HandleSpecial;
    }

    public void HandlePrimary(bool buttonPressed)
    {
        if (buttonPressed && !SpamPrevention.InputLocked)
        {
            Sound.PlayHitSound(player.SFXConfig, TempoConductor.Instance.CurrentHitQuality, transform);
            player.UI.HitQualityText.UpdateText();
        }
    }
    public void HandleSecondary(bool buttonPressed)
    {
        if(player.HeatGauge.CurrentCharge >= 2 && buttonPressed && !SpamPrevention.InputLocked)
        {
            Sound.PlayHitSound(player.SFXConfig, TempoConductor.Instance.CurrentHitQuality, transform);
            player.UI.HitQualityText.UpdateText();
        }
    }

    public void HandleSpecial(bool buttonPressed)
    {
        if (player.HeatGauge.CurrentCharge >= 6 && buttonPressed && !SpamPrevention.InputLocked)
        {
            Sound.PlayHitSound(player.SFXConfig, TempoConductor.Instance.CurrentHitQuality, transform);
            player.UI.HitQualityText.UpdateText();
        }
    }
}
