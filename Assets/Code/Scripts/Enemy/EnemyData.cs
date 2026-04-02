using UnityEngine;

[CreateAssetMenu(menuName = "Echo's Cry/Enemy/Enemy Data/General")]
public class EnemyData : ScriptableObject
{
    [SerializeField] private float _distanceCheck;
    [SerializeField] private float _attackChargeTime;
    [SerializeField] private float _attackDashForce;
    [SerializeField] private float _attackDuration;
    [SerializeField] private float _attackCooldown;
    [SerializeField] private float _staggerDuration;
    [SerializeField] private float _knockbackForce;
    [SerializeField] private float _baseDamage;
    [SerializeField] private float _stoppingDistance;

    public float DistanceCheck { get { return _distanceCheck; } }
    public float AttackChargeTime { get { return _attackChargeTime; } }
    public float AttackDashForce { get { return _attackDashForce; } }
    public float AttackDuration { get => _attackDuration; }
    public float AttackCooldown { get => _attackCooldown; }
    public float StaggerDuration { get => _staggerDuration; }
    public float KnockbackForce { get => _knockbackForce; }
    public float BaseDamage { get => _baseDamage; }
    public float StoppingDistance { get => _stoppingDistance; }
}