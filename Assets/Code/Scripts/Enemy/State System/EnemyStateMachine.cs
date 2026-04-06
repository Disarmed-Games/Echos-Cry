
using EchosCry.Enemy.StateSystem;

public class EnemyStateMachine
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
        EnemyState currentState = enemy.StateHandler.StateNodes[enemy.StateHandler.CurrentState].State;
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
