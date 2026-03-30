using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

//ORDER:
//- Create state transitions and then create StateNode
//- Add StateNode to dictionary using enums
//- Create a state container, setting the start state and placing dictionary 

public class Enemy : MonoBehaviour
{
    private EnemyStateCache _stateCache;
    private EnemyStateMachine _stateMachine;
    private EnemyPool _pool;
    
    private static NewEnemyStateMachine _newStateMachine;
    private static NewEnemyStateCache   _newStateCache;

    [SerializeField] private EnemyStateCache.EnemyStates _spawnState;
    private bool IsPooled => _pool != null;

    [Header("Damage")]
    [SerializeField] private float _attackDamage;

    [Header("Enemy-Related Components")]
    [SerializeField] private HealthSystem _health;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private NPCAnimator _npcAnimator;
    [SerializeField] private EnemySoundConfig _soundConfig;
    [SerializeField] private Collider _collider;
    [SerializeField] private PassiveEffectHandler _passiveEffectHandler;
    [SerializeField] private GameObject _deathEffect;
    [SerializeField] private EnemyData _data;
    
    private EnemyStateHandler _stateHandler;
    public EnemyStateHandler StateHandler { get => _stateHandler; set => _stateHandler = value; }
    
    private EnemyStateData _stateData;
    public EnemyStateData StateData { get => _stateData; set => _stateData = value; }

    [Header("Strategies")]
    [SerializeField] private AttackMethod[] _attackStrats;
    [SerializeField] private TargetStrategy[]   _targetStrats;
    [SerializeField] private MovementStrategy[] _movementStrats;
    [SerializeField] private ItemDropStrategy _drops;
    [SerializeField] private SoundStrategy _soundStrategy;
    [SerializeField] private EnemyCacheStrategy _enemyCacheStrategy;

    [Header("Event Channel (Subscriber)")]
    [Tooltip("Invoked when player's attack ends")]
    [SerializeField] private EventChannel _playerAttackEndChannel;
    [Header("Event Channel (Broadcaster)")]
    [SerializeField] private IntEventChannel _updateWaveCount;


    public EnemyStateCache StateCache { get => _stateCache; }
    public EnemyStateMachine StateMachine { get => _stateMachine; }
    public EnemyPool Pool { get => _pool; set => _pool = value; }

    public NewEnemyStateMachine NewStateMachine { get => _newStateMachine; }
    public NewEnemyStateCache NewStateCache { get => _newStateCache; }

    public HealthSystem Health { get => _health; }
    public NavMeshAgent NavMeshAgent { get => _navMeshAgent; }
    public Rigidbody Rigidbody { get => _rigidbody; }
    public NPCAnimator NPCAnimator { get => _npcAnimator; }
    public EnemySoundConfig SoundConfig { get => _soundConfig; }
    public Collider Collider { get => _collider; }
    public PassiveEffectHandler PassiveEffectHandler { get => _passiveEffectHandler; }

    public AttackMethod[] AttackStrategies { get => _attackStrats; }
    public TargetStrategy[] TargetStrategy { get => _targetStrats; }
    public MovementStrategy[] MovementStrategy { get => _movementStrats; }
    public ItemDropStrategy DropsStrategy { get => _drops; }
    public SoundStrategy SoundStrategy { get => _soundStrategy; }
    public EnemyData Data { get => _data; }

    public int EnemySpawnerID;

   private void Awake()
    {   
        //_stateMachine = new();
        //_stateCache = new();

        _newStateMachine ??= new();
        _newStateCache ??= new();

        _stateData = new();

        _enemyCacheStrategy.Execute(_stateCache, this);
        //_stateMachine.Init(_stateCache.StartState); 
    }
    private void OnEnable()
    {
        //_stateCache?.Enable();
        TickManager.Instance.GetTimer(0.2f).Tick += TickCheck;
        _playerAttackEndChannel.Channel += ResetCollider;
        ResetCollider();
    }
    private void OnDisable()
    {
        //_stateCache?.Disable();
        if(TickManager.Instance != null) TickManager.Instance.GetTimer(0.2f).Tick -= TickCheck;
        _playerAttackEndChannel.Channel -= ResetCollider; 
    }
    private void OnDestroy()
    {
        _stateMachine = null;
        _stateCache = null;
    }

    private void Update()
    {
        //_stateMachine.UpdateState();

        if (_stateHandler == null || _stateData == null) return;
        _newStateMachine.CheckSwitchStates(this);
        _newStateMachine.UpdateStates(this);
    }
    private void FixedUpdate()
    {
        //_stateMachine.FixedUpdateState();

        if (_stateHandler == null || _stateData == null) return;
        _newStateMachine.FixedUpdateStates(this);
    }

    private void ResetCollider() => _collider.enabled = true;

    public void HandleDeath()
    {
        //Effects and Updates
        _updateWaveCount.Invoke(EnemySpawnerID);

        DeathEffectHandler deathEffectPrefab = Instantiate(_deathEffect, transform.position, Quaternion.identity).GetComponent<DeathEffectHandler>();
        deathEffectPrefab.SetSpriteShape(_npcAnimator.NPCSprite);

        //Enemy Pooling
        if (IsPooled)
        {
            _stateMachine.SwitchState(_stateCache.RequestState(_spawnState));

            if (_stateHandler != null && _stateData != null)
                _newStateMachine.SwitchStates(_stateHandler.StartState, this);

            _health.ResetSystem();
            _pool.ReleaseEnemy(this);
        }
        else Destroy(gameObject);
    }

    private void TickCheck()
    {
        _newStateMachine.CheckTickSwitchStates(this);
    }

    public class Builder
    {
        public Enemy Build()
        {
            Enemy newEnemy = new GameObject("Enemy").AddComponent<Enemy>();
            return newEnemy;
        }
    }
}