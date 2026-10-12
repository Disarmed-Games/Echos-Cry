using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControlRemapManager : MonoBehaviour
{
    [SerializeField] private GameObject _controlsContainer;
    [SerializeField] private GameObject _remapOptionPrefab;
    [SerializeField] private InputTranslator _inputTranslator;
    private readonly List<GameObject> keyboardRows = new List<GameObject>();
    private readonly List<GameObject> controllerRows = new List<GameObject>();
    private Button keyboardButton;
    private Button controllerButton;
    private ScrollRect scroll;

    private void Start()
    {
        foreach (var actionMap in _inputTranslator.PlayerInputs.asset.actionMaps)
        {
            foreach (var action in actionMap.actions)
            {
                for (int i = 0; i < action.bindings.Count; i++)
                {
                    var binding = action.bindings[i];

                    if (binding.isComposite)
                        continue;

                    bool controller = InputBinding.MaskByGroup(InputTranslator.ControllerScheme).Matches(binding);
                    bool keyboardMouse = InputBinding.MaskByGroup(InputTranslator.KeyboardMouseScheme).Matches(binding);
                    if (!controller && !keyboardMouse)
                        continue;

                    string actionName = action.name;
                    if (binding.isPartOfComposite)
                    {
                        actionName += $" ({binding.name})";
                    }

                    GameObject instance = Instantiate(_remapOptionPrefab, _controlsContainer.transform);

                    instance.GetComponent<RemapOptionHandler>().SetupAction(
                        actionName,
                        action.GetBindingDisplayString(i),
                        action,
                        i
                    );
                    if (keyboardMouse) keyboardRows.Add(instance);
                    if (controller) controllerRows.Add(instance);
                }
            }
        }
        scroll = _controlsContainer.GetComponentInParent<ScrollRect>();
        if (scroll != null)
        {
            CreateDeviceButtons();
            var navigation = _controlsContainer.GetComponent<VisibleBindingNavigation>();
            if (navigation == null) navigation = _controlsContainer.AddComponent<VisibleBindingNavigation>();
            navigation.Setup(scroll, keyboardButton, controllerButton);
        }
        ShowBindings(false);
    }

    private void CreateDeviceButtons()
    {
        // Reserve space inside the existing list bounds, keeping the bottom edge in place.
        const float headerHeight = 80f;
        var listRect = (RectTransform)scroll.transform;
        var header = new GameObject("Binding Device", typeof(RectTransform));
        header.layer = gameObject.layer;
        var headerRect = (RectTransform)header.transform;
        headerRect.SetParent(listRect.parent, false);
        headerRect.anchorMin = listRect.anchorMin;
        headerRect.anchorMax = listRect.anchorMax;
        headerRect.pivot = new Vector2(listRect.pivot.x, 1f);
        headerRect.sizeDelta = new Vector2(listRect.sizeDelta.x, headerHeight);
        headerRect.anchoredPosition = listRect.anchoredPosition +
            new Vector2(0f, listRect.rect.height * (1f - listRect.pivot.y));
        listRect.sizeDelta -= new Vector2(0f, headerHeight);
        listRect.anchoredPosition -= new Vector2(0f, headerHeight * (1f - listRect.pivot.y));

        keyboardButton = CreateDeviceButton(headerRect, "Keyboard", 0f, 0.5f);
        controllerButton = CreateDeviceButton(headerRect, "Controller", 0.5f, 1f);
        keyboardButton.onClick.AddListener(() => ShowBindings(false));
        controllerButton.onClick.AddListener(() => ShowBindings(true));
    }

    private Button CreateDeviceButton(RectTransform parent, string label, float left, float right)
    {
        var obj = new GameObject(label + "BindingsButton", typeof(RectTransform), typeof(Image), typeof(Button));
        obj.layer = gameObject.layer;
        var rect = (RectTransform)obj.transform;
        rect.SetParent(parent, false);
        rect.anchorMin = new Vector2(left, 0f);
        rect.anchorMax = new Vector2(right, 1f);
        rect.offsetMin = new Vector2(8f, 12f);
        rect.offsetMax = new Vector2(-8f, -4f);
        var image = obj.GetComponent<Image>();
        image.color = new Color(0.12f, 0.12f, 0.12f, 1f);
        var button = obj.GetComponent<Button>();
        button.targetGraphic = image;

        var textObject = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.layer = gameObject.layer;
        var textRect = (RectTransform)textObject.transform;
        textRect.SetParent(rect, false);
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = textRect.offsetMax = Vector2.zero;
        var text = textObject.GetComponent<TextMeshProUGUI>();
        var template = _remapOptionPrefab.GetComponentInChildren<TextMeshProUGUI>(true);
        if (template != null) text.font = template.font;
        text.text = label;
        text.fontSize = 36f;
        text.alignment = TextAlignmentOptions.Center;
        text.raycastTarget = false;
        return button;
    }

    public void ShowBindings(bool controller)
    {
        var selectedRows = controller ? controllerRows : keyboardRows;
        foreach (var row in keyboardRows) row.SetActive(selectedRows.Contains(row));
        foreach (var row in controllerRows) row.SetActive(selectedRows.Contains(row));
        SetDeviceLabel(keyboardButton, !controller);
        SetDeviceLabel(controllerButton, controller);
        var system = EventSystem.current;
        if (system != null && system.currentSelectedGameObject != null &&
            system.currentSelectedGameObject.transform.IsChildOf(_controlsContainer.transform) &&
            !system.currentSelectedGameObject.activeInHierarchy)
            system.SetSelectedGameObject(controller ? controllerButton?.gameObject : keyboardButton?.gameObject);
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_controlsContainer.transform);
        if (scroll != null)
        {
            scroll.StopMovement();
            scroll.verticalNormalizedPosition = 1f;
        }
    }

    private static void SetDeviceLabel(Button button, bool selected)
    {
        if (button == null) return;
        var label = button.GetComponentInChildren<TextMeshProUGUI>();
        label.fontStyle = selected ? FontStyles.Bold | FontStyles.Underline : FontStyles.Normal;
        label.color = selected ? new Color(1f, 0.86f, 0.25f) : Color.white;
    }
}
