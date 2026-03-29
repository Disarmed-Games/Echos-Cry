using AudioSystem;
using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Echo's Cry/Strategies/Attack/Melee")]
public class MeleeAttack : AttackMethod
{
    [SerializeField] private Vector3 _boxExtents;
    [SerializeField] private LayerMask _playerMask;
    [SerializeField] private float _distance;
    [SerializeField] private soundEffect _attackSound;

    public override void Execute(float damage, Vector3 direction, Transform origin)
    {
        _attackEnded = false;
        StartCoroutine(StartAttack(damage, direction, origin));
    }
    private IEnumerator StartAttack(float damage, Vector3 direction, Transform origin)
    {
        Debug.Log("Attack started");
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
                Debug.Log("Hit");
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + _distance * transform.forward.normalized, _boxExtents * 2);
    }
}