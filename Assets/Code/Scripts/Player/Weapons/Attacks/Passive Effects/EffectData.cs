using AudioSystem;
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
    //public EchosCry.Effects EffectEnum = EchosCry.Effects.None;
    public EchosCry.EffectTier EffectTier = EchosCry.EffectTier.One;

    [SerializeReference] public List<Effect> Effects;
    private void OnEnable()
    {
        Effects ??= new();
    }
}

[Serializable]
public abstract class Effect
{
    public virtual void Use(Enemy enemy, EffectHandler handler, int stackCount) { }
    public virtual void Apply(Enemy enemy, EffectHandler handler) { }
    public virtual void Remove(Enemy enemy, EffectHandler handler) { }
}

//Does damage to target enemy on Use
[Serializable]
public class DamageEffect : Effect
{
    public float damage = 2f;
    public Color textColor = Color.white;
    public Color tintColor = Color.red;
    public soundEffect sound = null;

    public override void Use(Enemy enemy, EffectHandler handler, int stackCount)
    {
        float attackDamage = damage * stackCount;
        enemy.Health.Damage(attackDamage);

        if(sound != null) EchosCry.Sound.PlaySFX(sound, enemy.transform, 0);
        else EchosCry.Sound.PlaySFX(enemy.SoundConfig.HitSFX, enemy.transform, 0);

        enemy.Animator.TintFlash(tintColor, 0.2f);
        enemy.Animator.PlayBloodVisualEffect();

        if (DamageLabelManager.Instance != null && DamageLabelManager.Instance.isActiveAndEnabled)
            DamageLabelManager.Instance.SpawnPopup(attackDamage, enemy.transform.position, textColor);

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
    //public float duration = 2f;
    [Range(0f, 1f)] public float multiplierChance = 1f;

    public override void Apply(Enemy enemy, EffectHandler handler)
    {
        if (CheckHitChance())
        {
            enemy.Stats.DamageMultiplier *= damageMultiplier;
            //enemy.StartCoroutine(ResetDamageMultiplier(duration, enemy));
        }
    }
    public override void Remove(Enemy enemy, EffectHandler handler)
    {
        //enemy.Stats.DamageMultiplier /= damageMultiplier;
    }
    private bool CheckHitChance()
    {
        float randomVal = UnityEngine.Random.Range(0f, 1f);
        return (randomVal <= multiplierChance);
    }
    //private IEnumerator ResetDamageMultiplier(float time, Enemy enemy)
    //{
    //    yield return new WaitForSeconds(time);
    //    enemy.Stats.DamageMultiplier /= damageMultiplier;
    //}
}

[Serializable]
public class MovementAdjustEffect : Effect
{
    public float speedAdjustment = 1;
    public Color tintColor = Color.white;
    public override void Use(Enemy enemy, EffectHandler handler, int stackCount)
    {
        for (int i = 0; i < stackCount; i++)
        {
            enemy.Stats.MovementMultiplier *= speedAdjustment;
        }
    }
    public override void Apply(Enemy enemy, EffectHandler handler)
    {
        enemy.Animator.SetTint(tintColor);
    }
    public override void Remove(Enemy enemy, EffectHandler handler)
    {
        enemy.Animator.ResetTint();
        enemy.Stats.MovementMultiplier /= speedAdjustment;
        enemy.NavMeshAgent.speed = enemy.DefaultMovementSpeed;
    }
}

[Serializable]
public class KnockbackAdjustEffect : Effect
{
    public float knockbackAdjustment = 1.5f;
    public Color tintColor = Color.white;
    public override void Use(Enemy enemy, EffectHandler handler, int stackCount)
    {
        for (int i = 0; i < stackCount; i++)
        {
            enemy.Stats.KnockbackMultiplier *= knockbackAdjustment;
        }
    }
    public override void Remove(Enemy enemy, EffectHandler handler)
    {
        enemy.Stats.KnockbackMultiplier /= knockbackAdjustment;
    }
}
