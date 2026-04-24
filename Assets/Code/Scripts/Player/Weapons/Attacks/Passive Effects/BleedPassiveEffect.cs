using UnityEngine;

[CreateAssetMenu(fileName = "Bleed Effect", menuName = "Echo's Cry/Passive Effects/Bleed")]
public class BleedPassiveEffect : PassiveEffect
{
    public float bleedDamage = 2f;

    public override void Use(Enemy enemy)
    {
        enemy.Health.Damage(bleedDamage);

        EchosCry.Sound.PlaySFX(enemy.SoundConfig.HitSFX, enemy.transform, 0);
        enemy.EnemyAnimator.TintFlash(Color.red, 0.2f);
        enemy.EnemyAnimator.PlayBloodVisualEffect();

        if (DamageLabelManager.Instance != null && DamageLabelManager.Instance.isActiveAndEnabled)
            DamageLabelManager.Instance.SpawnPopup(bleedDamage, enemy.transform.position, Color.purple);
    }
}