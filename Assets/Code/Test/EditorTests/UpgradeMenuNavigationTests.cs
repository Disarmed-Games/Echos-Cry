using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeMenuNavigationTests
{
    [Test]
    public void UseButtonsCanBeReachedSubmittedAndExitedWithoutSelectingLockedUpgrades()
    {
        var root = new GameObject("UpgradeCanvas", typeof(RectTransform), typeof(Canvas));
        root.SetActive(false);
        var events = new GameObject("Events", typeof(EventSystem));
        var system = events.GetComponent<EventSystem>();
        EventSystem.current = system;
        try
        {
            var first = CreateUpgrade(root.transform, "First");
            var second = CreateUpgrade(root.transform, "Second");
            var third = CreateUpgrade(root.transform, "Third");
            var backObject = new GameObject("BackButton", typeof(RectTransform), typeof(Button));
            backObject.transform.SetParent(root.transform, false);
            var back = backObject.GetComponent<Button>();
            var navigation = root.AddComponent<UpgradeMenuNavigation>();
            root.SetActive(true);
            first.SetSelectorState(true);
            second.SetSelectorState(true);
            third.SetSelectorState(true);
            navigation.SendMessage("Update");
            Assert.That(system.currentSelectedGameObject, Is.EqualTo(first.UseButton.gameObject));
            int submits = 0;
            first.UseButton.onClick.AddListener(() => submits++);
            ExecuteEvents.Execute(first.UseButton.gameObject, new BaseEventData(system), ExecuteEvents.submitHandler);
            Assert.That(submits, Is.EqualTo(1));
            Move(system, first.UseButton, MoveDirection.Down);
            Assert.That(system.currentSelectedGameObject, Is.EqualTo(second.UseButton.gameObject));
            Move(system, second.UseButton, MoveDirection.Down);
            Assert.That(system.currentSelectedGameObject, Is.EqualTo(third.UseButton.gameObject));
            Move(system, third.UseButton, MoveDirection.Left);
            Assert.That(system.currentSelectedGameObject, Is.EqualTo(back.gameObject));
            Move(system, back, MoveDirection.Right);
            Assert.That(system.currentSelectedGameObject, Is.EqualTo(first.UseButton.gameObject));

            first.SetSelectorState(false);
            third.SetSelectorState(false);
            navigation.SendMessage("Update");
            Assert.That(system.currentSelectedGameObject, Is.EqualTo(second.UseButton.gameObject));
            second.SetSelectorState(false);
            navigation.SendMessage("Update");
            Assert.That(system.currentSelectedGameObject, Is.EqualTo(back.gameObject));
            Assert.That(back.navigation.selectOnRight, Is.Null);
        }
        finally
        {
            Object.DestroyImmediate(root);
            Object.DestroyImmediate(events);
        }
    }

    private static UpgradeSelector CreateUpgrade(Transform parent, string name)
    {
        var obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        var highlight = new GameObject("Highlight");
        highlight.transform.SetParent(obj.transform, false);
        var nameText = new GameObject("Name", typeof(RectTransform), typeof(TextMeshProUGUI));
        nameText.transform.SetParent(obj.transform, false);
        var description = new GameObject("Description", typeof(RectTransform), typeof(TextMeshProUGUI));
        description.transform.SetParent(obj.transform, false);
        var use = new GameObject("Use", typeof(RectTransform), typeof(Button));
        use.transform.SetParent(obj.transform, false);
        var selector = obj.AddComponent<UpgradeSelector>();
        SetField(selector, "_highlight", highlight);
        SetField(selector, "_upgradeNameText", nameText.GetComponent<TextMeshProUGUI>());
        SetField(selector, "_upgradeDescriptionText", description.GetComponent<TextMeshProUGUI>());
        SetField(selector, "_useButton", use.GetComponent<Button>());
        return selector;
    }

    private static void SetField(UpgradeSelector selector, string name, object value) =>
        typeof(UpgradeSelector).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(selector, value);

    private static void Move(EventSystem system, Button button, MoveDirection direction) =>
        ExecuteEvents.Execute(button.gameObject, new AxisEventData(system) { moveDir = direction }, ExecuteEvents.moveHandler);
}
