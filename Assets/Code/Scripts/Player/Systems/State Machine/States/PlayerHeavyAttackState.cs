public class PlayerHeavyAttackState : PlayerActionState
{
    public PlayerHeavyAttackState(Player playerContext, PlayerStateMachine playerStateMachine, PlayerStateCache playerStateCache) 
        : base(playerContext, playerStateMachine, playerStateCache) { }

    public override void Enter()
    {
        _playerContext.HeatGauge.UseCharge(2);

        //if (BeatManager.Instance.BeatInMeasure <= 2)
        //{
        //    _playerContext.WeaponHolder.SwitchWeapon(0); //Clarinet
        //    _playerContext.WeaponHolder.SecondaryAction();
        //}
        //else
        //{
        //    _playerContext.WeaponHolder.SwitchWeapon(1); //Drum
        //    _playerContext.WeaponHolder.SecondaryAction();
        //}

        _playerContext.WeaponHolder.SecondaryAction();

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
    }
    protected override void OnCheckSwitch()
    {
        if (_playerContext.WeaponHolder.IsActionEnded())
            _playerStateMachine.SwitchState(_playerStateCache.RequestState(PlayerStateCache.PlayerState.Idle));
    }
}
