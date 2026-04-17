using UnityEngine;

public class FireBallHandler : MonoBehaviour
{
    [SerializeField] private float _damage;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            AttackInfo attack = new AttackInfo.Builder().SetDamage(_damage).Build();
            damageable.Execute(attack);
        }
    }
}
