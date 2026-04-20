using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class PersistenceInitializer
{
    private static bool loaded = false;
    private static Object _persistenceRef;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        SceneManager.sceneLoaded += LoadObjects;
    }

    static void LoadObjects(Scene scene, LoadSceneMode mode)
    {
        var currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == "EndCreditsMenu" || currentScene.name == "MainMenu" || currentScene.name == "BeginningCutscene")
        {
            if (_persistenceRef != null)
            {
                Object.Destroy(_persistenceRef);
                loaded = false;
            }
        }

        if (loaded) return;

        if (currentScene.name != "MainMenu" && currentScene.name != "EndCreditsMenu" && currentScene.name != "BeginningCutscene")
        {
            //Debug.Log("Loaded by the Persist Object from the PersistenceInitializer script");
            _persistenceRef = Object.Instantiate(Resources.Load("PERSISTOBJECTS"));
            Object.DontDestroyOnLoad(_persistenceRef);
            loaded = true;
        }
    }
}
