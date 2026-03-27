using System.Collections.Generic;

public abstract class EnemyStateData 
{
    protected NewEnemyState _currentState, _startState;
    protected Dictionary<NewEnemyState, EnemyTransitionsContainer> _transitionDictionary;

    public NewEnemyState CurrentState { get => _currentState; set => _currentState = value; }
    public NewEnemyState StartState { get => _startState; }
    public Dictionary<NewEnemyState, EnemyTransitionsContainer> TransitionDictionary { get => _transitionDictionary; }

    public EnemyStateData(NewEnemyState startState)
    {
        _startState = startState;
        _transitionDictionary = new();
    }

    public void Init()
    {
        _currentState = _startState;
        OnInit();
    }
    protected abstract void OnInit();
}
