public class PlayerLightAttackState : PlayerActionState
{
    public PlayerLightAttackState(Player playerContext, PlayerStateMachine playerStateMachine, PlayerStateCache playerStateCache) 
        : base(playerContext, playerStateMachine, playerStateCache) { }

    public override void Enter()
    {
        _playerContext.WeaponHolder.SwitchWeapon(0);
        _playerContext.WeaponHolder.PrimaryAction();

        _playerContext.Animator.SpriteAnimator.Play("Attack");
        //_playerContext.Movement.MomentumPush();
        _playerContext.Orientation.IsRotating = false;
    }
    public override void Exit()
    {
        _playerContext.InvokeAttackEnded();

        _playerContext.Orientation.IsRotating = true;
        _playerStateMachine.UsingPrimaryAction = false;

        _playerContext.WeaponHolder.ProcessWeaponHits(_playerContext.ComboMeter);

        if (_playerContext.WeaponHolder.DidWeaponHit) _playerContext.HeatGauge.IncreaseCharge(1);
    }

    protected override void OnCheckSwitch()
    {
        if (_playerContext.WeaponHolder.IsActionEnded())
            _playerStateMachine.SwitchState(_playerStateCache.RequestState(PlayerStateCache.PlayerState.Idle));
    }
}
