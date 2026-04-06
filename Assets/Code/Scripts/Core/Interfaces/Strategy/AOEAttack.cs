using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.STP;

public class AOEAttack : AttackMethod
{
    [SerializeField] GameObject _aoeEffect;
    [SerializeField] float _aoeEffectTime;
    [SerializeField] float _aoeRadius;
    [SerializeField] LayerMask _layerMask;
    [SerializeField] int _maxHits;
    [SerializeField] Rigidbody _rb;

    public override void Execute(float damage, Vector3 direction, Transform origin)
    {
        Collider[] colliders = new Collider[_maxHits];
        int count = Physics.OverlapSphereNonAlloc(origin.position, _aoeRadius, colliders, _layerMask);
        for (int i = 0; i < count; i++)
        {
            if(colliders[i] != null 
                && colliders[i].TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damageable.Execute(damage);
            }
        }

        GameObject fireRing = GameObject.Instantiate(_aoeEffect);
        fireRing.transform.position = _rb.transform.position;
        if (fireRing.TryGetComponent<ParticleSystem>(out ParticleSystem particles))
        {
            particles.Play();
            StartCoroutine(AOEVisualDuration(particles));
        }

        _attackEnded = true;
    }
    private IEnumerator AOEVisualDuration(ParticleSystem particles)
    {
        yield return new WaitForSeconds(_aoeEffectTime);
        particles.Stop();
        GameObject.Destroy(particles.gameObject, _aoeEffectTime);
    }
}
