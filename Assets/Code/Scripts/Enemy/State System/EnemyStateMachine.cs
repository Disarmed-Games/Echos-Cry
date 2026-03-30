
using Ink.Parsed;
using PlasticGui.Gluon.Help.Conditions;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.UI.Selectable;
using EchosCry.Enemy.StateSystem;

public class EnemyStateMachine : AbstractStateMachine<EnemyState> { }



public class NewEnemyStateMachine
{
    public void UpdateStates(Enemy enemy)
    {
        enemy.StateHandler.StateNodes[enemy.StateHandler.CurrentState].State.Update(enemy);
    }
    public void FixedUpdateStates(Enemy enemy)
    {
        enemy.StateHandler.StateNodes[enemy.StateHandler.CurrentState].State.FixedUpdate(enemy);
    }

    public void SwitchStates(EnemyStates state, Enemy enemy)
    {
        NewEnemyState currentState = enemy.StateHandler.StateNodes[enemy.StateHandler.CurrentState].State;
        if (currentState == null) return;

        currentState.Exit(enemy);
        enemy.StateHandler.CurrentState = state;
        currentState = enemy.StateHandler.StateNodes[enemy.StateHandler.CurrentState].State;
        currentState.Enter(enemy);
    }
    public void CheckSwitchStates(Enemy enemy)
    {
        EnemyStateTransition[] transitions = enemy.StateHandler.StateNodes[enemy.StateHandler.CurrentState].DefaultArray;
        foreach (EnemyStateTransition transition in transitions)
        {
            if (transition.MetCondition(enemy))
            {
                SwitchStates(transition.TargetState, enemy);
                break;
            }
        }
    }
    public void CheckTickSwitchStates(Enemy enemy)
    {
        EnemyStateTransition[] transitions = enemy.StateHandler.StateNodes[enemy.StateHandler.CurrentState].TickArray;
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
