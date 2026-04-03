using UnityEngine;

public class DashWeapon : Weapon
{
    [SerializeField] private AttackData _dashAttackData;
    [SerializeField] private float _dashAttackTime = 0.1f; //How long the dashes collider will be active for.

    protected override void Attack()
    {
        StopAllCoroutines();
        _weaponCollider.UpdateAttack(_currentAttackData.BaseDamage * _playerAttackDamage.BaseDamageMultiplier, TempoConductor.Instance.CurrentHitQuality);
        StartCoroutine(AttackLengthCoroutine(_dashAttackTime));
    }
    protected override void OnPrimaryAction()
    {
        _currentAttackData = _dashAttackData;
    }
}