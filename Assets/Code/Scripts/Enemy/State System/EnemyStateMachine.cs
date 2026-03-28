
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
    public void UpdateStates(Enemy enemy)
    {
        enemy.StateData.CurrentState.Update(enemy);
    }
    public void FixedUpdateStates(Enemy enemy)
    {
        enemy.StateData.CurrentState.FixedUpdate(enemy);
    }

    public void SwitchStates(NewEnemyState state, Enemy enemy)
    {
        if (enemy.StateData.CurrentState == null) return;
        enemy.StateData.CurrentState.Exit(enemy);
        enemy.StateData.CurrentState = state;
        enemy.StateData.CurrentState.Enter(enemy);
    }
    public void CheckSwitchStates(Enemy enemy)
    {
        EnemyStateData stateData = enemy.StateData;
        EnemyStateTransition[] transitions = stateData.TransitionDictionary[stateData.CurrentState].DefaultArray;
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
        EnemyStateData stateInfo = enemy.StateData;
        EnemyStateTransition[] transitions = stateInfo.TransitionDictionary[stateInfo.CurrentState].TickArray;
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
