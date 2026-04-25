using AudioSystem;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NonSpawnableSingleton<GameManager>
{
    [SerializeField] private GameObject musicManager;

    public static event Action OnGameStartEvent;
    public static event Action OnGameOverEvent;
    public static event Action OnPlayerDeathEvent;

    private static int _maxPlayerLives = 2;
    public static int PlayerLives = _maxPlayerLives;

    private bool _isGameOver = false;
    public bool IsGameOver { get => _isGameOver; }

    private SceneManager _sceneManager;
    public SceneManager SceneManager { get => _sceneManager; }

    protected override void OnAwake()
    {
        _sceneManager = new();
    }

    public void GameStart()
    {
        OnGameStartEvent?.Invoke();
    }

    public void HandlePlayerDeath(Player player)
    {
        PlayerLives--;
        OnPlayerDeathEvent?.Invoke();

        if (PlayerLives <= 0)
        {
            _isGameOver = true;
            //player.FullReset();
            //PlayerLives = _maxPlayerLives;
            //OnGameOverEvent?.Invoke();
        }
    }
    private void OnEnable()
    {
        if (!GameObject.FindAnyObjectByType<BeatManager>())
        {
            GameObject musicManagerRef = UnityEngine.Object.Instantiate(musicManager);
            UnityEngine.Object.DontDestroyOnLoad(musicManagerRef);
        }

        _sceneManager.OnEnable();
    }
    private void OnDisable()
    {
        _sceneManager.OnDisable();
    }
}

public class SceneManager
{
    [SerializeField] soundEffect portalSFX;
    [SerializeField] private InputTranslator _inputTranslator;

    public static event Action OnSceneTransitionEvent;
    private bool sceneTransitioning = false;

    public void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public void TransitionScene(SceneField sceneTarget, MonoBehaviour mb)
    {
        if (sceneTransitioning) return;

        mb.StartCoroutine(HandleSceneTransition());

        IEnumerator HandleSceneTransition()
        {
            //Scene Transition First
            sceneTransitioning = true;
            OnSceneTransitionEvent?.Invoke();
            AsyncOperation newSceneLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneTarget.SceneName, LoadSceneMode.Single);
            newSceneLoad.allowSceneActivation = true;
            while (!newSceneLoad.isDone)
            {
                yield return null;
            }
            sceneTransitioning = false;
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_inputTranslator != null)
        {
            _inputTranslator.PlayerInputs.Gameplay.Pause.Disable();
            _inputTranslator.PlayerInputs.Gameplay.Upgrade.Disable();
        }
        MenuManager.Instance.ScreenFadeIn(_inputTranslator);
        HUDMessage.Instance.UpdateMessage("Loading...", 1f);
        SoundEffectManager.Instance.Builder
            .SetSound(portalSFX)
            .SetSoundPosition(PlayerRef.Transform.position)
            .ValidateAndPlaySound();
    }
}

public class SceneNames
{
    public static readonly string Tutorial = "TutorialScene";
    public static readonly string Town = "TownScene";
    public static readonly string Level1 = "LevelOneScene";
    public static readonly string Level2 = "LevelTwoScene";
    public static readonly string Level3 = "LevelThreeScene";
    public static readonly string Level4 = "LevelFourScene";
    public static readonly string Level5 = "LevelFiveScene";
    public static readonly string MainMenu = "MainMenu";
    public static readonly string Credits = "EndCreditsMenu";
    public static readonly string Cutscene = "IntroScene";
}
