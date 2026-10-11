using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

// Install on scene EventSystems without requiring changes to every scene/prefab.
[RequireComponent(typeof(EventSystem))]
public class ControllerUIFocus : MonoBehaviour
{
    private bool usingController;
    private RectTransform focusFrame;
    private Canvas focusCanvas;
    private readonly Vector3[] targetCorners = new Vector3[4];
    private Graphic focusedGraphic;
    private Color originalGraphicColor;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= Attach;
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += Attach;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AttachInitialScene()
    {
        Attach(default, LoadSceneMode.Single);
    }

    private static void Attach(Scene scene, LoadSceneMode mode)
    {
        foreach (var system in Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None))
            if (system.GetComponent<ControllerUIFocus>() == null)
                system.gameObject.AddComponent<ControllerUIFocus>();
    }

    private void OnEnable()
    {
        InputSystem.onEvent += OnInput;
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDisable()
    {
        InputSystem.onEvent -= OnInput;
        InputSystem.onDeviceChange -= OnDeviceChange;
        HideFocus();
    }

    private void OnDestroy()
    {
        if (focusCanvas != null) Destroy(focusCanvas.gameObject);
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is Gamepad && (change == InputDeviceChange.Disconnected ||
            change == InputDeviceChange.Removed || change == InputDeviceChange.Disabled))
        {
            usingController = false;
            HideFocus();
        }
    }

    private void OnInput(InputEventPtr eventPtr, InputDevice device)
    {
        if (InputTranslator.IsMeaningfulInput(eventPtr, device)) usingController = device is Gamepad;
    }

    private void LateUpdate()
    {
        if (!usingController)
        {
            HideFocus();
            return;
        }
        var system = GetComponent<EventSystem>();
        if (system != EventSystem.current)
        {
            HideFocus();
            return;
        }
        var selected = system.currentSelectedGameObject;
        var current = selected != null ? selected.GetComponent<Selectable>() : null;
        if (CanFocus(current))
        {
            var dropdown = current.GetComponentInParent<TMP_Dropdown>();
            if (dropdown != null && dropdown.IsExpanded && current is Toggle)
            {
                var scroll = current.GetComponentInParent<ScrollRect>();
                if (scroll != null) VisibleBindingNavigation.RevealDropdownOption(scroll, (RectTransform)current.transform);
            }
            ShowFocus(current);
            return;
        }

        HideFocus();

        foreach (var selectable in Selectable.allSelectablesArray)
        {
            if (!CanFocus(selectable) ||
                selectable.navigation.mode == Navigation.Mode.None) continue;
            system.SetSelectedGameObject(selectable.gameObject);
            ShowFocus(selectable);
            break;
        }
    }

    private static bool CanFocus(Selectable control)
    {
        if (control == null || !control.IsActive() || !control.IsInteractable()) return false;
        if (control.GetComponent<SlotScript>() != null) return false;
        var canvas = control.GetComponentInParent<Canvas>();
        if (canvas == null || !canvas.isActiveAndEnabled) return false;
        var bindingsList = control.GetComponentInParent<VisibleBindingNavigation>();
        if (bindingsList != null && !bindingsList.IsVisible(control)) return false;
        var dropdown = control.GetComponentInParent<TMP_Dropdown>();
        bool openingDropdownOption = dropdown != null && dropdown.IsExpanded && control is Toggle;
        // Menus can keep their GameObjects active while hiding via CanvasGroup alpha.
        foreach (var group in control.GetComponentsInParent<CanvasGroup>())
        {
            // TMP selects its option before fading in the popup. Preserve that selection.
            if (group.alpha <= 0.01f && !(openingDropdownOption && group.GetComponent<ScrollRect>() != null)) return false;
            if (group.ignoreParentGroups) break;
        }
        return true;
    }

    private void HideFocus()
    {
        if (focusFrame != null) focusFrame.gameObject.SetActive(false);
        RestoreGraphicColor();
    }

    private void RestoreGraphicColor()
    {
        if (focusedGraphic != null) focusedGraphic.color = originalGraphicColor;
        focusedGraphic = null;
    }

    private void ShowFocus(Selectable control)
    {
        var target = control.transform as RectTransform;
        if (target == null) return;
        var graphic = control.targetGraphic != null ? control.targetGraphic : control.GetComponent<Graphic>();
        if (focusedGraphic != graphic)
        {
            RestoreGraphicColor();
            focusedGraphic = graphic;
            if (focusedGraphic != null) originalGraphicColor = focusedGraphic.color;
        }
        // Tint the existing UI graphic too: slider handles, toggle backgrounds, and buttons.
        // This uses the control's established rendering path even if its border is obscured.
        if (focusedGraphic != null)
            focusedGraphic.color = new Color(1f, 0.86f, 0.25f, Mathf.Max(0.85f, originalGraphicColor.a));
        if (focusCanvas == null)
        {
            // This root overlay cannot inherit a menu's material, mask, scale, or camera.
            var overlay = new GameObject("Controller Focus Overlay", typeof(RectTransform),
                typeof(Canvas), typeof(CanvasGroup));
            overlay.transform.SetParent(transform, false);
            focusCanvas = overlay.GetComponent<Canvas>();
            focusCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            focusCanvas.sortingOrder = short.MaxValue;
            overlay.GetComponent<CanvasGroup>().blocksRaycasts = false;
            overlay.GetComponent<CanvasGroup>().interactable = false;

            var frame = new GameObject("Controller Focus", typeof(RectTransform),
                typeof(CanvasRenderer), typeof(ControllerFocusOutline));
            focusFrame = frame.GetComponent<RectTransform>();
            focusFrame.SetParent(overlay.transform, false);
            focusFrame.anchorMin = focusFrame.anchorMax = new Vector2(0.5f, 0.5f);
            focusFrame.pivot = new Vector2(0.5f, 0.5f);
            Canvas.ForceUpdateCanvases();
        }

        var sourceCanvas = target.GetComponentInParent<Canvas>().rootCanvas;
        focusCanvas.targetDisplay = sourceCanvas.targetDisplay;
        var sourceCamera = sourceCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null :
            sourceCanvas.worldCamera != null ? sourceCanvas.worldCamera : Camera.main;
        var overlayRect = (RectTransform)focusCanvas.transform;
        target.GetWorldCorners(targetCorners);
        Vector2 minimum = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        Vector2 maximum = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
        foreach (var corner in targetCorners)
        {
            Vector2 screen = RectTransformUtility.WorldToScreenPoint(sourceCamera, corner);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(overlayRect, screen, null, out Vector2 local);
            minimum = Vector2.Min(minimum, local);
            maximum = Vector2.Max(maximum, local);
        }
        focusFrame.anchoredPosition = (minimum + maximum) * 0.5f;
        focusFrame.sizeDelta = maximum - minimum + new Vector2(14f, 14f);
        focusFrame.gameObject.SetActive(true);
    }

    [ContextMenu("Log Controller Focus State")]
    private void LogFocusState()
    {
        var system = GetComponent<EventSystem>();
        var selected = system.currentSelectedGameObject;
        Debug.Log($"Controller focus: controller={usingController}, currentEventSystem={system == EventSystem.current}, " +
            $"selected={(selected != null ? selected.name : "none")}, " +
            $"frameVisible={(focusFrame != null && focusFrame.gameObject.activeInHierarchy)}, " +
            $"frameBounds={(focusFrame != null ? focusFrame.rect.ToString() : "none")}", this);
    }
}
