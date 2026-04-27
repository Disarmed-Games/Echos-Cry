using System.Collections;
using UnityEngine;

public class EnemyDamageable : MonoBehaviour, IDamageable
{
    [SerializeField] private Enemy _enemy;
    private bool _armorBreak = false;

    private void OnEnable()
    {
        _armorBreak = false;
    }

    public void Execute(AttackInfo attackData)
    {
        //Logic
        if (_enemy.Health.IsInvincible) return;

        _enemy.Collider.enabled = false;
        
        _enemy.DeathInfo = attackData;

        float damage = attackData.Damage * _enemy.Stats.DamageMultiplier;

        _enemy.Health.Damage(damage);

        HandleEffects(attackData.Effects);

        if (_enemy.Health.CurrentArmor <= 0)
        {
            _enemy.StateData.IsStaggered = true;
            StartCoroutine(KnockBackDuration(attackData, 0.2f));
        }

        //Visuals
        if (_enemy.Health.CurrentArmor > 0)
        {
            EchosCry.Sound.PlaySFX(_enemy.SoundConfig.ArmorHitSFX, _enemy.transform, 0);
            _enemy.Animator.TintFlash(_enemy.Data.TintShieldFlash, _enemy.Data.TintFlashDuration);
            _enemy.Animator.PlayArmorVisualEffect();
        }
        else
        {
            if (!_armorBreak)
            {
                _armorBreak = true;
                EchosCry.Sound.PlaySFX(_enemy.SoundConfig.ArmorBreakSFX, _enemy.transform, 0);
            }
            DecalManager.Instance.GetBloodDecal().transform.position = _enemy.transform.position;
            EchosCry.Sound.PlaySFX(_enemy.SoundConfig.HitSFX, _enemy.transform, 0);
            _enemy.Animator.TintFlash(_enemy.Data.TintHealthFlash, _enemy.Data.TintFlashDuration);
            _enemy.Animator.PlayBloodVisualEffect();
        }
            
        if(DamageLabelManager.Instance != null && DamageLabelManager.Instance.isActiveAndEnabled)
            DamageLabelManager.Instance.SpawnPopup(damage, _enemy.transform.position, Color.white);
        
        if(_enemy.EnemyHealthUI != null) _enemy.EnemyHealthUI.UpdateUI(_enemy.Health.CurrentHealth, 
            _enemy.Health.MaxHealth, 
            _enemy.Health.CurrentArmor, 
            _enemy.Health.MaxArmor);
    }

    private IEnumerator KnockBackDuration(AttackInfo attackData, float duration)
    {
        _enemy.Rigidbody.isKinematic = false;
        Vector3 direction = (_enemy.transform.position - attackData.Origin.position).normalized;
        _enemy.Rigidbody.AddForce(attackData.Force * direction, attackData.ForceMode);
        yield return new WaitForSeconds(duration);
        _enemy.Rigidbody.isKinematic = true;
    }

    private void HandleEffects(EffectData[] effects)
    {
        if (effects == null || effects.Length == 0) return;
        foreach (EffectData effect in effects)
        {
            switch (effect.EffectTier)
            {
                case EchosCry.EffectTier.One:
                    if (PlayerComboMeter.CurrentMeterState != PlayerComboMeter.MeterState.Starting)
                        _enemy.PassiveEffectHandler.ApplyEffect(effect, _enemy);
                    break;
                case EchosCry.EffectTier.Two:
                    if (PlayerComboMeter.CurrentMeterState >= PlayerComboMeter.MeterState.TwoThirds)
                        _enemy.PassiveEffectHandler.ApplyEffect(effect, _enemy);
                    break;
                case EchosCry.EffectTier.Three:
                    if (PlayerComboMeter.CurrentMeterState == PlayerComboMeter.MeterState.Full)
                        _enemy.PassiveEffectHandler.ApplyEffect(effect, _enemy);
                    break;
            }
        }
    }
}
