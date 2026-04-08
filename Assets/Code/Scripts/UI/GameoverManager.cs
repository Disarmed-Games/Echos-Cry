using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameoverManager : MonoBehaviour
{
    [SerializeField] SceneTriggerManager sceneTriggerManager;
    [SerializeField] private TextMeshProUGUI livesLeftText;
    [SerializeField] private TextMeshProUGUI deathText;

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
        Debug.Log("========Respawning player");
        sceneTriggerManager.StartTransition();
        DisableGameoverMenu();
    }
}
