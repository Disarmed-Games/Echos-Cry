using AudioSystem;
using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.STP;
public class RangedAttack : AttackMethod
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private soundEffect _projectileSound;
    [SerializeField] private int _projectileCount;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _blowbackForce = 2f;

    public override void Execute(float damage, Vector3 direction, Transform origin)
    {
        _attackEnded = false;
        if(_rb != null) _rb.isKinematic = false;
        StartCoroutine(WaitUntilBeat(damage, direction, origin));
    }
    private void ShootProjectile(Transform origin, Vector3 direction, float damage)
    {
        RBProjectilePool pool = RBProjectileManager.Instance.RequestPool(_projectilePrefab);
        pool.UseProjectile(origin.position, direction, damage);
        if (!SoundEffectManager.Instance.Builder.GetSoundPlayer().IsSoundPlaying())
        {
            SoundEffectManager.Instance.Builder
                .SetSound(_projectileSound)
                .SetSoundPosition(origin.position)
                .ValidateAndPlaySound();
        }
    }

    private IEnumerator ProjectileAttack(float damage, Vector3 direction, Transform origin)
    {
        int count = 0;
        while(count < _projectileCount)
        {
            Vector3 attackDirection = (PlayerRef.Transform.position - origin.position).normalized;
            _rb.AddForce(-attackDirection * _blowbackForce, ForceMode.Impulse);
            ShootProjectile(origin, attackDirection, damage);
            count++;
            yield return new WaitForSeconds(TempoConductor.Instance.TimeBetweenBeats);
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