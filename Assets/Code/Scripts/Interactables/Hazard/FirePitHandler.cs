using UnityEngine;

public class FirePitHandler : MonoBehaviour
{
    [SerializeField] private float _damage = 5f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.Execute(_damage);
        }
    }
}
