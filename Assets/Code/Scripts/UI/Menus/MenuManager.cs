using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[System.Serializable]
public class StringGameobjectPair
{
    public string key;
    public GameObject value;
}

public class MenuManager : Singleton<MenuManager>
{
    [SerializeField] private InputTranslator _translator;
    [SerializeField] private GameObject screenFadeObject;
    [SerializeField] private List<StringGameobjectPair> menuDictionary;
    [SerializeField] private InputTranslator _inputTranslator;
    [SerializeField] private SettingsManager _settingsManager;
    [SerializeField] private bool hideHUD = false;

    private string _currentMenu;

    protected override void OnAwake()
    {
        SetMenu("HUD");
    }

    private void OnEnable()
    {
        GameManager.OnPlayerDeathEvent += EnableGameoverMenu;
        DialogueManager.onDialogueStarted += HandleDialogueStarted;
        DialogueManager.onDialogueEnded += HandleDialogueEnded;
        _settingsManager.OnMenuBackButton += HandleMenuBack;
        _translator.OnUpgradeEvent += EnableUpgradeMenu;
        _translator.OnPauseEvent += EnablePauseMenu;
        _translator.OnResumeEvent += DisablePauseMenu;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerDeathEvent -= EnableGameoverMenu;
        DialogueManager.onDialogueStarted -= HandleDialogueStarted;
        DialogueManager.onDialogueEnded -= HandleDialogueEnded;
        _settingsManager.OnMenuBackButton -= HandleMenuBack;
        _translator.OnUpgradeEvent -= EnableUpgradeMenu;
        _translator.OnPauseEvent -= EnablePauseMenu;
        _translator.OnResumeEvent -= DisablePauseMenu;
    }

    void HandleDialogueStarted() => SetMenu("Dialogue");
    void HandleDialogueEnded() => SetMenu("HUD");
    void HandleMenuBack() => SetMenu("Pause");

    public void EnableGameoverMenu()
    {
        SetMenu("Gameover");
        VolumeManager.Instance.SetDepthOfField(true);
        VolumeManager.Instance.SetColorSaturationGrey();
        Time.timeScale = 0f;
    }

    private void EnablePauseMenu()
    {
        _inputTranslator.PlayerInputs.PauseMenu.Enable();
        _inputTranslator.PlayerInputs.Gameplay.Disable();

        SetMenu("Pause");
        VolumeManager.Instance.SetDepthOfField(true);
        Time.timeScale = 0f;
    }

    public void EnableUpgradeMenu()
    {
        _inputTranslator.PlayerInputs.PauseMenu.Enable();
        _inputTranslator.PlayerInputs.Gameplay.Disable();

        SetMenu("Upgrade");
        VolumeManager.Instance.SetDepthOfField(true);
        Time.timeScale = 0f;
    }
    public void EnableShopMenu()
    {
        _inputTranslator.PlayerInputs.ShopMenu.Enable();
        _inputTranslator.PlayerInputs.Gameplay.Disable();

        VolumeManager.Instance.SetDepthOfField(true);
        SetMenu("Shop");
    }

    public void DisablePauseMenu()
    {
        _inputTranslator.PlayerInputs.Gameplay.Enable();
        _inputTranslator.PlayerInputs.PauseMenu.Disable();
        
        SetMenu("HUD");
        VolumeManager.Instance.SetDepthOfField(false);
        Time.timeScale = 1f;
    }

    public void SetMenu(string menuName)
    {
        _currentMenu = menuName;

        if (UITip.Instance != null)
            UITip.Instance.StopAllMessage(); //Reset any messages that may be opened still.

        if (hideHUD && menuName == "HUD") return;

        foreach (StringGameobjectPair menu in menuDictionary) 
        { 
            if (menu.key == menuName) 
            {
                menu.value.SetActive(true);
            }
            else
            {
                menu.value.SetActive(false);
            }
        }
    }

    public void ScreenFadeIn(InputTranslator translator)
    {
        StartCoroutine(FadeInCoroutine(translator));
    }

    private IEnumerator FadeInCoroutine(InputTranslator translator)
    {
        float duration = 2f;
        float elapsedTime = 0f;

        CanvasGroup canvasGroup = screenFadeObject.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(1f);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(elapsedTime / duration);
            yield return null;
        }
        canvasGroup.alpha = 0f;

        if (translator != null)
        {
            translator.PlayerInputs.Gameplay.Pause.Enable();
            translator.PlayerInputs.Gameplay.Upgrade.Enable();
        }
    }

    public void BackButton()
    {
        MenuManager.Instance.DisablePauseMenu();
    }
}