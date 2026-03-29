using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static event Action OnGameStartEvent;
    public static event Action OnGameOverEvent;
    public static event Action OnPlayerDeathEvent;

    private static int _maxPlayerLives = 3;
    public static int PlayerLives = _maxPlayerLives;

    private EnemyManager _enemyManager;
    public EnemyManager EnemyManager { get => _enemyManager; }

    protected override void OnAwake()
    {
        _enemyManager = new();
    }

    public static void GameStart()
    {
        OnGameStartEvent?.Invoke();
    }
    //Player parameter may not be used idk yet
    public static void HandlePlayerDeath(Player player)
    {
        PlayerLives--;
        OnPlayerDeathEvent?.Invoke();

        if (PlayerLives <= 0)
        {
            player.FullReset();
            PlayerLives = _maxPlayerLives;
            OnGameOverEvent?.Invoke();
        }
    }
}

public class EnemyManager
{
    private readonly NewEnemyStateMachine _stateMachine;
    private readonly NewEnemyStateCache _stateCache;
    public NewEnemyStateMachine StateMachine { get => _stateMachine; }
    public NewEnemyStateCache StateCache { get => _stateCache; }

    public EnemyManager()
    {
        _stateMachine = new NewEnemyStateMachine();
        _stateCache = new();
    }
}
