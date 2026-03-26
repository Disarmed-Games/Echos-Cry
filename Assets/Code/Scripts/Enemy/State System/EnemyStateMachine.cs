
using Ink.Parsed;
using PlasticGui.Gluon.Help.Conditions;
using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class EnemyStateMachine : AbstractStateMachine<EnemyState> { }









public class NewEnemyStateMachine
{
    public void CheckSwitchStates(Enemy[] enemies)
    {
        foreach (Enemy enemy in enemies)
        {
            foreach(EnemyStateTransition transition in enemy.StateInfo.Transitions[enemy.StateInfo.CurrentState])
            {
                if (transition.MetCondition)
                {
                    SwitchStates(transition.TargetState, enemy);
                    break;
                }
            }
        }
    }
    public void UpdateStates(Enemy[] enemies)
    {
        foreach(Enemy enemy in enemies)
        {
            enemy.StateInfo.CurrentState.Update(enemy);
        }
    }
    public void FixedUpdateStates(Enemy[] enemies)
    {
        foreach (Enemy enemy in enemies)
        {
            enemy.StateInfo.CurrentState.FixedUpdate(enemy);
        }
    }

    public void SwitchStates(NewEnemyState state, Enemy enemyContext)
    {
        if (enemyContext.StateInfo.CurrentState == null) return;
        enemyContext.StateInfo.CurrentState.Exit(enemyContext);
        enemyContext.StateInfo.CurrentState = state;
        enemyContext.StateInfo.CurrentState.Enter(enemyContext);
    }
}

public class NewEnemyState
{
    protected bool _isActive;
    public bool IsActive { get => _isActive; set => _isActive = value; }
    public virtual void Update(Enemy enemyContext) { }
    public virtual void FixedUpdate(Enemy enemyContext) { }
    public virtual void Enter(Enemy enemyContext) { }
    public virtual void Exit(Enemy enemyContext) { }
    public virtual void Tick(Enemy enemyContext) { }
}

public abstract class EnemyStateInfo 
{
    protected NewEnemyState _currentState, _startState;
    protected Dictionary<NewEnemyState, EnemyStateTransition[]> _transitions;

    public NewEnemyState CurrentState { get => _currentState; set => _currentState = value; }
    public Dictionary<NewEnemyState, EnemyStateTransition[]> Transitions { get => _transitions; }

    public EnemyStateInfo(NewEnemyState startState)
    {
        _startState = startState;
    }

    public void Init()
    {
        _currentState = _startState;
        OnInit();
    }
    public abstract void OnInit();
}

public class EnemyStateTransition
{
    private readonly NewEnemyState _targetState;
    private readonly Func<bool> _condition;
    
    public NewEnemyState TargetState { get => _targetState; }
    public bool MetCondition => _condition();
    
    public EnemyStateTransition(Func<bool> conditionFunc, NewEnemyState transitionState)
    {
        _condition = conditionFunc;
        _targetState = transitionState;
    }
}