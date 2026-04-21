using EchosCry;

public class PlayerSpecialAttack2State : PlayerActionState
{
    public PlayerSpecialAttack2State(Player playerContext, PlayerStateMachine playerStateMachine, PlayerStateCache playerStateCache)
        : base(playerContext, playerStateMachine, playerStateCache) { }

    public override void Enter()
    {
        _playerStateMachine.CurrentStateEnum = PlayerState.Special2;
        _playerContext.HeatGauge.UseCharge(6);

        _playerContext.WeaponHolder.SwitchWeapon(1);
        _playerContext.WeaponHolder.SecondaryAction();

        _playerContext.Animator.SpriteAnimator.Play("Attack");
        CameraManager.Instance.ScreenShake(0.8f, 0.5f);
        //_playerContext.Movement.MomentumPush();
        _playerContext.Orientation.IsRotating = false;
    }
    public override void Exit()
    {
        _playerContext.InvokeAttackEnded();

        _playerContext.Orientation.IsRotating = true;
        _playerStateMachine.UsingSpecialAction = false;

        _playerContext.WeaponHolder.ProcessWeaponHits(_playerContext.ComboMeter);
        _playerContext.WeaponHolder.SwitchWeapon(0);
    }
    protected override void OnCheckSwitch()
    {
        if (_playerContext.WeaponHolder.IsActionEnded())
            _playerStateMachine.SwitchState(_playerStateCache.RequestState(PlayerStateCache.PlayerState.Idle));
    }
}
