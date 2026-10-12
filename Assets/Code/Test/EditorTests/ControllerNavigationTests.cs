using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class ControllerNavigationTests : InputTestFixture
{
    private GameObject root;
    private GameObject eventObject;
    private EventSystem system;

    public override void Setup()
    {
        base.Setup();
        root = new GameObject("Test Canvas", typeof(RectTransform), typeof(Canvas));
        root.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        eventObject = new GameObject("Test EventSystem", typeof(EventSystem));
        system = eventObject.GetComponent<EventSystem>();
        EventSystem.current = system;
    }

    public override void TearDown()
    {
        Object.DestroyImmediate(eventObject);
        Object.DestroyImmediate(root);
        base.TearDown();
    }

    private RectTransform Rect(string name, Transform parent, Vector2 size, Vector2 position)
    {
        var obj = new GameObject(name, typeof(RectTransform));
        var rect = obj.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        return rect;
    }

    private void Move(GameObject selected, MoveDirection direction)
    {
        var data = new AxisEventData(system) { moveDir = direction };
        ExecuteEvents.Execute(selected, data, ExecuteEvents.moveHandler);
    }

    [Test]
    public void DeviceButtonsFilterBindingsAndReturnFocusWhenSwitchingLists()
    {
        var translator = ScriptableObject.CreateInstance<InputTranslator>();
        try
        {
            var listRect = Rect("Remap list", root.transform, new Vector2(1024, 640), Vector2.zero);
            var scroll = listRect.gameObject.AddComponent<ScrollRect>();
            var viewport = Rect("Viewport", listRect, new Vector2(1024, 560), Vector2.zero);
            scroll.viewport = viewport;
            var content = Rect("Content", viewport, new Vector2(1024, 560), Vector2.zero);
            content.pivot = new Vector2(0.5f, 1f);
            content.gameObject.AddComponent<VerticalLayoutGroup>();
            content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            scroll.content = content;
            var manager = root.AddComponent<ControlRemapManager>();
            SetRemapField(manager, "_controlsContainer", content.gameObject);
            SetRemapField(manager, "_inputTranslator", translator);
            SetRemapField(manager, "_remapOptionPrefab", UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/Level/Prefabs/UI/RemapOption.prefab"));
            manager.SendMessage("Start");

            var keyboard = root.transform.Find("Binding Device/KeyboardBindingsButton").GetComponent<Button>();
            var controller = root.transform.Find("Binding Device/ControllerBindingsButton").GetComponent<Button>();
            Assert.That(keyboard.transform.GetSiblingIndex(), Is.LessThan(controller.transform.GetSiblingIndex()));
            Assert.That(listRect.rect.height, Is.EqualTo(560f));
            int totalRows = content.childCount;
            AssertBindingType(content, InputTranslator.KeyboardMouseScheme);
            var selectedRow = content.GetComponentInChildren<Button>();
            system.SetSelectedGameObject(selectedRow.gameObject);
            controller.onClick.Invoke();
            AssertBindingType(content, InputTranslator.ControllerScheme);
            Assert.That(system.currentSelectedGameObject, Is.EqualTo(controller.gameObject));
            Assert.That(scroll.verticalNormalizedPosition, Is.EqualTo(1f).Within(0.001f));
            keyboard.onClick.Invoke();
            AssertBindingType(content, InputTranslator.KeyboardMouseScheme);
            Assert.That(content.childCount, Is.EqualTo(totalRows), "Switching must reuse the binding rows.");
        }
        finally
        {
            Object.DestroyImmediate(translator);
        }
    }

    private static void SetRemapField(ControlRemapManager manager, string name, object value)
    {
        typeof(ControlRemapManager).GetField(name, System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic).SetValue(manager, value);
    }

    private static void AssertBindingType(Transform content, string type)
    {
        var rows = content.GetComponentsInChildren<RemapOptionHandler>();
        Assert.That(rows.Length, Is.GreaterThan(0));
        foreach (var row in rows)
        {
            var name = (TextMeshProUGUI)typeof(RemapOptionHandler).GetField("_ActionNameText",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(row);
            Assert.That(name.text, Does.Not.Contain("[Keyboard/Mouse]").And.Not.Contain("[Controller]"));
            var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
            var action = (InputAction)typeof(RemapOptionHandler).GetField("_inputAction", flags).GetValue(row);
            var index = (int)typeof(RemapOptionHandler).GetField("_bindingIndex", flags).GetValue(row);
            Assert.That(InputBinding.MaskByGroup(type).Matches(action.bindings[index]), Is.True);
        }
    }

    [Test]
    public void DeviceButtonsHaveControllerPathsToEachOtherAndTheRemapList()
    {
        var tab = Rect("ControlTab", root.transform, Vector2.one * 20, Vector2.zero).gameObject.AddComponent<Button>();
        var back = Rect("BackButton", root.transform, Vector2.one * 20, Vector2.zero).gameObject.AddComponent<Button>();
        var keyboard = Rect("Keyboard", root.transform, Vector2.one * 20, Vector2.zero).gameObject.AddComponent<Button>();
        var controller = Rect("Controller", root.transform, Vector2.one * 20, Vector2.zero).gameObject.AddComponent<Button>();
        var viewport = Rect("Viewport", root.transform, new Vector2(200, 100), Vector2.zero);
        var scroll = viewport.gameObject.AddComponent<ScrollRect>();
        scroll.viewport = viewport;
        var content = Rect("Content", viewport, new Vector2(200, 100), Vector2.zero);
        scroll.content = content;
        var row = Rect("Binding", content, new Vector2(100, 30), Vector2.zero).gameObject.AddComponent<Button>();
        var navigation = content.gameObject.AddComponent<VisibleBindingNavigation>();
        navigation.Setup(scroll, keyboard, controller);
        navigation.SendMessage("Update");
        system.SetSelectedGameObject(tab.gameObject);
        Move(tab.gameObject, MoveDirection.Down);
        Assert.That(system.currentSelectedGameObject, Is.EqualTo(keyboard.gameObject));
        Move(keyboard.gameObject, MoveDirection.Right);
        Assert.That(system.currentSelectedGameObject, Is.EqualTo(controller.gameObject));
        Move(controller.gameObject, MoveDirection.Down);
        Assert.That(system.currentSelectedGameObject, Is.EqualTo(row.gameObject));
        Move(row.gameObject, MoveDirection.Up);
        Assert.That(system.currentSelectedGameObject, Is.EqualTo(keyboard.gameObject));
        Move(keyboard.gameObject, MoveDirection.Up);
        Assert.That(system.currentSelectedGameObject, Is.EqualTo(tab.gameObject));
    }

    [Test]
    public void PauseNavigationIncludesMainMenuInBothDirections()
    {
        var panel = Rect("Pause", root.transform, new Vector2(200, 300), Vector2.zero);
        panel.gameObject.SetActive(false);
        var resume = Rect("ContinueButton", panel, Vector2.one * 20, Vector2.zero).gameObject.AddComponent<Button>();
        var settings = Rect("SettingsButton", panel, Vector2.one * 20, Vector2.zero).gameObject.AddComponent<Button>();
        var menu = Rect("MainMenuButton", panel, Vector2.one * 20, Vector2.zero).gameObject.AddComponent<Button>();
        var exit = Rect("ExitButton", panel, Vector2.one * 20, Vector2.zero).gameObject.AddComponent<Button>();
        panel.gameObject.AddComponent<PauseMenu>();
        panel.gameObject.SetActive(true);
        system.SetSelectedGameObject(settings.gameObject);
        Move(settings.gameObject, MoveDirection.Down);
        Assert.That(system.currentSelectedGameObject, Is.EqualTo(menu.gameObject));
        Move(menu.gameObject, MoveDirection.Down);
        Assert.That(system.currentSelectedGameObject, Is.EqualTo(exit.gameObject));
        Move(exit.gameObject, MoveDirection.Up);
        Assert.That(system.currentSelectedGameObject, Is.EqualTo(menu.gameObject));
        Move(menu.gameObject, MoveDirection.Up);
        Assert.That(system.currentSelectedGameObject, Is.EqualTo(settings.gameObject));
    }

    [Test]
    public void BindingFocusStopsAtVisibleRowsAndRecoversAfterScrolling()
    {
        var viewport = Rect("Viewport", root.transform, new Vector2(200, 100), Vector2.zero);
        var scroll = viewport.gameObject.AddComponent<ScrollRect>();
        scroll.viewport = viewport;
        var content = Rect("Content", viewport, new Vector2(200, 200), Vector2.zero);
        scroll.content = content;
        var first = Rect("First", content, new Vector2(100, 30), new Vector2(0, 25)).gameObject.AddComponent<Button>();
        var last = Rect("Last visible", content, new Vector2(100, 30), new Vector2(0, -25)).gameObject.AddComponent<Button>();
        var hidden = Rect("Hidden", content, new Vector2(100, 30), new Vector2(0, -75)).gameObject.AddComponent<Button>();
        var navigation = content.gameObject.AddComponent<VisibleBindingNavigation>();
        navigation.Setup(scroll);
        system.SetSelectedGameObject(last.gameObject);
        navigation.SendMessage("Update");
        Move(last.gameObject, MoveDirection.Down);
        Assert.That(system.currentSelectedGameObject, Is.EqualTo(last.gameObject));
        Assert.That(hidden.navigation.mode, Is.EqualTo(Navigation.Mode.None));
        Assert.That(first.navigation.selectOnUp, Is.Null);
        content.anchoredPosition += new Vector2(0, 100);
        navigation.SendMessage("Update");
        Assert.That(system.currentSelectedGameObject, Is.EqualTo(hidden.gameObject));
        Assert.That(hidden.navigation.selectOnDown, Is.Null);
    }

    [Test]
    public void DropdownKeepsItsOptionDuringFadeAndCanSubmitAnotherOption()
    {
        var focus = eventObject.AddComponent<ControllerUIFocus>();
        var pad = InputSystem.AddDevice<Gamepad>();
        Press(pad.buttonSouth);
        var dropdownRect = Rect("Resolution", root.transform, new Vector2(200, 30), Vector2.zero);
        var dropdown = dropdownRect.gameObject.AddComponent<TMP_Dropdown>();
        var template = Rect("Template", dropdownRect, new Vector2(200, 100), new Vector2(0, -80));
        var scroll = template.gameObject.AddComponent<ScrollRect>();
        template.gameObject.AddComponent<CanvasGroup>();
        var viewport = Rect("Viewport", template, new Vector2(200, 100), Vector2.zero);
        var content = Rect("Content", viewport, new Vector2(200, 24), Vector2.zero);
        var item = Rect("Item", content, new Vector2(200, 24), Vector2.zero);
        item.anchorMin = new Vector2(0, 0.5f);
        item.anchorMax = new Vector2(1, 0.5f);
        item.sizeDelta = new Vector2(0, 24);
        item.gameObject.AddComponent<Toggle>();
        scroll.viewport = viewport;
        scroll.content = content;
        template.gameObject.SetActive(false);
        dropdown.template = template;
        for (int i = 0; i < 10; i++) dropdown.options.Add(new TMP_Dropdown.OptionData("Resolution " + i));
        dropdown.value = 8;
        dropdown.Show();
        var selected = system.currentSelectedGameObject;
        Assert.That(selected.GetComponent<Toggle>(), Is.Not.Null);
        focus.SendMessage("LateUpdate");
        Assert.That(system.currentSelectedGameObject, Is.EqualTo(selected), "Opening fade must not steal selection.");
        var popupScroll = selected.GetComponentInParent<ScrollRect>();
        Assert.That(VisibleBindingNavigation.IsFullyVisible(popupScroll.viewport, (RectTransform)selected.transform), Is.True);
        Move(selected, MoveDirection.Up);
        ExecuteEvents.Execute(system.currentSelectedGameObject, new BaseEventData(system), ExecuteEvents.submitHandler);
        Assert.That(dropdown.value, Is.EqualTo(7));
    }
}
