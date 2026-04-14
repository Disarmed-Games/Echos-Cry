using System.Collections;
using UnityEngine;

public class PlayerMoveState : PlayerActionState
{
    Coroutine currentCoroutine;
    public PlayerMoveState(Player playerContext,
        PlayerStateMachine playerStateMachine, 
        PlayerStateCache playerStateCache)
        : base(playerContext, playerStateMachine, playerStateCache)
    {}

    protected override void OnCheckSwitch()
    {
        if (!_playerStateMachine.IsMoving)
        {
            _playerStateMachine.SwitchState(_playerStateCache.RequestState(PlayerStateCache.PlayerState.Idle));
        }
        else if (_playerStateMachine.UsingPrimaryAction)
        {
            _playerStateMachine.SwitchState(_playerStateCache.RequestState(PlayerStateCache.PlayerState.LightAttack));
        }
        else if (_playerStateMachine.UsingSecondaryAction && _playerContext.HeatGauge.CurrentCharge >= 2)
        {
            _playerStateMachine.SwitchState(_playerStateCache.RequestState(PlayerStateCache.PlayerState.HeavyAttack));
        }
        else if(_playerStateMachine.UsingSpecialAction && _playerContext.HeatGauge.CurrentCharge >= 6)
        {
            if(BeatManager.Instance.BeatInMeasure == 1)
            {
                _playerStateMachine.SwitchState(_playerStateCache.RequestState(PlayerStateCache.PlayerState.SpecialAttack1));
            }
            else if(BeatManager.Instance.BeatInMeasure == 3)
            {
                _playerStateMachine.SwitchState(_playerStateCache.RequestState(PlayerStateCache.PlayerState.SpecialAttack2));
            }
        }
        else if (_playerStateMachine.CanDash && _playerContext.Movement.HasDash && _playerStateMachine.IsDashing)
        {
            if (_playerContext.Movement.PlayerMovementConfig.IsDashToBeat)
            {
                if (TempoConductor.Instance.IsOnBeat()) _playerStateMachine.SwitchState(_playerStateCache.RequestState(PlayerStateCache.PlayerState.Dash));
            }
            else _playerStateMachine.SwitchState(_playerStateCache.RequestState(PlayerStateCache.PlayerState.Dash));
        }
    }

    public override void Enter()
    {
        _playerContext.Animator.SpriteAnimator.Play("Run");
        _playerContext.Animator.StartMovementParticles();
        _playerContext.SFX.Execute(_playerContext.SFXConfig.FootstepSFX, _playerContext.transform, 0);
        currentCoroutine = _playerContext.StartCoroutine(RepeatSoundFootstep(_playerContext.SFXConfig.FootstepSFX.soundClips[0].length));
    }
    public override void Exit()
    {
        _playerContext.Animator.EndMovementParticles();
        if (currentCoroutine != null) _playerContext.StopCoroutine(currentCoroutine);
    }
    public override void Update()
    {
        _playerContext.Animator.UpdateMainSpriteDirection(_playerStateMachine.Locomotion);
    }
    public override void FixedUpdate()
    {
        _playerContext.Movement.Move(_playerStateMachine.Locomotion);
    }
    private IEnumerator RepeatSoundFootstep(float clipLength)
    {
        yield return new WaitForSeconds(clipLength);
        _playerContext.SFX.Execute(_playerContext.SFXConfig.FootstepSFX, _playerContext.transform, 0);
        currentCoroutine = _playerContext.StartCoroutine(RepeatSoundFootstep(_playerContext.SFXConfig.FootstepSFX.soundClips[0].length));
    }
}
