using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class Enemy : MonoBehaviour
{
    private static Dictionary<EnemyType, EnemyInitMethod> _initMethods;
    private static EnemyStateMachine _stateMachine;
    private static EnemyStateCache   _stateCache;

    private EnemyPool _pool;

    [SerializeField] private EnemyType _enemyType = EnemyType.Bat;

    private bool IsPooled => _pool != null;
    public bool Invulnerable { get; set; } = false;

    [Header("Enemy-Related Components")]
    [SerializeField] private HealthSystem _health;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private EnemyAnimator _enemyAnimator;
    [SerializeField] private EnemySoundConfig _soundConfig;
    [SerializeField] private Collider _collider;
    [SerializeField] private EffectHandler _passiveEffectHandler;
    [SerializeField] private GameObject _deathEffect;
    [SerializeField] private EnemyData _data;
    [SerializeField] private EnemyHealthUI _enemyHealthUI;
    
    private EnemyStateHandler _stateHandler;
    private EnemyStateData _stateData;

    public AttackInfo DeathInfo;

    [Header("Strategies")]
    [SerializeField] private AttackMethod[] _attackStrats;
    [SerializeField] private TargetStrategy[]   _targetStrats;
    [SerializeField] private MovementStrategy[] _movementStrats;
    [SerializeField] private ItemDropStrategy _drops;

    [Header("Event Channel (Subscriber)")]
    [Tooltip("Invoked when player's attack ends")]
    [SerializeField] private EventChannel _playerAttackEndChannel;
    [Header("Event Channel (Broadcaster)")]
    [SerializeField] private IntEventChannel _updateWaveCount;

    public EnemyPool Pool { get => _pool; set => _pool = value; }
    public EnemyStateMachine StateMachine { get => _stateMachine; }
    public EnemyStateCache StateCache { get => _stateCache; }
    public HealthSystem Health { get => _health; }
    public NavMeshAgent NavMeshAgent { get => _navMeshAgent; }
    public Rigidbody Rigidbody { get => _rigidbody; }
    public EnemyAnimator EnemyAnimator { get => _enemyAnimator; }
    public EnemySoundConfig SoundConfig { get => _soundConfig; }
    public Collider Collider { get => _collider; }
    public EffectHandler PassiveEffectHandler { get => _passiveEffectHandler; }
    public EnemyHealthUI EnemyHealthUI { get => _enemyHealthUI; }
    public AttackMethod[] AttackStrategies { get => _attackStrats; }
    public TargetStrategy[] TargetStrategy { get => _targetStrats; }
    public MovementStrategy[] MovementStrategy { get => _movementStrats; }
    public ItemDropStrategy DropsStrategy { get => _drops; }
    public EnemyData Data { get => _data; }
    public EnemyType EnemyType { get => _enemyType; }
    public EnemyStateHandler StateHandler { get => _stateHandler; set => _stateHandler = value; }
    public EnemyStateData StateData { get => _stateData; set => _stateData = value; }

    public int EnemySpawnerID;

   private void Awake()
    {   
        _stateMachine ??= new();
        _stateCache ??= new();

        _initMethods ??= new()
        {
            {EnemyType.Bat, new BatInitMethod()},
            {EnemyType.Crawler, new RangeInitMethod() },
            {EnemyType.Frog, new BombInitMethod()},
            {EnemyType.Walker, new WalkerInitMethod() },
            {EnemyType.Slime, new SlimeInitMethod() },
            {EnemyType.Turtle, new TurtleInitMethod()},
            {EnemyType.Tower, new TowerInitMethod()},
        };

        _stateData = new();

        _initMethods[_enemyType].Execute(this);
    }

    private void OnEnable()
    {
        TickManager.Instance.GetTimer(0.2f).Tick += TickCheck;
        _playerAttackEndChannel.Channel += ResetCollider;
        
        ResetCollider();
        _health.ResetSystem();
        _stateData.Reset();
        _enemyHealthUI.UpdateUI(_health.CurrentHealth, _health.MaxHealth, _health.CurrentArmor, _health.MaxArmor);
        _rigidbody.isKinematic = true;
        _stateMachine.SwitchStates(_stateHandler.StartState, this);
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        if(TickManager.Instance != null) TickManager.Instance.GetTimer(0.2f).Tick -= TickCheck;
        _playerAttackEndChannel.Channel -= ResetCollider; 
    }

    private void Update()
    {
        if (_stateHandler == null || _stateData == null) return;
        _stateMachine.CheckSwitchStates(this);
        _stateMachine.UpdateStates(this);
    }
    private void FixedUpdate()
    {
        if (_stateHandler == null || _stateData == null) return;
        _stateMachine.FixedUpdateStates(this);
    }

    private void ResetCollider() => _collider.enabled = true;

    public void HandleDeath()
    {
        _updateWaveCount.Invoke(EnemySpawnerID);

        DeathEffectHandler deathEffectPrefab = Instantiate(_deathEffect, transform.position, Quaternion.identity).GetComponent<DeathEffectHandler>();
        deathEffectPrefab.SetSpriteShape(_enemyAnimator.EnemySprite);

        ScoreManager scoreManager = ScoreManager.Instance;
        if (scoreManager != null) {
            int score = scoreManager.CalculateScore(DeathInfo, _data.BaseScore);
            scoreManager.AddScore(score);
            scoreManager.SpawnScoreText(score, transform);
        }

        if (IsPooled) _pool.ReleaseEnemy(this);
        else Destroy(gameObject);
    }

    private void TickCheck()
    {
        _stateMachine.CheckTickSwitchStates(this);
    }
}