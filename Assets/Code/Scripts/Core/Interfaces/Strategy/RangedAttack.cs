using AudioSystem;
using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.STP;
public class RangedAttack : AttackMethod
{
    [SerializeField] protected GameObject _projectilePrefab;
    [SerializeField] protected soundEffect _projectileSound;
    [SerializeField] protected int _projectileCount;
    [SerializeField] protected Rigidbody _rb;
    [SerializeField] protected float _blowbackForce = 2f;
    [SerializeField] protected Vector3 spawnOffset;

    public override void Execute(float damage, Vector3 direction, Transform origin)
    {
        _attackEnded = false;
        if(_rb != null) _rb.isKinematic = false;
        StartCoroutine(WaitUntilBeat(damage, direction, origin));
    }
    protected virtual void ShootProjectile(Transform origin, Vector3 direction, float damage)
    {
        RBProjectilePool pool = RBProjectileManager.Instance.RequestPool(_projectilePrefab);
        pool.UseProjectile(origin.position + spawnOffset, direction, damage);

        SoundEffectManager.Instance.Builder
            .SetSound(_projectileSound)
            .SetSoundPosition(origin.position + spawnOffset)
            .ValidateAndPlaySound();
    }

    protected virtual IEnumerator ProjectileAttack(float damage, Vector3 direction, Transform origin)
    {
        int count = 0;
        while(count < _projectileCount)
        {
            Vector3 attackDirection = (Player.Instance.transform.position - origin.position).normalized;
            _rb.AddForce(-attackDirection * _blowbackForce, ForceMode.Impulse);
            ShootProjectile(origin, attackDirection, damage);
            count++;
            yield return new WaitForSeconds(BeatManager.Instance.GetTimeBetweenBeats());
        }
        _attackEnded = true;
    }

    private IEnumerator WaitUntilBeat(float damage, Vector3 direction, Transform origin)
    {
        while (!TempoConductor.Instance.IsOnBeat())
        {
            yield return null;
        }
        StartCoroutine(ProjectileAttack(damage, direction, origin));
    }
}