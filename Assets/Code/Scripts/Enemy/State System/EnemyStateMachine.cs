
using Ink.Parsed;
using PlasticGui.Gluon.Help.Conditions;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.UI.Selectable;

public class EnemyStateMachine : AbstractStateMachine<EnemyState> { }



public class NewEnemyStateMachine
{
    private NewEnemyStateCache _stateCache;

    public NewEnemyStateMachine(NewEnemyStateCache stateCache)
    {
        _stateCache = stateCache;
    }

    public void UpdateStates(Enemy enemy)
    {
        enemy.StateContainer.StateNodes[enemy.StateData.CurrentState].State.Update(enemy);
    }
    public void FixedUpdateStates(Enemy enemy)
    {
        enemy.StateContainer.StateNodes[enemy.StateData.CurrentState].State.FixedUpdate(enemy);
    }

    public void SwitchStates(NewEnemyStateCache.EnemyStates state, Enemy enemy)
    {
        NewEnemyState currentState = enemy.StateContainer.StateNodes[enemy.StateData.CurrentState].State;
        if (currentState == null) return;

        currentState.Exit(enemy);
        enemy.StateData.CurrentState = state;
        currentState = enemy.StateContainer.StateNodes[enemy.StateData.CurrentState].State;
        currentState.Enter(enemy);
    }
    public void CheckSwitchStates(Enemy enemy)
    {
        EnemyStateTransition[] transitions = enemy.StateContainer.StateNodes[enemy.StateData.CurrentState].DefaultArray;
        foreach (EnemyStateTransition transition in transitions)
        {
            if (transition.MetCondition(enemy))
            {
                SwitchStates(transition.TargetState, enemy);
                break;
            }
        }
    }
    public void TickCheckSwitchStates(Enemy enemy)
    {
        EnemyStateTransition[] transitions = enemy.StateContainer.StateNodes[enemy.StateData.CurrentState].TickArray;
        foreach (EnemyStateTransition transition in transitions)
        {
            if (transition.MetCondition(enemy))
            {
                SwitchStates(transition.TargetState, enemy);
                break;
            }
        }
    }
}
