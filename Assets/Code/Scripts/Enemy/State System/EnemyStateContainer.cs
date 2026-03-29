using System.Collections.Generic;

//Container for enemy state data that does not change dynamically and is associated to the enemy type
//This container would be the same across enemies of the same type such as bats, slimes, etc
//Stores the starting state of this enemy (spawn state in most circumstances) and the transitions associated with the enemy's states
//State transitions are the conditions in which a state will change from one state to another state
public readonly struct EnemyStateContainer 
{
    private readonly NewEnemyState _startState;
    private readonly Dictionary<NewEnemyState, EnemyStateTransitionsContainer> _stateTransitions;

    public readonly NewEnemyState StartState => _startState;
    public readonly Dictionary<NewEnemyState, EnemyStateTransitionsContainer> TransitionDictionary => _stateTransitions;

    public EnemyStateContainer(NewEnemyState startState, Dictionary<NewEnemyState, EnemyStateTransitionsContainer> stateTransitions)
    {
        _startState = startState;
        _stateTransitions = stateTransitions;
    }
}

public class EnemyStateData
{
    public NewEnemyState CurrentState;
    public bool IsStaggered;

}
