using UnityEngine;
using System.Collections;
using EchosCry;
using AudioSystem;

public class SpikesHandler : MonoBehaviour
{
    [SerializeField] private float _spikeDamage = 5f;
    [SerializeField] private float _spikeDamageWait = 1f;
    [SerializeField] private Animator _spikesAnimator;
    [SerializeField] private soundEffect _clickSFX;
    [SerializeField] private soundEffect _spikeSFX;
    private Coroutine _damageRoutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            _damageRoutine = StartCoroutine(DelayedDamage(damageable));
            Sound.PlaySFX(_clickSFX, transform, 0);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out _))
        {
            if (_damageRoutine != null)
            {
                StopCoroutine(_damageRoutine);
                _damageRoutine = null;
            }
        }
    }

    private IEnumerator DelayedDamage(IDamageable damageable)
    {
        yield return new WaitForSeconds(_spikeDamageWait);

        _spikesAnimator.SetTrigger("Activate");
        Sound.PlaySFX(_spikeSFX, transform, 0);

        AttackInfo attack = new AttackInfo.Builder().SetDamage(_spikeDamage).Build();
        damageable.Execute(attack);
        _damageRoutine = null;
    }
}