using System;
using EchosCry.Enemy.StateSystem;

//The information related to transitioning from one state to another
//Stores the target state, aka the state that will be transitioned to if a condition if met
//Stores a bool function that takes in an Enemy which is the condition that must be met for transition
public readonly struct EnemyStateTransition
{
    private readonly EnemyStates _targetState;
    private readonly Func<Enemy, bool> _condition;
    
    public readonly EnemyStates TargetState { get => _targetState; }
    public readonly bool MetCondition(Enemy enemy) => _condition(enemy);
    
    public EnemyStateTransition(EnemyStates transitionState, Func<Enemy, bool> conditionFunc)
    {
        _targetState = transitionState;
        _condition = conditionFunc;
    }
}