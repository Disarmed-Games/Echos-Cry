using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BindingMenuExitTests
{
    [Test]
    public void BindingRowsAndScrollbarHaveReturnPathsToTabsAndBack()
    {
        var root = new GameObject("Settings", typeof(RectTransform), typeof(Canvas), typeof(SettingsManager));
        var events = new GameObject("Events", typeof(EventSystem));
        var system = events.GetComponent<EventSystem>();
        EventSystem.current = system;
        try
        {
            var tab = CreateControl<Button>("ControlTab", root.transform, new Vector2(0, 100));
            var back = CreateControl<Button>("BackButton", root.transform, new Vector2(200, 100));
            var viewportObject = new GameObject("Viewport", typeof(RectTransform), typeof(ScrollRect));
            var viewport = (RectTransform)viewportObject.transform;
            viewport.SetParent(root.transform, false);
            viewport.sizeDelta = new Vector2(200, 100);
            var scroll = viewportObject.GetComponent<ScrollRect>();
            scroll.viewport = viewport;
            var contentObject = new GameObject("Content", typeof(RectTransform));
            var content = (RectTransform)contentObject.transform;
            content.SetParent(viewport, false);
            content.sizeDelta = new Vector2(200, 200);
            scroll.content = content;
            var first = CreateControl<Button>("First", content, new Vector2(0, 25));
            var last = CreateControl<Button>("Last", content, new Vector2(0, -25));
            var hidden = CreateControl<Button>("Hidden", content, new Vector2(0, -75));
            var scrollbar = CreateControl<Scrollbar>("Scrollbar", root.transform, new Vector2(120, 0));
            scrollbar.direction = Scrollbar.Direction.BottomToTop;
            scroll.verticalScrollbar = scrollbar;
            var navigation = contentObject.AddComponent<VisibleBindingNavigation>();
            navigation.Setup(scroll);
            system.SetSelectedGameObject(last.gameObject);
            navigation.SendMessage("Update");

            Move(system, last, MoveDirection.Left);
            Assert.That(system.currentSelectedGameObject, Is.EqualTo(tab.gameObject));
            Move(system, tab, MoveDirection.Down);
            Assert.That(system.currentSelectedGameObject, Is.EqualTo(first.gameObject));
            Move(system, first, MoveDirection.Right);
            Assert.That(system.currentSelectedGameObject, Is.EqualTo(scrollbar.gameObject));
            Move(system, scrollbar, MoveDirection.Right);
            Assert.That(system.currentSelectedGameObject, Is.EqualTo(back.gameObject));
            Move(system, back, MoveDirection.Up);
            Assert.That(system.currentSelectedGameObject, Is.EqualTo(tab.gameObject));
            system.SetSelectedGameObject(scrollbar.gameObject);
            Move(system, scrollbar, MoveDirection.Left);
            Assert.That(system.currentSelectedGameObject, Is.EqualTo(last.gameObject));
            Assert.That(last.navigation.selectOnDown, Is.Null);
            Assert.That(hidden.navigation.mode, Is.EqualTo(Navigation.Mode.None));

            system.SetSelectedGameObject(scrollbar.gameObject);
            scrollbar.value = 0.5f;
            Move(system, scrollbar, MoveDirection.Up);
            Assert.That(system.currentSelectedGameObject, Is.EqualTo(scrollbar.gameObject));
            Assert.That(scrollbar.value, Is.GreaterThan(0.5f), "Vertical movement must still scroll.");
        }
        finally
        {
            Object.DestroyImmediate(events);
            Object.DestroyImmediate(root);
        }
    }

    private static T CreateControl<T>(string name, Transform parent, Vector2 position) where T : Selectable
    {
        var obj = new GameObject(name, typeof(RectTransform));
        var rect = (RectTransform)obj.transform;
        rect.SetParent(parent, false);
        rect.sizeDelta = new Vector2(100, 30);
        rect.anchoredPosition = position;
        return obj.AddComponent<T>();
    }

    private static void Move(EventSystem system, Selectable control, MoveDirection direction)
    {
        ExecuteEvents.Execute(control.gameObject, new AxisEventData(system) { moveDir = direction }, ExecuteEvents.moveHandler);
    }
}
