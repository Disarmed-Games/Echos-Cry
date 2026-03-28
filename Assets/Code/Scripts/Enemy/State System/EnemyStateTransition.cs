using System;

public class EnemyStateTransition
{
    private readonly NewEnemyState _targetState;
    private readonly Func<bool, Enemy> _condition;
    
    public NewEnemyState TargetState { get => _targetState; }
    public bool MetCondition(Enemy enemy) => _condition(enemy);
    
    public EnemyStateTransition(Func<bool, Enemy> conditionFunc, NewEnemyState transitionState)
    {
        _condition = conditionFunc;
        _targetState = transitionState;
    }
}

public class EnemyTransitionsContainer
{
    readonly EnemyStateTransition[] _defaultArray;
    readonly EnemyStateTransition[] _tickArray;
    readonly float _tickTime;

    public EnemyStateTransition[] DefaultArray { get => _defaultArray; }
    public EnemyStateTransition[] TickArray { get => _tickArray; }
    public float TickTime => _tickTime;

    public EnemyTransitionsContainer(float tickTime, EnemyStateTransition[] defaultList, EnemyStateTransition[] tickList)
    {
        _defaultArray = defaultList;
        _tickTime = tickTime;
        _tickArray = tickList;
    }
}