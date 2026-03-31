using AudioSystem;
using EchosCry.Enemy.Animation;
using System;
using System.Collections;
using UnityEngine;

public class MeleeAttack : AttackMethod
{
    [SerializeField] private Vector3 _boxExtents;
    [SerializeField] private LayerMask _playerMask;
    [SerializeField] private float _distance;
    [SerializeField] private soundEffect _attackSound;
    [SerializeField] private float _duration;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _dashForce = 4;

    public override void Execute(float damage, Vector3 direction, Transform origin)
    {
        _attackEnded = false;
        if(_rb != null)
        {
            _rb.isKinematic = false;
            _rb.AddForce(_dashForce * direction, ForceMode.Impulse);
        }
        StartCoroutine(StartAttack(damage, direction, origin));
        StartCoroutine(AttackDuration());
    }
    private IEnumerator StartAttack(float damage, Vector3 direction, Transform origin)
    {
        while (true)
        {
            if (Physics.BoxCast(
                        center: origin.position,
                        halfExtents: _boxExtents,
                        direction: direction,
                        hitInfo: out RaycastHit hitInfo,
                        orientation: origin.rotation,
                        maxDistance: _distance,
                        layerMask: _playerMask))
            {
                hitInfo.collider.gameObject.GetComponent<IDamageable>().Execute(damage);
                if (!SoundEffectManager.Instance.Builder.GetSoundPlayer().IsSoundPlaying())
                {
                    SoundEffectManager.Instance.Builder
                        .SetSound(_attackSound)
                        .SetSoundPosition(origin.position)
                        .ValidateAndPlaySound();
                }
                _attackEnded = true;
                break;
            }
            else yield return null;
        }
    }
    private IEnumerator AttackDuration()
    {
        yield return new WaitForSeconds(_duration);
        _attackEnded = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + _distance * transform.forward.normalized, _boxExtents * 2);
    }
}