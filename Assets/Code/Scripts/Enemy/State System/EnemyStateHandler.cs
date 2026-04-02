using System.Collections.Generic;
using EchosCry.Enemy.StateSystem;
using UnityEditor;

//Container for enemy state data that does not change dynamically and is associated to the enemy type
//This container would be the same across enemies of the same type such as bats, slimes, etc
//Stores the starting state of this enemy (spawn state in most circumstances) and the transitions associated with the enemy's states
//State transitions are the conditions in which a state will change from one state to another state
public class EnemyStateHandler 
{
    private EnemyStates _currentState;
    
    private readonly EnemyStates _startState;
    private readonly Dictionary<EnemyStates, StateNode> _states;

    public EnemyStates StartState { get => _startState; }
    public Dictionary<EnemyStates, StateNode> StateNodes { get => _states; }
    public EnemyStates CurrentState { get => _currentState; set => _currentState = value; }

    public EnemyStateHandler(EnemyStateCache cache)
    {
        _startState = EnemyStates.Spawn;
        _currentState = _startState;
        _states = new Dictionary<EnemyStates, StateNode>();
        EnemyStateTransition transition = new(EnemyStates.Idle, x => true);
        AddStateNode(cache, _startState, new EnemyStateTransition[] {transition}, new EnemyStateTransition[] { });
    }
    public EnemyStateHandler(EnemyStateCache cache, EnemyStates startState)
    {
        _startState = startState;
        _currentState = _startState;
        _states = new Dictionary<EnemyStates, StateNode>();
        EnemyStateTransition transition = new(EnemyStates.Idle, x => true);
        AddStateNode(cache, _startState, new EnemyStateTransition[] { transition }, new EnemyStateTransition[] { });
    }
    public EnemyStateHandler(EnemyStates startState, Dictionary<EnemyStates, StateNode> states)
    {
        _startState = startState;
        _currentState = _startState;
        _states = states;
    }

    public bool AddStateNode(EnemyStateCache cache, EnemyStates state, EnemyStateTransition[] defaultArray, EnemyStateTransition[] tickArray)
    {
        if (_states.ContainsKey(state)) return true;
        if (cache == null) return false;
        EnemyState newState = cache.RequestState(state);
        if(newState == null) return false;
        StateNode node = new(newState, defaultArray, tickArray);
        _states.Add(state, node);
        return true;
    }
}
