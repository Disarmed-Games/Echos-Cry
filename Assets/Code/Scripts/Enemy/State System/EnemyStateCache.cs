using System.Collections.Generic;

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
    public enum EnemyStates
    {
        Unassigned = 0,
        Spawn, Death, Idle, Stagger,
        ////Bat Enemy
        //BatSpawn, BatStagger, BatDeath, BatCharge, BatAttack, BatIdle, BatChase,
        ////Range Enemy
        //RangeSpawn, RangeStagger, RangeDeath, RangeCharge, RangeAttack, RangeIdle, RangeRoam,
        ////Walker Enemy
        //WalkerSpawn, WalkerStagger, WalkerDeath, WalkerJump, WalkerAttack, WalkerIdle, WalkerChase,
        ////Slime Enemy
        //SlimeSpawn, SlimeStagger, SlimeDeath, SlimeCharge, SlimeAttack, SlimeIdle, SlimeChase,
        ////Bomb Enemy
    }
    private Dictionary<EnemyStates, NewEnemyState> _stateCache;


    public NewEnemyStateCache()
    {
        _stateCache = new();

        //Init all states
    }
    

    public NewEnemyState RequestState(NewEnemyStateCache.EnemyStates requestedState)
    {
        if (!_stateCache.ContainsKey(requestedState)) return null;
        else return _stateCache[requestedState];
    }
}