using UnityEngine;

[CreateAssetMenu(menuName = "Stats Data", fileName = "Echo's Cry/Stats/Stats Data")]
public class StatsData : ScriptableObject
{
    [SerializeField] private float _movementMultiplier = 1f;
    [SerializeField] private float _damageMultiplier = 1f;
    [SerializeField] private float _knockbackMultiplier = 1f;

    public float MovementMultiplier { get => _movementMultiplier;  }
    public float DamageMultiplier { get => _damageMultiplier; }
    public float KnockbackMultiplier { get => _knockbackMultiplier; }
}
public class Stats
{
    public float MovementMultiplier = 1f;
    public float DamageMultiplier = 1f;
    public float KnockbackMultiplier = 1f;
    private readonly float _defaultMovement = 1f;
    private readonly float _defaultDamage = 1f;
    private readonly float _defaultKnockback = 1f;
    public Stats()
    {
        MovementMultiplier = 1f;
        DamageMultiplier = 1f;
        KnockbackMultiplier = 1f;
        _defaultMovement = 1f;
        _defaultDamage = 1f;
        _defaultKnockback = 1f;
    }
    public Stats(StatsData data)
    {
        MovementMultiplier = data.MovementMultiplier;
        DamageMultiplier = data.DamageMultiplier;
        KnockbackMultiplier = data.KnockbackMultiplier;

        _defaultMovement = MovementMultiplier;
        _defaultDamage = DamageMultiplier;
        _defaultKnockback = KnockbackMultiplier;
    }
    public void Reset()
    {
        MovementMultiplier = _defaultMovement;
        DamageMultiplier = _defaultDamage;
        KnockbackMultiplier = _defaultKnockback;
    }
}