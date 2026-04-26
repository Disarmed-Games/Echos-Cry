using System.Collections.Generic;
using UnityEngine;

//Handles attack collider.
//When either a melee trigger or projectile trigger collide with the enemy, they will do damage
//Projectiles will be returned to pool/destroyed

public class WeaponCollider : MonoBehaviour
{
    public struct ColliderInfo
    {
        public Collider collider;
        public TempoConductor.HitQuality hitQuality;
    }
    private List<ColliderInfo> _hitColliders = new();
    public List<ColliderInfo> HitColliders { get => _hitColliders; }

    [SerializeField] private Weapon _weaponContext;
    private AttackInfo attackInfo;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out IDamageable damagable))
        {
            damagable.Execute(attackInfo);
            _hitColliders.Add(new ColliderInfo { collider = other, hitQuality = attackInfo.HitQuality });
        }
    }

    public void UpdateAttack(AttackInfo attack)
    {
        attackInfo = attack;
    }
    public void ClearColliderList()
    {
        _hitColliders.Clear();
    }
}