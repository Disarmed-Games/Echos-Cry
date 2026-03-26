using UnityEngine;

public class BombData : ScriptableObject
{
    [SerializeField] private float _distanceCheck;
    [SerializeField] private float _attackChargeTime;
    [SerializeField] private float _attackDashForce;
    [SerializeField] private float _attackDuration;
    [SerializeField] private float _attackCooldown;
    [SerializeField] private float _staggerDuration;
    [SerializeField] private float _knockbackForce;
    [SerializeField] private float _baseDamage;

    //public readonly int FlyHashCode = Animator.StringToHash("Fly");
    //public readonly int AttackHashCode = Animator.StringToHash("Attack");

    public float DistanceCheck { get { return _distanceCheck; } }
    public float AttackChargeTime { get { return _attackChargeTime; } }
    public float AttackDashForce { get { return _attackDashForce; } }
    public float AttackDuration { get => _attackDuration; }
    public float AttackCooldown { get => _attackCooldown; }
    public float StaggerDuration { get => _staggerDuration; }
    public float KnockbackForce { get => _knockbackForce; }
    public float BaseDamage { get => _baseDamage; }
}
