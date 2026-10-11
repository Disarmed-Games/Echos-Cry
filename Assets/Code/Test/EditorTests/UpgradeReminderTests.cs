using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UpgradeReminderTests : InputTestFixture
{
    [Test]
    public void ControllerOpensUpgradesAndReminderFollowsDeviceAndRebinding()
    {
        var pad = InputSystem.AddDevice<Gamepad>();
        var keyboard = InputSystem.AddDevice<Keyboard>();
        var translator = ScriptableObject.CreateInstance<InputTranslator>();
        var label = new GameObject("Upgrade Reminder", typeof(RectTransform), typeof(TextMeshProUGUI));
        label.SetActive(false);
        try
        {
            var reminder = label.AddComponent<UpgradeReminderPrompt>();
            typeof(UpgradeReminderPrompt).GetField("translator", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(reminder, translator);
            var text = label.GetComponent<TextMeshProUGUI>();
            label.SetActive(true);
            Assert.That(text.text, Is.EqualTo("Press 'Tab' to upgrade!"));
            int requests = 0;
            translator.OnUpgradeEvent += () => requests++;
            Press(pad.selectButton);
            Assert.That(requests, Is.EqualTo(1));
            Assert.That(text.text, Is.EqualTo($"Press '{translator.GetBindingDisplayString(translator.PlayerInputs.Gameplay.Upgrade)}' to upgrade!"));
            Assert.That(translator.ActiveControlScheme, Is.EqualTo("Controller"));
            Press(keyboard.wKey);
            Assert.That(text.text, Is.EqualTo("Press 'Tab' to upgrade!"));
            translator.PlayerInputs.Gameplay.Upgrade.ApplyBindingOverride(0, "<Keyboard>/f");
            RemapOptionHandler.OnActionRemappedEvent?.Invoke();
            Assert.That(text.text, Is.EqualTo("Press 'F' to upgrade!"));
            Press(keyboard.fKey);
            Assert.That(requests, Is.EqualTo(2));
        }
        finally
        {
            Object.DestroyImmediate(label);
            Object.DestroyImmediate(translator);
        }
    }
}
