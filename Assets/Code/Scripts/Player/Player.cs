using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Relevant Player Components")]
    [SerializeField] private PlayerHealth _health;
    [SerializeField] private PlayerComboMeter _comboMeter;
    [SerializeField] private PlayerAnimator _animator;
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private WeaponHolder _weaponHolder;
    [SerializeField] private PlayerOrientation _orientation;
    [SerializeField] private PlayerCurrencySystem _currencySystem;
    [SerializeField] private PlayerXP _xp;
    [SerializeField] private InputTranslator _inputTranslator;
    [SerializeField] private SoundStrategy _sfx;
    [SerializeField] private SFXConfig _sfxConfig;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private DashWeapon _dashAttack;
    [SerializeField] private PlayerStats _stats;
    [SerializeField] private SpamPrevention _spamPrevention;
    [SerializeField] private HeatGauge _heatGauge;

    [Header("Event Channel (Broadcaster)")]
    [SerializeField] EventChannel _attackEndedChannel;

    private PlayerStateMachine _playerStateMachine;
    private PlayerStateCache _playerStateCache;
    
    public PlayerHealth Health { get => _health; }
    public PlayerComboMeter ComboMeter { get => _comboMeter; }
    public PlayerAnimator Animator { get => _animator; }
    public SoundStrategy SFX { get => _sfx; }
    public SFXConfig SFXConfig { get => _sfxConfig; }
    public PlayerMovement Movement { get => _movement; }
    public WeaponHolder WeaponHolder { get => _weaponHolder; }
    public PlayerOrientation Orientation { get => _orientation; }
    public PlayerCurrencySystem CurrencySystem { get => _currencySystem; }
    public PlayerXP XP { get => _xp; }
    public InputTranslator InputTranslator { get => _inputTranslator; }
    public Rigidbody RB { get => _rb; }
    public DashWeapon DashAttack { get => _dashAttack; }
    public PlayerStats Stats { get => _stats; }
    public SpamPrevention SpamPrevention { get => _spamPrevention; }
    public HeatGauge HeatGauge { get => _heatGauge; }

    private void InitStateCache()
    {
        _playerStateCache.AddState(
            PlayerStateCache.PlayerState.Idle,
            new PlayerIdleState (this, _playerStateMachine, _playerStateCache)
        );
        _playerStateCache.AddState(
            PlayerStateCache.PlayerState.Move,
            new PlayerMoveState
            (this, _playerStateMachine, _playerStateCache)
        );
        _playerStateCache.AddState(
            PlayerStateCache.PlayerState.Dash,
            new PlayerDashState
            (this, _playerStateMachine, _playerStateCache)
        );
        _playerStateCache.AddState(
            PlayerStateCache.PlayerState.Death,
            new PlayerDeathState
            (this, _playerStateMachine, _playerStateCache)
        );
        _playerStateCache.AddState(
            PlayerStateCache.PlayerState.LightAttack,
            new PlayerLightAttackState
            (this, _playerStateMachine, _playerStateCache)
        );
        _playerStateCache.AddState(
            PlayerStateCache.PlayerState.HeavyAttack,
            new PlayerHeavyAttackState
            (this, _playerStateMachine, _playerStateCache)
        );
    }

    private void Awake()
    {
        _playerStateMachine = new();
        _playerStateCache = new();
    }
    private void Start()
    {
        InitStateCache();
        _playerStateMachine.Init(_playerStateCache.RequestState(PlayerStateCache.PlayerState.Idle));
        _health.ResetHealth();
    }
    private void OnEnable()
    {
        _playerStateMachine.BindInputs(_inputTranslator);
    }
    private void OnDisable()
    {
        _playerStateMachine.UnbindInputs(_inputTranslator);
    }
    private void Update()
    {
        _playerStateMachine.UpdateState();
    }
    private void FixedUpdate()
    {
        _playerStateMachine.FixedUpdateState();
    }

    public void Reset()
    {
        _playerStateMachine.SwitchState(_playerStateCache.RequestState(PlayerStateCache.PlayerState.Idle));
        _health.HealFullHealthAndArmor();
    }

    public void FullReset()
    {
        //Reset is already called on player death, so only reset other values.
        //ISSUE: Should all upgrades and levels be reset on game over?
        _health.ResetHealth();
        _currencySystem.SetGoldCurrency(0);
    }

    public void InvokeAttackEnded()
    {
        if (_attackEndedChannel != null) _attackEndedChannel.Invoke();
    }
}