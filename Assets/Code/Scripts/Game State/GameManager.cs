using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static event Action OnGameStartEvent;
    public static event Action OnGameOverEvent;
    public static event Action OnPlayerDeathEvent;

    private static int _maxPlayerLives = 2;
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
    private readonly EnemyStateMachine _stateMachine;
    private readonly EnemyStateCache _stateCache;
    public EnemyStateMachine StateMachine { get => _stateMachine; }
    public EnemyStateCache StateCache { get => _stateCache; }

    public EnemyManager()
    {
        _stateCache = new();
        _stateMachine = new EnemyStateMachine();
    }
}
