using EchosCry;

public class PlayerHeavyAttackState : PlayerActionState
{
    public PlayerHeavyAttackState(Player playerContext, PlayerStateMachine playerStateMachine, PlayerStateCache playerStateCache) 
        : base(playerContext, playerStateMachine, playerStateCache) { }

    public override void Enter()
    {
        _playerStateMachine.CurrentStateEnum = PlayerState.Heavy;
        _playerContext.HeatGauge.UseCharge(2);

        _playerContext.WeaponHolder.SwitchToPrimary();
        _playerContext.WeaponHolder.ActivateCurrentWeapon();
        _playerContext.WeaponHolder.SecondaryAction(_playerContext.Stats);

        _playerContext.Animator.SpriteAnimator.Play("Attack");
        CameraManager.Instance.ScreenShake(0.8f, 0.5f);
        _playerContext.Orientation.IsRotating = false;
    }
    public override void Exit()
    {
        _playerContext.InvokeAttackEnded();

        _playerContext.Orientation.IsRotating = true;
        _playerStateMachine.UsingSecondaryAction = false;

        _playerContext.WeaponHolder.ProcessWeaponHits(_playerContext.ComboMeter);
        _playerContext.WeaponHolder.DeactivateCurrentWeapon();
    }
    public override void FixedUpdate()
    {
        float movementMult = _playerContext.Stats.MovementMultiplier * 0.75f;
        _playerContext.Movement.Move(_playerStateMachine.Locomotion, movementMult);
    }
    protected override void OnCheckSwitch()
    {
        if (_playerContext.WeaponHolder.IsActionEnded)
            _playerStateMachine.SwitchState(_playerStateCache.RequestState(PlayerStateCache.PlayerState.Idle));
    }
}
