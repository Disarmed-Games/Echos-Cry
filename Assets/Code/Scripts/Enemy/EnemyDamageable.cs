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

    public virtual void Execute(AttackInfo attackData)
    {
        if (_enemy.Invulnerable) return;

        _enemy.Collider.enabled = false;

        float damage = attackData.Damage * _enemy.Health.DamageMultiplier;

        _enemy.Health.Damage(damage);

        if(_enemy.Health.CurrentArmor > 0)
        {
            EchosCry.Sound.PlaySFX(_enemy.SoundConfig.ArmorHitSFX, _enemy.transform, 0);
            _enemy.EnemyAnimator.TintFlash(_enemy.Data.TintShieldFlash, _enemy.Data.TintFlashDuration);
            _enemy.EnemyAnimator.PlayArmorVisualEffect();
        }
        else
        {
            if (!_armorBreak)
            {
                _armorBreak = true;
                EchosCry.Sound.PlaySFX(_enemy.SoundConfig.ArmorBreakSFX, _enemy.transform, 0);
            }
            _enemy.StateData.IsStaggered = true;
            DecalManager.Instance.GetBloodDecal().transform.position = _enemy.transform.position;
            EchosCry.Sound.PlaySFX(_enemy.SoundConfig.HitSFX, _enemy.transform, 0);
            _enemy.EnemyAnimator.TintFlash(_enemy.Data.TintHealthFlash, _enemy.Data.TintFlashDuration);
            _enemy.EnemyAnimator.PlayBloodVisualEffect();

            _enemy.Rigidbody.isKinematic = false;
            _enemy.Rigidbody.AddForce(attackData.Force * attackData.Direction, attackData.ForceMode);
            StartCoroutine(KnockBackDuration(_enemy.Data.KnockbackDuration));
        }
            
        if(DamageLabelManager.Instance != null)
            DamageLabelManager.Instance.SpawnPopup(damage, _enemy.transform.position, Color.white);
        
        
        if(_enemy.EnemyHealthUI != null) _enemy.EnemyHealthUI.UpdateUI(_enemy.Health.CurrentHealth, 
            _enemy.Health.MaxHealth, 
            _enemy.Health.CurrentArmor, 
            _enemy.Health.MaxArmor);
        
        HitStop.Instance.Execute(0.05f);
    }

    private IEnumerator KnockBackDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        _enemy.Rigidbody.isKinematic = true;
    }
}
