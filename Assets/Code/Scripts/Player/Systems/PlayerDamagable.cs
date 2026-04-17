using UnityEngine;

public class PlayerDamagable : MonoBehaviour, IDamageable
{
    [SerializeField] Player player;
    private bool _armorBreak = false;

    public void Execute(AttackInfo attackData)
    {
        player.Health.Damage(attackData.Damage);
        if (player.Health.HasArmor)
        {
            player.Animator.TintFlash(Color.blue);
            if (GlobalSFXManager.Instance != null && GlobalSFXManager.Instance.ArmorHitSFX != null)
                EchosCry.Sound.PlaySFX(GlobalSFXManager.Instance.ArmorHitSFX, player.transform, 0);
            

        }
        else
        {
            if (!_armorBreak)
            {
                _armorBreak = true;
                if (GlobalSFXManager.Instance != null && GlobalSFXManager.Instance.ArmorBreakSFX != null)
                    EchosCry.Sound.PlaySFX(GlobalSFXManager.Instance.ArmorBreakSFX, player.transform, 0);
            }
            player.Animator.TintFlash(Color.red);
        }
        EchosCry.Sound.PlaySFX(player.SFXConfig.HurtEffect, player.transform, 0);
    }
}
