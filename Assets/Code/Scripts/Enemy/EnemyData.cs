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
    [SerializeField] private float _baseDamage;
    [SerializeField] private float _stoppingDistance;

    [SerializeField] private float _tintFlashDuration = 0.15f;
    [SerializeField] private float _knockbackDuration = 0.15f;
    [SerializeField] private Color _tintHealthFlash = Color.red;
    [SerializeField] private Color _tintShieldFlash = Color.blue;

    [SerializeField] private int _baseScore = 100;

    public float DistanceCheck { get { return _distanceCheck; } }
    public float AttackChargeTime { get { return _attackChargeTime; } }
    public float AttackDashForce { get { return _attackDashForce; } }
    public float AttackDuration { get => _attackDuration; }
    public float AttackCooldown { get => _attackCooldown; }
    public float StaggerDuration { get => _staggerDuration; }
    public float BaseDamage { get => _baseDamage; }
    public float StoppingDistance { get => _stoppingDistance; }
    public float TintFlashDuration { get => _tintFlashDuration; }
    public float KnockbackDuration { get => _knockbackDuration; }
    public Color TintHealthFlash { get => _tintHealthFlash; }
    public Color TintShieldFlash { get => _tintShieldFlash; }
    public int BaseScore { get => _baseScore; set => _baseScore = value; }
}