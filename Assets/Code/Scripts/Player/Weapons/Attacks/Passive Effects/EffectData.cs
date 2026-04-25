using Ink.Parsed;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect Data", menuName = "Echo's Cry/Effects/Effect Data")]
public class EffectData : ScriptableObject
{
    public string EffectName;
    public string EffectDescription;
    public Sprite EffectIcon;
    public bool IsEffectUseOneTime = false;
    public float EffectUseInterval;
    public float EffectDuration;
    [Min(1)] public int MaxStacks = 1;
    public EchosCry.Effects EffectEnum;

    [SerializeReference] public List<Effect> Effects;

    private void OnEnable()
    {
        Effects ??= new();
    }
}

[Serializable]
public abstract class Effect
{
    public abstract void Use(Enemy enemy, EffectHandler handler);  
}

//Does damage to target enemy on Use
[Serializable]
public class DamageEffect : Effect
{
    public float damage = 2f;
    public override void Use(Enemy enemy, EffectHandler handler)
    {
        enemy.Health.Damage(damage);

        EchosCry.Sound.PlaySFX(enemy.SoundConfig.HitSFX, enemy.transform, 0);
        enemy.EnemyAnimator.TintFlash(Color.red, 0.2f);
        enemy.EnemyAnimator.PlayBloodVisualEffect();

        if (DamageLabelManager.Instance != null && DamageLabelManager.Instance.isActiveAndEnabled)
            DamageLabelManager.Instance.SpawnPopup(damage, enemy.transform.position, Color.purple);

        if (enemy.EnemyHealthUI != null) enemy.EnemyHealthUI.UpdateUI(enemy.Health.CurrentHealth,
        enemy.Health.MaxHealth,
        enemy.Health.CurrentArmor,
        enemy.Health.MaxArmor);
    }
}

//Updates the damage multiplier of target enemy, unless it is chance-based, which then means it'll check based on the multiplier chance passed to it
[Serializable]
public class DamageMultiplierEffect : Effect
{
    public float damageMultiplier = 1.2f;
    public float duration = 2f;
    [Range(0f, 1f)] public float multiplierChance = 1f;

    public override void Use(Enemy enemy, EffectHandler handler)
    {
        if (CheckHitChance())
        {
            enemy.Stats.DamageMultiplier *= damageMultiplier;
            enemy.StartCoroutine(ResetDamageMultiplier(duration, enemy));
        }
    }
    private bool CheckHitChance()
    {
        float randomVal = UnityEngine.Random.Range(0f, 1f);
        return (randomVal <= multiplierChance);
    }
    private IEnumerator ResetDamageMultiplier(float time, Enemy enemy)
    {
        yield return new WaitForSeconds(time);
        enemy.Stats.DamageMultiplier /= damageMultiplier;
    }
}
