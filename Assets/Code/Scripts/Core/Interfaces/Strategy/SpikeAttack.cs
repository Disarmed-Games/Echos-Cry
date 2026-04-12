using System.Collections;
using UnityEngine;

public class SpikeAttack : RangedAttack
{
    [SerializeField] private Enemy _enemy;
    [SerializeField] private float attackCooldown;

    protected override IEnumerator ProjectileAttack(float damage, Vector3 direction, Transform origin)
    {
        _enemy.Invulnerable = true;

        float angleStep = Mathf.PI * 2f / _projectileCount;

        int count = 0;
        while (count < _projectileCount)
        {
            float angle = count * angleStep;
            Vector3 attackDirection = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));

            _rb.AddForce(-attackDirection * _blowbackForce, ForceMode.Impulse);
            ShootProjectile(origin, attackDirection, damage);
            count++;
        }

        yield return new WaitForSeconds(attackCooldown);
        _enemy.Invulnerable = false;
        _attackEnded = true;
    }
}