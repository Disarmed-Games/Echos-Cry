public class PlayerHeavyAttackState : PlayerActionState
{
    public PlayerHeavyAttackState(Player playerContext, PlayerStateMachine playerStateMachine, PlayerStateCache playerStateCache) 
        : base(playerContext, playerStateMachine, playerStateCache) { }

    public override void Enter()
    {
        _playerContext.HeatGauge.UseCharge(2);
        _playerContext.WeaponHolder.SwitchWeapon(0);
        _playerContext.WeaponHolder.SecondaryAction();

        _playerContext.Animator.SpriteAnimator.Play("Attack");
        CameraManager.Instance.ScreenShake(0.8f, 0.5f);
        _playerContext.Movement.MomentumPush();
        _playerContext.Orientation.IsRotating = false;
    }
    public override void Exit()
    {
        _playerContext.InvokeAttackEnded();

        _playerContext.Orientation.IsRotating = true;
        _playerStateMachine.UsingSecondaryAction = false;

        _playerContext.WeaponHolder.ProcessWeaponHits(_playerContext.ComboMeter);
    }
    protected override void OnCheckSwitch()
    {
        if (_playerContext.WeaponHolder.IsActionEnded())
            _playerStateMachine.SwitchState(_playerStateCache.RequestState(PlayerStateCache.PlayerState.Idle));
    }
}
