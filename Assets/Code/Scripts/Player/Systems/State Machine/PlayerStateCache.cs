using System.Collections.Generic;
using UnityEngine;

public class PlayerStateCache
{
    public enum PlayerState
    {
        None = 0,
        Idle, Move, Dash, Death, LightAttack, HeavyAttack
    }

    private readonly Dictionary<PlayerState, PlayerActionState> _stateCache;

    public PlayerStateCache() => _stateCache = new();

    public void AddState(PlayerState state, PlayerActionState actionState)
    {
        _stateCache.Add(state, actionState);
    }

    public PlayerActionState RequestState(PlayerState state)
    {
        if (_stateCache.ContainsKey(state)) return _stateCache[state];
        else return null;
    }

}
