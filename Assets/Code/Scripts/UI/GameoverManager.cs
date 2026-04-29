using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameoverManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI livesLeftText;
    [SerializeField] private TextMeshProUGUI deathText;
    [SerializeField] private TextMeshProUGUI deathButtonText;
    [SerializeField] private SceneField sceneTarget;
    [SerializeField] private SceneField sceneEndTarget;

    private void OnEnable()
    {
        livesLeftText.text = $"Lives Left: {GameManager.PlayerLives}";
        if (GameManager.PlayerLives > 0)
        {
            deathText.text = "You Died.";
            deathButtonText.text = "Respawn";
        }  
        else
        {
            deathText.text = "Game Over.";
            deathButtonText.text = "Retry";
        }  
    }
    private void DisableGameoverMenu()
    {
        EventSystem.current.SetSelectedGameObject(null); //Clear selected button
        VolumeManager.Instance.SetDepthOfField(false);
        VolumeManager.Instance.ResetColorSaturation();
        Time.timeScale = 1f;
        MenuManager.Instance.SetMenu("HUD");
    }

    public void Respawn()
    {
        DisableGameoverMenu();
        if (GameManager.Instance.IsGameOver)
        {
            GameManager.Instance.ResetGame();
            GameManager.Instance.SceneManager.TransitionScene(sceneEndTarget, GameManager.Instance);
        }
        else
        {
            GameManager.Instance.SceneManager.TransitionScene(sceneTarget, GameManager.Instance);
        }
    }
}