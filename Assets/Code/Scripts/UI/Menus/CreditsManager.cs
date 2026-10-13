using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    // The gameplay manager is destroyed when the credits scene loads.
    public static bool ShowGameOverOnNextLoad { get; set; }

    [SerializeField] private string _returnSceneName;
    [SerializeField] private TextMeshProUGUI _headingText;

    private void Awake()
    {
        bool showGameOver = ShowGameOverOnNextLoad;
        ShowGameOverOnNextLoad = false;

        if (showGameOver)
        {
            _headingText.text = "GAME OVER\n" + _headingText.text;
            _headingText.enableAutoSizing = true;
            _headingText.fontSizeMax = _headingText.fontSize;
        }
    }

    public void BackButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(_returnSceneName);
    }
}
