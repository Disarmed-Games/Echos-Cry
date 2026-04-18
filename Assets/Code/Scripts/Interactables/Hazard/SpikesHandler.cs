using UnityEngine;

public class SpikesHandler : MonoBehaviour
{
    [SerializeField] private float _spikeDamage = 5f;
    [SerializeField] private float _spikeDamageWait = 1f;
    [SerializeField] private Animator _spikesAnimator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            _spikesAnimator.SetTrigger("Activate");
            AttackInfo attack = new AttackInfo.Builder().SetDamage(_spikeDamage).Build();
            damageable.Execute(attack);
        }
    }
}
