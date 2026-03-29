using System.Collections.Generic;

//Container for enemy state data that does not change dynamically and is associated to the enemy type
//This container would be the same across enemies of the same type such as bats, slimes, etc
//Stores the starting state of this enemy (spawn state in most circumstances) and the transitions associated with the enemy's states
//State transitions are the conditions in which a state will change from one state to another state
public readonly struct EnemyStateContainer 
{
    private readonly NewEnemyState _startState;
    private readonly Dictionary<NewEnemyStateCache.EnemyStates, StateNode> _states;

    public NewEnemyState StartState { get => _startState; }
    public Dictionary<NewEnemyStateCache.EnemyStates, StateNode> StateNodes { get => _states; }

    public EnemyStateContainer(NewEnemyState startState, Dictionary<NewEnemyStateCache.EnemyStates, StateNode> states)
    {
        _startState = startState;
        _states = states;
    }
}

public class EnemyStateData
{
    public NewEnemyStateCache.EnemyStates CurrentState;
    public bool IsStaggered;

}
