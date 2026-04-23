using System.Collections;
using UnityEngine;

public class PlayerDashState : PlayerActionState
{
    public PlayerDashState(
        Player playerContext,
        PlayerStateMachine playerStateMachine, 
        PlayerStateCache playerStateCache) 
        : base(playerContext, playerStateMachine, playerStateCache)
    {}

    public override void Enter()
    {
        _playerStateMachine.CurrentStateEnum = PlayerState.Dash;

        // Handle Dash Attack
        //--------------------
        DashAttack dashAttack = _playerContext.Abilities.TryGetDashAttack();
        if(dashAttack != null)
        {
            dashAttack.HitQuality = TempoConductor.Instance.CurrentHitQuality;
            dashAttack.gameObject.SetActive(true);
        }
        //--------------------

        _playerContext.Health.IsInvincible = true;
        _playerContext.Animator.SetIsTrailEmit(true);
        _playerContext.Animator.SpriteAnimator.Play(_playerContext.Animator.hashedDashAnim);
        _playerContext.PlayerParticles.StartDashParticles();
        EchosCry.Sound.PlaySFX(_playerContext.SFXConfig.DashSFX, _playerContext.transform, 0);
        _playerContext.Movement.Dash();
        _playerStateMachine.CanDash = false;
        _playerContext.StartCoroutine(DashDuration());
    }
    public override void Exit()
    {
        // Handle Dash Attack
        //--------------------
        DashAttack dashAttack = _playerContext.Abilities.TryGetDashAttack();
        if (dashAttack != null)
        {
            dashAttack.gameObject.SetActive(false);
            _playerContext.InvokeAttackEnded();
        }
        //--------------------

        _playerContext.Health.IsInvincible = false;
        _playerContext.Animator.SetIsTrailEmit(false);
        _playerStateMachine.IsDashing = false;
    }

    IEnumerator DashDuration()
    {
        yield return new WaitForSeconds(_playerContext.Movement.PlayerMovementConfig.DashDuration);
        _playerStateMachine.SwitchState(_playerStateCache.RequestState(PlayerStateCache.PlayerState.Move));
        _playerStateMachine.CanDash = true;
    }

    //IEnumerator Dash_Cooldown()
    //{
    //    yield return new WaitForSeconds(_playerContext.Movement.PlayerMovementConfig.Dash_Cooldown);
    //    _playerStateMachine.CanDash = true;
    //}
}
