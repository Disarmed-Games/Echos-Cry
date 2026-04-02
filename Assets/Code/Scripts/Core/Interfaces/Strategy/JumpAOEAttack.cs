using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.STP;

public class JumpAOEAttack : AttackMethod
{
    [SerializeField] GameObject _aoeEffect;
    [SerializeField] float _aoeEffectTime;
    [SerializeField] float _aoeRadius;
    [SerializeField] LayerMask _layerMask;
    [SerializeField] int _maxHits;
    [SerializeField] private float _jumpDuration;
    [SerializeField] private float _jumpDashForce;
    [SerializeField] private Rigidbody rb;

    public override void Execute(float damage, Vector3 direction, Transform origin)
    {
        _attackEnded = false;
        StartCoroutine(JumpAttack(damage, direction, origin));
    }

    IEnumerator JumpAttack(float damage, Vector3 direction, Transform origin)
    {
        while (!TempoConductor.Instance.IsOnBeat())
        {
            yield return null;
        }

        rb.isKinematic = false;
        rb.AddForce(_jumpDashForce * direction, ForceMode.Impulse);
        yield return new WaitForSeconds(_jumpDuration);

        GameObject fireRing = GameObject.Instantiate(_aoeEffect);
        fireRing.transform.position = rb.transform.position;
        if (fireRing.TryGetComponent<ParticleSystem>(out ParticleSystem particles))
        {
            particles.Play();
            StartCoroutine(AOEVisualDuration(particles));
        }

        Collider[] colliders = new Collider[_maxHits];
        int count = Physics.OverlapSphereNonAlloc(origin.position, _aoeRadius, colliders, _layerMask);
        for (int i = 0; i < count; i++)
        {
            if (colliders[i] != null
                && colliders[i].TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damageable.Execute(damage);
            }
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
