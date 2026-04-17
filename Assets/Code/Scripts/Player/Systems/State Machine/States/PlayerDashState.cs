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
        // Handle Dash Attack
        //--------------------
        if (_playerContext.Stats.DashAttackEnabled)
            _playerContext.WeaponHolder.DashAction();
        //--------------------

        //_playerContext.ComboMeter.ResetComboMultiplier();
        _playerContext.Animator.SetIsTrailEmit(true);
        _playerContext.Animator.SpriteAnimator.Play("Dash");
        _playerContext.PlayerParticles.StartDashParticles();
        EchosCry.Sound.Execute(_playerContext.SFXConfig.DashSFX, _playerContext.transform, 0);
        _playerContext.Movement.Dash();
        _playerStateMachine.CanDash = false;
        _playerContext.StartCoroutine(DashDuration());
    }
    public override void Exit()
    {
        // Handle Dash Attack
        //--------------------
        if (_playerContext.Stats.DashAttackEnabled)
        {
            _playerContext.WeaponHolder.ResetPreviousWeapon();
            int hitCount = _playerContext.WeaponHolder.CurrentlyEquippedWeapon.HitColliders.Count;
            for (int i = 0; i < hitCount; i++)
            {
                TempoConductor.HitQuality hitQuality = _playerContext.WeaponHolder.CurrentlyEquippedWeapon.HitColliders[i].hit;
                _playerContext.ComboMeter.AddToComboMeter(hitQuality);
            }
            _playerContext.InvokeAttackEnded();
        }
        //--------------------

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
