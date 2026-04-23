using UnityEngine;

public class SceneTriggerManager : MonoBehaviour
{
    [SerializeField] private SceneField sceneTarget;
    [SerializeField] private InputTranslator _inputTranslator;
    [SerializeField] private bool _isLevelExit = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_isLevelExit)
        {
            _inputTranslator.PlayerInputs.Gameplay.Disable();
            MenuManager.Instance.SetMenu("Score");
        }
        else
            GameManager.Instance.SceneManager.TransitionScene(sceneTarget, GameManager.Instance);
    }
}