using Unity.VisualScripting;
using UnityEngine;

public class PlayerIdleState : PlayerActionState
{
    public PlayerIdleState(Player playerContext, PlayerStateMachine playerStateMachine, PlayerStateCache playerStateCache)
        : base(playerContext, playerStateMachine, playerStateCache) { }

    protected override void OnCheckSwitch()
    {
        if (_playerStateMachine.IsMoving)
        {
            _playerStateMachine.SwitchState(_playerStateCache.RequestState(PlayerStateCache.PlayerState.Move));
        }
        else if (_playerStateMachine.UsingPrimaryAction)
        {
            _playerStateMachine.SwitchState(_playerStateCache.RequestState(PlayerStateCache.PlayerState.LightAttack));
        }
        //ISSUE: fix the heat cost so it is not hard coded
        else if (_playerStateMachine.UsingSecondaryAction && _playerContext.HeatGauge.CurrentCharge >= 2)
        {
            _playerStateMachine.SwitchState(_playerStateCache.RequestState(PlayerStateCache.PlayerState.HeavyAttack));
        }
        else if (_playerStateMachine.UsingSpecialAction && _playerContext.HeatGauge.CurrentCharge >= 6)
        {
            if (BeatManager.Instance.BeatInMeasure == 1)
            {
                _playerStateMachine.SwitchState(_playerStateCache.RequestState(PlayerStateCache.PlayerState.SpecialAttack1));
            }
            else if (BeatManager.Instance.BeatInMeasure == 3)
            {
                _playerStateMachine.SwitchState(_playerStateCache.RequestState(PlayerStateCache.PlayerState.SpecialAttack2));
            }
        }
    }

    public override void Enter()
    {
        _playerContext.Animator.SpriteAnimator.Play("Idle");
        //Reset players momentum to prevent gliding
        _playerContext.RB.linearVelocity = Vector3.zero;
    }
}
