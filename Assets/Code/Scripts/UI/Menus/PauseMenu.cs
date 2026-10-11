using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum pauseOptions
{
    CONTINUE, SETTINGS, MENU, MENU_YES, MENU_NO, QUIT
}
public class PauseMenu : MonoBehaviour
{
    private pauseOptions currentPauseOption = pauseOptions.CONTINUE;
    
    [SerializeField] private InputTranslator translator;
    [SerializeField] private GameObject optionButtonsContainer;
    private Button mainMenuButton;

    private void OnEnable()
    {
        var buttons = GetComponentsInChildren<Button>(true);
        var order = new string[] { "ContinueButton", "SettingsButton", "MainMenuButton", "ExitButton" };
        var menuButtons = new System.Collections.Generic.List<Button>();
        foreach (string buttonName in order)
            foreach (var button in buttons)
                if (button.name == buttonName)
                {
                    menuButtons.Add(button);
                    if (buttonName == "MainMenuButton") mainMenuButton = button;
                    break;
                }
        for (int i = 0; i < menuButtons.Count; i++)
        {
            var navigation = menuButtons[i].navigation;
            navigation.mode = Navigation.Mode.Explicit;
            navigation.selectOnUp = menuButtons[(i + menuButtons.Count - 1) % menuButtons.Count];
            navigation.selectOnDown = menuButtons[(i + 1) % menuButtons.Count];
            navigation.selectOnLeft = navigation.selectOnRight = null;
            menuButtons[i].navigation = navigation;
        }
    }

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
                if (optionButtonsContainer.activeSelf)
                {
                    var choices = optionButtonsContainer.GetComponentsInChildren<Button>();
                    for (int i = 0; i < choices.Length; i++)
                    {
                        var navigation = choices[i].navigation;
                        navigation.mode = Navigation.Mode.Explicit;
                        var other = choices[(i + 1) % choices.Length];
                        navigation.selectOnUp = navigation.selectOnDown = other;
                        navigation.selectOnLeft = navigation.selectOnRight = other;
                        choices[i].navigation = navigation;
                    }
                    if (choices.Length > 0 && EventSystem.current != null)
                        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
                }
                else if (mainMenuButton != null && EventSystem.current != null)
                    EventSystem.current.SetSelectedGameObject(mainMenuButton.gameObject);
                break;
            case pauseOptions.MENU_YES:
                MenuManager.Instance.DisablePauseMenu();
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
                break;
            case pauseOptions.MENU_NO:
                optionButtonsContainer.SetActive(false);
                if (mainMenuButton != null && EventSystem.current != null)
                    EventSystem.current.SetSelectedGameObject(mainMenuButton.gameObject);
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
