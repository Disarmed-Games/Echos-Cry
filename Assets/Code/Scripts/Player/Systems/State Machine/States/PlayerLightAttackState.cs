public class PlayerLightAttackState : PlayerActionState
{
    public PlayerLightAttackState(Player playerContext, PlayerStateMachine playerStateMachine, PlayerStateCache playerStateCache) 
        : base(playerContext, playerStateMachine, playerStateCache) { }

    public override void Enter()
    {
        if (BeatManager.Instance.BeatInMeasure <= 2)
        {
            _playerContext.WeaponHolder.SwitchWeapon(0); //Clarinet
            _playerContext.WeaponHolder.PrimaryAction();
        }
        else
        {
            _playerContext.WeaponHolder.SwitchWeapon(1); //Drum
            _playerContext.WeaponHolder.PrimaryAction();
        }

        _playerContext.Animator.SpriteAnimator.Play("Attack");
        _playerContext.Movement.MomentumPush();
        _playerContext.Orientation.IsRotating = false;
    }
    public override void Exit()
    {
        _playerContext.InvokeAttackEnded();

        _playerContext.Orientation.IsRotating = true;
        _playerStateMachine.IsAttacking = false;

        _playerContext.WeaponHolder.ProcessWeaponHits(_playerContext.ComboMeter);

        if (_playerContext.WeaponHolder.DidWeaponHit) _playerContext.HeatGauge.IncreaseCharge(1);
    }

    protected override void OnCheckSwitch()
    {
        if (_playerContext.WeaponHolder.IsActionEnded())
            _playerStateMachine.SwitchState(_playerStateCache.RequestState(PlayerStateCache.PlayerState.Idle));
    }
}
