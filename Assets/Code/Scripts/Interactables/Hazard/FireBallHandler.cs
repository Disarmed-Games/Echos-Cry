using UnityEngine;

public class FireBallHandler : MonoBehaviour
{
    [SerializeField] private float _damage;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.Execute(_damage);
        }
    }
}
