using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameoverManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI livesLeftText;
    [SerializeField] private TextMeshProUGUI deathText;
    [SerializeField] private SceneField sceneTarget;

    private void OnEnable()
    {
        livesLeftText.text = $"Lives Left: {GameManager.PlayerLives}";
        if (GameManager.PlayerLives > 0)
            deathText.text = "You Died.";
        else
            deathText.text = "Game Over.";
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
        GameManager.Instance.SceneManager.TransitionScene(sceneTarget, GameManager.Instance);
        DisableGameoverMenu();
    }
}
