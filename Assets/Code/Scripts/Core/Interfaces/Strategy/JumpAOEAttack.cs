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
                AttackInfo attackInfo = new AttackInfo.Builder().SetDamage(damage).Build();
                damageable.Execute(attackInfo);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        int segments = 50;
        float angleStep = 360f / segments;

        Vector3 prevPoint = transform.position + new Vector3(_aoeRadius, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;

            Vector3 newPoint = transform.position + new Vector3(
                Mathf.Cos(angle) * _aoeRadius,
                0,
                Mathf.Sin(angle) * _aoeRadius
            );

            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}
