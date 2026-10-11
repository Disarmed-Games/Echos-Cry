using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Update navigation before EventSystem processes move/submit, so hidden rows cannot be reached.
[DefaultExecutionOrder(-100)]
public class VisibleBindingNavigation : MonoBehaviour
{
    private ScrollRect scroll;
    private readonly List<Button> visibleButtons = new List<Button>();
    private readonly Dictionary<Selectable, Navigation> originalNavigation = new Dictionary<Selectable, Navigation>();
    private Selectable controlsTab;
    private Selectable backButton;
    private Button lastFocusedBinding;

    public void Setup(ScrollRect scrollRect)
    {
        scroll = scrollRect;
        FindMenuLinks();
    }

    private void FindMenuLinks()
    {
        var settings = scroll.GetComponentInParent<SettingsManager>();
        var canvas = scroll.GetComponentInParent<Canvas>();
        Transform scope = settings != null ? settings.transform : canvas != null ? canvas.transform : null;
        if (scope == null) return;
        foreach (var selectable in scope.GetComponentsInChildren<Selectable>(true))
        {
            if (selectable.name == "ControlTab") controlsTab = selectable;
            if (selectable.name == "BackButton") backButton = selectable;
        }
    }

    private void Update()
    {
        if (scroll == null) return;
        if (controlsTab == null || backButton == null) FindMenuLinks();
        var viewport = scroll.viewport != null ? scroll.viewport : (RectTransform)scroll.transform;
        var system = EventSystem.current;
        var selected = system != null ? system.currentSelectedGameObject : null;

        // D-pad/left stick move focus; right stick scrolls the list using unscaled time.
        if (Gamepad.current != null && selected != null &&
            (selected.transform.IsChildOf(transform) ||
             (scroll.verticalScrollbar != null && selected == scroll.verticalScrollbar.gameObject)))
        {
            float amount = Gamepad.current.rightStick.ReadValue().y;
            if (Mathf.Abs(amount) > 0.25f)
            {
                scroll.StopMovement();
                scroll.verticalNormalizedPosition = Mathf.Clamp01(scroll.verticalNormalizedPosition + amount * Time.unscaledDeltaTime * 0.7f);
            }
        }

        visibleButtons.Clear();
        foreach (var button in GetComponentsInChildren<Button>())
        {
            if (!originalNavigation.ContainsKey(button)) originalNavigation.Add(button, button.navigation);
            if (button.IsInteractable() && IsFullyVisible(viewport, (RectTransform)button.transform))
                visibleButtons.Add(button);
            var navigation = button.navigation;
            navigation.mode = Navigation.Mode.None;
            navigation.selectOnUp = navigation.selectOnDown = null;
            navigation.selectOnLeft = navigation.selectOnRight = null;
            button.navigation = navigation;
        }

        for (int i = 0; i < visibleButtons.Count; i++)
        {
            var navigation = visibleButtons[i].navigation;
            navigation.mode = Navigation.Mode.Explicit;
            navigation.selectOnUp = i > 0 ? visibleButtons[i - 1] : null;
            navigation.selectOnDown = i + 1 < visibleButtons.Count ? visibleButtons[i + 1] : null;
            navigation.selectOnLeft = Available(controlsTab) ? controlsTab : backButton;
            navigation.selectOnRight = scroll.verticalScrollbar != null ? scroll.verticalScrollbar : backButton;
            visibleButtons[i].navigation = navigation;
        }

        if (selected != null && visibleButtons.Contains(selected.GetComponent<Button>()))
            lastFocusedBinding = selected.GetComponent<Button>();
        ConfigureMenuLinks();

        // Scrolling can move the selected row out of view. Keep focus on the nearest visible row.
        if (selected == null || !selected.transform.IsChildOf(transform)) return;
        var selectedButton = selected.GetComponent<Button>();
        if (selectedButton == null || visibleButtons.Contains(selectedButton)) return;
        Button closest = null;
        float closestDistance = float.PositiveInfinity;
        foreach (var button in visibleButtons)
        {
            float distance = (button.transform.position - selected.transform.position).sqrMagnitude;
            if (distance >= closestDistance) continue;
            closestDistance = distance;
            closest = button;
        }
        system.SetSelectedGameObject(closest != null ? closest.gameObject :
            scroll.verticalScrollbar != null ? scroll.verticalScrollbar.gameObject : null);
    }

    private static bool Available(Selectable control) => control != null && control.IsActive() && control.IsInteractable();

    private Navigation OriginalLinks(Selectable control)
    {
        if (!originalNavigation.ContainsKey(control)) originalNavigation.Add(control, control.navigation);
        var original = originalNavigation[control];
        control.navigation = original;
        var links = original;
        if (original.mode != Navigation.Mode.Explicit)
        {
            links.selectOnUp = control.FindSelectableOnUp();
            links.selectOnDown = control.FindSelectableOnDown();
            links.selectOnLeft = control.FindSelectableOnLeft();
            links.selectOnRight = control.FindSelectableOnRight();
        }
        links.mode = Navigation.Mode.Explicit;
        return links;
    }

    private void ConfigureMenuLinks()
    {
        var first = visibleButtons.Count > 0 ? visibleButtons[0] : null;
        var returnRow = lastFocusedBinding != null && visibleButtons.Contains(lastFocusedBinding) ? lastFocusedBinding : first;
        if (Available(scroll.verticalScrollbar))
        {
            var navigation = OriginalLinks(scroll.verticalScrollbar);
            // With no up/down links, Unity's Scrollbar still uses those directions to scroll.
            navigation.selectOnUp = navigation.selectOnDown = null;
            navigation.selectOnLeft = returnRow != null ? returnRow : controlsTab;
            navigation.selectOnRight = Available(backButton) ? backButton : controlsTab;
            scroll.verticalScrollbar.navigation = navigation;
        }
        if (Available(controlsTab))
        {
            var navigation = OriginalLinks(controlsTab);
            navigation.selectOnDown = first != null ? first : scroll.verticalScrollbar;
            if (Available(backButton)) navigation.selectOnUp = backButton;
            controlsTab.navigation = navigation;
        }
        if (Available(backButton))
        {
            var navigation = OriginalLinks(backButton);
            navigation.selectOnLeft = Available(scroll.verticalScrollbar) ? scroll.verticalScrollbar : first;
            navigation.selectOnUp = navigation.selectOnDown = controlsTab;
            backButton.navigation = navigation;
        }
    }

    public bool IsVisible(Selectable control)
    {
        if (scroll == null) return true;
        return IsFullyVisible(scroll.viewport != null ? scroll.viewport : (RectTransform)scroll.transform,
            (RectTransform)control.transform);
    }

    public static Rect BoundsInViewport(RectTransform viewport, RectTransform item)
    {
        var corners = new Vector3[4];
        item.GetWorldCorners(corners);
        Vector2 minimum = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        Vector2 maximum = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
        foreach (var corner in corners)
        {
            Vector2 point = viewport.InverseTransformPoint(corner);
            minimum = Vector2.Min(minimum, point);
            maximum = Vector2.Max(maximum, point);
        }
        return Rect.MinMaxRect(minimum.x, minimum.y, maximum.x, maximum.y);
    }

    public static bool IsFullyVisible(RectTransform viewport, RectTransform item)
    {
        Rect bounds = BoundsInViewport(viewport, item);
        Rect visible = viewport.rect;
        const float tolerance = 0.5f;
        return bounds.xMin >= visible.xMin - tolerance && bounds.xMax <= visible.xMax + tolerance &&
            bounds.yMin >= visible.yMin - tolerance && bounds.yMax <= visible.yMax + tolerance;
    }

    public static void RevealDropdownOption(ScrollRect scrollRect, RectTransform item)
    {
        if (scrollRect.content == null) return;
        var viewport = scrollRect.viewport != null ? scrollRect.viewport : (RectTransform)scrollRect.transform;
        Rect bounds = BoundsInViewport(viewport, item);
        float offset = bounds.yMax > viewport.rect.yMax ? viewport.rect.yMax - bounds.yMax :
            bounds.yMin < viewport.rect.yMin ? viewport.rect.yMin - bounds.yMin : 0f;
        if (Mathf.Abs(offset) < 0.01f) return;
        scrollRect.StopMovement();
        Vector3 shift = scrollRect.content.parent.InverseTransformVector(viewport.TransformVector(new Vector3(0f, offset, 0f)));
        scrollRect.content.anchoredPosition += new Vector2(shift.x, shift.y);
    }

    private void OnDisable()
    {
        foreach (var entry in originalNavigation)
            if (entry.Key != null) entry.Key.navigation = entry.Value;
    }
}
