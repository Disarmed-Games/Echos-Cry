using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class UpgradeMenuNavigation : MonoBehaviour
{
    private UpgradeSelector[] selectors;
    private Button backButton;
    private readonly List<Button> availableButtons = new List<Button>();
    private bool selectInitial;
    private bool usingController;
    public bool IsUsingController => usingController;
    private readonly Dictionary<TextMeshProUGUI, UpgradeSelector> descriptions = new Dictionary<TextMeshProUGUI, UpgradeSelector>();

    private void OnEnable()
    {
        selectors = GetComponentsInChildren<UpgradeSelector>(true);
        var player = Player.Instance;
        usingController = player != null && player.InputTranslator != null &&
            player.InputTranslator.ActiveControlScheme == InputTranslator.ControllerScheme;
        InputSystem.onEvent += OnInput;
        foreach (var button in GetComponentsInChildren<Button>(true))
            if (button.name == "BackButton") backButton = button;
        selectInitial = true;
        RefreshNavigation();
        RefreshDescriptions();
    }

    private void OnInput(InputEventPtr eventPtr, InputDevice device)
    {
        if (InputTranslator.IsMeaningfulInput(eventPtr, device)) usingController = device is Gamepad;
    }

    private void Update()
    {
        RefreshNavigation();
        var system = EventSystem.current;
        if (system == null) return;
        var selected = system.currentSelectedGameObject;
        var selectedButton = selected != null ? selected.GetComponent<Button>() : null;
        bool unavailableSelection = selectedButton != null && selected.transform.IsChildOf(transform) &&
            (!selectedButton.IsActive() || !selectedButton.IsInteractable());
        if (selectInitial || unavailableSelection)
        {
            var first = availableButtons.Count > 0 ? availableButtons[0] : backButton;
            if (first != null) system.SetSelectedGameObject(first.gameObject);
            selectInitial = false;
        }
        foreach (var selector in selectors)
            selector.SetControllerFocus(usingController && selector.UseButton != null &&
                system.currentSelectedGameObject == selector.UseButton.gameObject);
        RefreshDescriptions();
    }

    public void RefreshDescriptions()
    {
        if (selectors == null) return;
        descriptions.Clear();
        foreach (var selector in selectors)
        {
            var text = selector.DescriptionText;
            if (text == null || !selector.isActiveAndEnabled) continue;
            if (!descriptions.TryGetValue(text, out var owner) ||
                (selector.IsPresentationFocused && !owner.IsPresentationFocused)) descriptions[text] = selector;
        }
        foreach (var entry in descriptions)
            entry.Key.text = entry.Value.Description;
    }

    private void RefreshNavigation()
    {
        availableButtons.Clear();
        foreach (var selector in selectors)
        {
            var button = selector.UseButton;
            if (button == null) continue;
            if (button.IsActive() && button.IsInteractable()) availableButtons.Add(button);
            var navigation = button.navigation;
            navigation.mode = Navigation.Mode.None;
            button.navigation = navigation;
        }
        for (int i = 0; i < availableButtons.Count; i++)
        {
            var navigation = availableButtons[i].navigation;
            navigation.mode = Navigation.Mode.Explicit;
            navigation.selectOnUp = i > 0 ? availableButtons[i - 1] : backButton;
            navigation.selectOnDown = i + 1 < availableButtons.Count ? availableButtons[i + 1] : backButton;
            navigation.selectOnLeft = backButton;
            navigation.selectOnRight = null;
            availableButtons[i].navigation = navigation;
        }
        if (backButton != null)
        {
            var navigation = backButton.navigation;
            navigation.mode = Navigation.Mode.Explicit;
            navigation.selectOnUp = availableButtons.Count > 0 ? availableButtons[availableButtons.Count - 1] : null;
            navigation.selectOnDown = navigation.selectOnRight = availableButtons.Count > 0 ? availableButtons[0] : null;
            navigation.selectOnLeft = null;
            backButton.navigation = navigation;
        }
    }

    private void OnDisable()
    {
        InputSystem.onEvent -= OnInput;
        if (selectors == null) return;
        foreach (var selector in selectors) selector.SetControllerFocus(false);
    }
}
