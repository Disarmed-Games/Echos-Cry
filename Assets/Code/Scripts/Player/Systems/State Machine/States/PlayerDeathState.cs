using UnityEngine;

public class PlayerDeathState : PlayerActionState
{
    public PlayerDeathState(Player playerContext, PlayerStateMachine playerStateMachine, PlayerStateCache playerStateCache)
        : base(playerContext, playerStateMachine, playerStateCache) { }

    public override void Enter()
    {
        _playerStateMachine.CurrentStateEnum = PlayerState.Death;
        _playerContext.DeathReset();
        GameManager.Instance.HandlePlayerDeath(_playerContext);
    }
}
