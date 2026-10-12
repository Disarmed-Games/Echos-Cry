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
            DisplayQuality(TempoConductor.Instance.CurrentHitQuality);
        }
    }
    public void HandleSecondary(bool buttonPressed)
    {
        if(player.HeatGauge.CurrentCharge >= 2 && buttonPressed && !SpamPrevention.InputLocked)
        {
            DisplayQuality(TempoConductor.Instance.CurrentHitQuality);
        }
    }

    public void HandleSpecial(bool buttonPressed)
    {
        if (player.HeatGauge.CurrentCharge >= HeatGauge.SpecialAttackCost && buttonPressed && !SpamPrevention.InputLocked)
        {
            var tempo = TempoConductor.Instance;
            var quality = tempo.GetSpecialAttackIndex() != 0
                ? tempo.CurrentHitQuality : TempoConductor.HitQuality.Miss;
            DisplayQuality(quality);
        }
    }

    private void DisplayQuality(TempoConductor.HitQuality quality)
    {
        // Text, tint, and sound all use one result captured for this press.
        Sound.PlayHitSound(player.SFXConfig, quality, transform);
        player.UI.HitQualityText.UpdateText(quality);
    }
}
