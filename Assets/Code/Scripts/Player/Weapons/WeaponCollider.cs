using System.Collections.Generic;
using UnityEngine;

//Handles attack collider.
//When either a melee trigger or projectile trigger collide with the enemy, they will do damage
//Projectiles will be returned to pool/destroyed

public class WeaponCollider : MonoBehaviour
{
    [SerializeField] private Weapon _weaponContext;
    private AttackInfo attackInfo;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EffectHandler>(out EffectHandler passiveEffectHandler))
        {
            if (PlayerComboMeter.CurrentMeterState == PlayerComboMeter.MeterState.OneThird)
                passiveEffectHandler.ApplyEffect(_weaponContext.CurrentAttackData.PassiveEffects.OneThirdEffect);
            else if (PlayerComboMeter.CurrentMeterState == PlayerComboMeter.MeterState.TwoThirds)
                passiveEffectHandler.ApplyEffect(_weaponContext.CurrentAttackData.PassiveEffects.TwoThirdsEffect);
            else if (PlayerComboMeter.CurrentMeterState == PlayerComboMeter.MeterState.Full)
                passiveEffectHandler.ApplyEffect(_weaponContext.CurrentAttackData.PassiveEffects.FullEffect);
        }

        if (other.TryGetComponent<IDamageable>(out IDamageable damagable))
        {
            damagable.Execute(attackInfo);
            if (_weaponContext != null) _weaponContext.AddColliderToList(other, attackInfo.HitQuality);
        }
    }

    public void UpdateAttack(AttackInfo attack)
    {
        attackInfo = attack;
    }
}