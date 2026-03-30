using System.Collections.Generic;
using UnityEngine;
using EchosCry.Enemy.StateSystem;

public class EnemyStateCache 
{
    public enum EnemyStates
    {
        UNASSIGNED = 0,
        //Bat Enemy
        BatSpawn, BatStagger, BatDeath, BatCharge, BatAttack, BatIdle, BatChase,
        //Range Enemy
        RangeSpawn, RangeStagger, RangeDeath, RangeCharge, RangeAttack, RangeIdle, RangeRoam,
        //Walker Enemy
        WalkerSpawn, WalkerStagger, WalkerDeath, WalkerJump, WalkerAttack, WalkerIdle, WalkerChase,
        //Slime Enemy
        SlimeSpawn, SlimeStagger, SlimeDeath, SlimeCharge, SlimeAttack, SlimeIdle, SlimeChase,
        //Bomb Enemy
    }

    private readonly Dictionary<EnemyStates, EnemyState> _stateCache;
    private EnemyState _startStart;
    public EnemyState StartState { get => _startStart; set => _startStart = value; }

    public EnemyStateCache()
    {
        _stateCache = new();
    }

    public EnemyState RequestState(EnemyStates requestedState)
    {
        if (!_stateCache.ContainsKey(requestedState)) return null;
        else return _stateCache[requestedState];
    }
    public void AddState(EnemyStates statesEnum, EnemyState state)
    {
        _stateCache.Add(statesEnum, state);
    }

    public void Enable()
    {
        EnableStates();
    }
    public void Disable()
    {
        DisableStates();
    }
    private void EnableStates()
    {
        foreach (var state in _stateCache.Values)
            state.Enable();
    }
    private void DisableStates()
    {
        foreach (var state in _stateCache.Values)
            state.Disable();
    }
}











public class NewEnemyStateCache
{
    private readonly Dictionary<EnemyStates, NewEnemyState> _stateCache;

    public NewEnemyStateCache()
    {
        _stateCache = new()
        {
            //Init all states
            { EnemyStates.Death, new DeathEnemyState() },
            { EnemyStates.Spawn, new SpawnEnemyState() },
            { EnemyStates.Idle, new IdleEnemyState() },
            { EnemyStates.Stagger, new StaggerEnemyState() },
            { EnemyStates.Pursue, new PursueEnemyState() },
            {EnemyStates.Attack, new AttackEnemyState() },
            {EnemyStates.Charge, new ChargeEnemyState() },
            {EnemyStates.Cooldown, new CooldownEnemyState() },
        };
    }
    
    public NewEnemyState RequestState(EnemyStates requestedState)
    {
        if (!_stateCache.ContainsKey(requestedState)) return null;
        else return _stateCache[requestedState];
    }
}