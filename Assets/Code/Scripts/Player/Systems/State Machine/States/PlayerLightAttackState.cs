using EchosCry;
using UnityEngine;

public class PlayerLightAttackState : PlayerActionState
{
    public PlayerLightAttackState(Player playerContext, PlayerStateMachine playerStateMachine, PlayerStateCache playerStateCache) 
        : base(playerContext, playerStateMachine, playerStateCache) { }

    public override void Enter()
    {
        _playerStateMachine.CurrentStateEnum = PlayerState.Light;

        _playerContext.WeaponHolder.SwitchToPrimary();
        _playerContext.WeaponHolder.ActivateCurrentWeapon();
        _playerContext.WeaponHolder.PrimaryAction(_playerContext.Stats);

        _playerContext.Animator.SpriteAnimator.Play(_playerContext.Animator.hashedAttackAnim);

        _playerContext.Orientation.IsRotating = false;

        CameraManager.Instance.ScreenShake(0.3f, 0.15f);
    }
    public override void Exit()
    {
        _playerContext.InvokeAttackEnded();

        _playerContext.Orientation.IsRotating = true;
        _playerStateMachine.UsingPrimaryAction = false;

        _playerContext.WeaponHolder.ProcessWeaponHits(_playerContext.ComboMeter);
        _playerContext.WeaponHolder.DeactivateCurrentWeapon();

        if (_playerContext.WeaponHolder.DidWeaponHit) _playerContext.HeatGauge.IncreaseCharge(1);
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
