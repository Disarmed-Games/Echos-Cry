using UnityEngine;

public class DummyDamageable : MonoBehaviour, IDamageable
{
    [Header("Relevant Components")]
    [SerializeField] private Collider _collider;
    [SerializeField] private EnemyAnimator _enemyAnimator;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private EnemySoundConfig _soundConfig;
    [Header("Event Channel (Subscriber)")]
    [Tooltip("Invoked when player's attack ends")]
    [SerializeField] EventChannel _playerAttackEndedChannel;

    public virtual void Execute(AttackInfo attackData)
    {
        _collider.enabled = false;
        EchosCry.Sound.PlaySFX(_soundConfig.HitSFX, gameObject.transform, 0);
        _enemyAnimator.TintFlash(Color.red, 0.15f);
        _particleSystem.Play();

        if (DamageLabelManager.Instance != null && DamageLabelManager.Instance.isActiveAndEnabled)
            DamageLabelManager.Instance.SpawnPopup(attackData.Damage, transform.position, Color.white);
    }

    private void OnEnable()
    {
        _playerAttackEndedChannel.Channel += ResetCollider;
    }
    private void OnDisable()
    {
        _playerAttackEndedChannel.Channel -= ResetCollider;
    }

    private void ResetCollider() => _collider.enabled = true;
}