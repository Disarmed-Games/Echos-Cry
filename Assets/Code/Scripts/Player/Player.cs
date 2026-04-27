using EchosCry;
using Ink.Parsed;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NonSpawnableSingleton<Player>
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
    [SerializeField] private SFXConfig _sfxConfig;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private SpamPrevention _spamPrevention;
    [SerializeField] private HeatGauge _heatGauge;
    [SerializeField] private PlayerParticles _particles;
    [SerializeField] private AbilityManager abilities;
    [SerializeField] private DashHandler _dashHandler;
    private Stats _stats;

    [SerializeField] private GameObject _mainCanvas;
    private PlayerUI _ui;

    [Header("Event Channel (Broadcaster)")]
    [SerializeField] EventChannel _attackEndedChannel;

    private PlayerStateMachine _playerStateMachine;
    private PlayerStateCache _playerStateCache;

    private List<EffectData> _activeEffects = new();

    public PlayerHealth Health { get => _health; }
    public PlayerComboMeter ComboMeter { get => _comboMeter; }
    public PlayerAnimator Animator { get => _animator; }
    public SFXConfig SFXConfig { get => _sfxConfig; }
    public PlayerMovement Movement { get => _movement; }
    public WeaponHolder WeaponHolder { get => _weaponHolder; }
    public PlayerOrientation Orientation { get => _orientation; }
    public PlayerCurrencySystem CurrencySystem { get => _currencySystem; }
    public PlayerXP XP { get => _xp; }
    public InputTranslator InputTranslator { get => _inputTranslator; }
    public Rigidbody RB { get => _rb; }
    public SpamPrevention SpamPrevention { get => _spamPrevention; }
    public HeatGauge HeatGauge { get => _heatGauge; }
    public PlayerParticles PlayerParticles { get => _particles; }
    public PlayerUI UI { get => _ui; }
    public AbilityManager Abilities { get => abilities; }
    public Stats Stats { get => _stats; }
    public DashHandler DashHandler { get => _dashHandler; }
    public List<EffectData> ActiveEffects { get => _activeEffects; }

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
        _playerStateCache.AddState(
            PlayerStateCache.PlayerState.SpecialAttack1,
            new PlayerSpecialAttack1State
            (this, _playerStateMachine, _playerStateCache)
        );
        _playerStateCache.AddState(
            PlayerStateCache.PlayerState.SpecialAttack2,
            new PlayerSpecialAttack2State
            (this, _playerStateMachine, _playerStateCache)
        );
    }

    private void Awake()
    {
        _playerStateMachine = new();
        _playerStateCache = new();

        _stats = new Stats();

        _ui = Instantiate(_mainCanvas, transform).GetComponent<PlayerUI>();
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
        _heatGauge.UseCharge(99);
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
        if (!Physics.Raycast(transform.position + new Vector3(0,1,0), Vector3.down, 1.25f))
        {
            RB.AddForce(Vector3.down * 100, ForceMode.Impulse);
        }

    }

    public void DeathReset()
    {
        _playerStateMachine.SwitchState(_playerStateCache.RequestState(PlayerStateCache.PlayerState.Idle));
        _currencySystem.SetGoldCurrency((int)Mathf.Floor(_currencySystem.GetGoldCurrency() * 0.5f));
        _health.ResetHealth();
    }

    public void FullReset()
    {
        //Reset is already called on player death, so only reset other values.
        //ISSUE: Should all upgrades and levels be reset on game over?
        _health.ResetHealth();
        _xp.ResetXP();
        _heatGauge.UseCharge(99);
        _currencySystem.SetGoldCurrency(0);
        _stats.Reset();
        _weaponHolder.ResetEffects();
        _dashHandler.ClearEffects();
    }

    public void InvokeAttackEnded()
    {
        if (_attackEndedChannel != null) _attackEndedChannel.Invoke();
    }
}