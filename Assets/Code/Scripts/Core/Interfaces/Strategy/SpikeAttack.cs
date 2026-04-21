using System.Collections;
using UnityEngine;

public class SpikeAttack : RangedAttack
{
    [SerializeField] private Enemy _enemy;
    [SerializeField] private float projectileSpawnDistance = 1f;
    [SerializeField] private float invincibleCooldown;

    protected override void ShootProjectile(Transform origin, Vector3 direction, float damage)
    {
        RBProjectilePool pool = RBProjectileManager.Instance.RequestPool(_projectilePrefab);
        pool.UseProjectile(origin.position + spawnOffset + direction * projectileSpawnDistance, direction, damage);

        SoundEffectManager.Instance.Builder
            .SetSound(_projectileSound)
            .SetSoundPosition(origin.position + spawnOffset + direction * 1f)
            .ValidateAndPlaySound();
    }

    protected override IEnumerator ProjectileAttack(float damage, Vector3 direction, Transform origin)
    {
        _enemy.Invulnerable = true;

        int count = 0;
        while (count < _projectileCount)
        {
            float angleStep = (Mathf.PI * 2f / _projectileCount);
            float angle = count * angleStep;
            Vector3 attackDirection = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)).normalized;

            //_rb.AddForce(-attackDirection * _blowbackForce, ForceMode.Impulse);
            ShootProjectile(origin, attackDirection, damage);
            count++;
        }

        yield return new WaitForSeconds(invincibleCooldown);
        _enemy.Invulnerable = false;
        _attackEnded = true;
    }
}