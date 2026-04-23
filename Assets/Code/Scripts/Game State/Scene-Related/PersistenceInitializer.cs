using UnityEngine;
using UnityEngine.SceneManagement;

public static class PersistenceInitializer
{
    private static bool loaded = false;
    private static GameObject _persistenceRef;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= LoadObjects;
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += LoadObjects;
    }

    static void LoadObjects(Scene scene, LoadSceneMode mode)
    {
        var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

        if (currentScene.name == "EndCreditsMenu" || currentScene.name == "MainMenu")
        {
            if (_persistenceRef != null)
            {
                Object.Destroy(_persistenceRef);
                _persistenceRef = null;
                loaded = false;
            }
            return;
        }

        if (loaded) return;

        var prefab = Resources.Load<GameObject>("PERSISTOBJECTS");
        _persistenceRef = Object.Instantiate(prefab);
        Object.DontDestroyOnLoad(_persistenceRef);
        loaded = true;
    }
}