using AudioSystem;
using UnityEngine;

public class EnvironmentObject : MonoBehaviour, IDamageable
{
    [SerializeField] private ItemDropStrategy _itemDropStrategy;
    [SerializeField] private GameObject _destroyedPrefab;
    [SerializeField] soundEffect _destroySFX;
    [SerializeField] soundEffect hitSFX;
    [SerializeField] private bool _isDestructable;
    
    [SerializeField] private float _health;
    public float Health { get => _health; set => _health = value; }

    public void Execute(AttackInfo attackData)
    {
        _health -= attackData.Damage;
        SoundEffectManager.Instance.Builder
            .SetSound(hitSFX)
            .SetSoundPosition(transform.position)
            .ValidateAndPlaySound();
        if(!HasHealth()) HandleObjectDestroyed();
    }

    private bool HasHealth()
    {
        if (_health <= 0) return false;
        else return true;
    }
    private void HandleObjectDestroyed()
    {
        Instantiate(_destroyedPrefab, transform.position, transform.rotation);
        if (_itemDropStrategy != null) _itemDropStrategy.Execute(transform);
        SoundEffectManager.Instance.Builder
            .SetSound(_destroySFX)
            .SetSoundPosition(transform.position)
            .ValidateAndPlaySound();
        Destroy(gameObject);
    }
}