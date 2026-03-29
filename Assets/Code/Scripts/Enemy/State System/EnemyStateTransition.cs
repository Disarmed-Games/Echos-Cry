using System;

//The information related to transitioning from one state to another
//Stores the target state, aka the state that will be transitioned to if a condition if met
//Stores a bool function that takes in an Enemy which is the condition that must be met for transition
public readonly struct EnemyStateTransition
{
    private readonly NewEnemyState _targetState;
    private readonly Func<bool, Enemy> _condition;
    
    public readonly NewEnemyState TargetState { get => _targetState; }
    public readonly bool MetCondition(Enemy enemy) => _condition(enemy);
    
    public EnemyStateTransition(NewEnemyState transitionState, Func<bool, Enemy> conditionFunc)
    {
        _targetState = transitionState;
        _condition = conditionFunc;
    }
}

//Container for transitions associated to a state
//Default array represents the default transitions of a state aka transitions that need their conditions checked every frame
//Tick array represents transitions of a state that can be checked based on a tick timer rather than every frame (ex. checking player distance)
public readonly struct EnemyStateTransitionsContainer
{
    private readonly EnemyStateTransition[] _defaultArray;
    private readonly EnemyStateTransition[] _tickArray;

    public readonly EnemyStateTransition[] DefaultArray { get => _defaultArray; }
    public readonly EnemyStateTransition[] TickArray { get => _tickArray; }

    public EnemyStateTransitionsContainer(EnemyStateTransition[] defaultList, EnemyStateTransition[] tickList)
    {
        _defaultArray = defaultList;
        _tickArray = tickList;
    }
}