using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum pauseOptions
{
    CONTINUE, SETTINGS, MENU, MENU_YES, MENU_NO, QUIT
}
public class PauseMenu : MonoBehaviour
{
    private pauseOptions currentPauseOption = pauseOptions.CONTINUE;
    
    [SerializeField] private InputTranslator translator;
    [SerializeField] private GameObject optionButtonsContainer;

    private void ChooseOption()
    {
        switch(currentPauseOption)
        {
            case pauseOptions.CONTINUE:
                MenuManager.Instance.SetMenu("HUD");
                MenuManager.Instance.DisablePauseMenu();
                translator.PlayerInputs.Gameplay.Enable();
                translator.PlayerInputs.PauseMenu.Disable();
                break;
            case pauseOptions.SETTINGS:
                MenuManager.Instance.SetMenu("Settings");
                break;
            case pauseOptions.MENU:
                optionButtonsContainer.SetActive(!optionButtonsContainer.activeSelf);
                break;
            case pauseOptions.MENU_YES:
                MenuManager.Instance.DisablePauseMenu();
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
                break;
            case pauseOptions.MENU_NO:
                optionButtonsContainer.SetActive(false);
                break;
            case pauseOptions.QUIT:
                Application.Quit();
                break;
            default:
                break;
        }
    }

    public void SelectPauseOption(int option)
    {
        currentPauseOption = (pauseOptions)option;
        ChooseOption();
    }
}
