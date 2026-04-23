using UnityEngine;

public class ScoreSceneManager : MonoBehaviour
{
    [SerializeField] private SceneField sceneTarget;

    public void ContinueButton()
    {
        GameManager.Instance.SceneManager.TransitionScene(sceneTarget, GameManager.Instance);
        MenuManager.Instance.DisablePauseMenu();
    }
}
