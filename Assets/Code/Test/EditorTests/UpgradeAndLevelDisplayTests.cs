using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeAndLevelDisplayTests
{
    private static readonly FieldInfo PlayerInstance = typeof(NonSpawnableSingleton<Player>).GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic);
    private static readonly FieldInfo UpgradeInstance = typeof(UpgradeManager).GetField("<Instance>k__BackingField", BindingFlags.Static | BindingFlags.NonPublic);

    [Test]
    public void UnfocusedCardCannotOverwriteHoveredCardsSharedDescription()
    {
        object previousPlayer = PlayerInstance.GetValue(null);
        object previousUpgrade = UpgradeInstance.GetValue(null);
        var playerObject = new GameObject("Test Player");
        playerObject.SetActive(false);
        PlayerInstance.SetValue(null, playerObject.AddComponent<Player>());
        var managerObject = new GameObject("Test Upgrade Manager");
        managerObject.SetActive(false);
        var manager = managerObject.AddComponent<UpgradeManager>();
        UpgradeInstance.SetValue(null, manager);
        var root = new GameObject("Upgrade Menu", typeof(RectTransform), typeof(Canvas));
        root.SetActive(false);
        try
        {
            var text = Text(root.transform, "Shared Description");
            var first = Card(root.transform, text);
            var second = Card(root.transform, text);
            first.SetUpgrade(UpgradeManager.UpgradeType.Haste);
            second.SetUpgrade(UpgradeManager.UpgradeType.Health);
            root.AddComponent<UpgradeMenuNavigation>();
            root.SetActive(true);
            first.SetSelectorState(true);
            second.SetSelectorState(true);
            first.OnPointerEnter(null);
            second.OnPointerEnter(null);
            first.OnPointerExit(null);
            Assert.That(text.text, Is.EqualTo(manager.GetDescription(UpgradeManager.UpgradeType.Health)));
            first.SetSelectorState(true);
            first.SetUpgrade(UpgradeManager.UpgradeType.Strength);
            Assert.That(text.text, Is.EqualTo(manager.GetDescription(UpgradeManager.UpgradeType.Health)));
            second.OnPointerExit(null);
            Assert.That(text.text, Is.EqualTo("Select an upgrade to see the improvements!"));
        }
        finally
        {
            Object.DestroyImmediate(root);
            PlayerInstance.SetValue(null, previousPlayer);
            UpgradeInstance.SetValue(null, previousUpgrade);
            Object.DestroyImmediate(managerObject);
            Object.DestroyImmediate(playerObject);
        }
    }

    [Test]
    public void HudReopeningDisplaysLevelChangesMissedWhileHidden()
    {
        object previousPlayer = PlayerInstance.GetValue(null);
        var playerObject = new GameObject("Test Player");
        playerObject.SetActive(false);
        var player = playerObject.AddComponent<Player>();
        var xp = playerObject.AddComponent<PlayerXP>();
        Set(player, "_xp", xp);
        Set(xp, "_currentLevel", 1);
        Set(xp, "_goalXPAmount", 50f);
        PlayerInstance.SetValue(null, player);
        var channel = ScriptableObject.CreateInstance<FloatFloatIntEventChannel>();
        var hud = new GameObject("HUD", typeof(RectTransform));
        hud.SetActive(false);
        try
        {
            var bar = hud.AddComponent<PlayerXpBar>();
            var level = Text(hud.transform, "Level");
            var amount = Text(hud.transform, "XP");
            Set(bar, "levelText", level);
            Set(bar, "xpText", amount);
            Set(bar, "eventChannel", channel);
            hud.SetActive(true);
            Assert.That(level.text, Is.EqualTo("1"));
            hud.SetActive(false);
            Set(xp, "_currentLevel", 4);
            Set(xp, "_currentXPAmount", 12f);
            channel.Invoke(12f, 50f, 4);
            Assert.That(level.text, Is.EqualTo("1"), "Hidden HUD has no event subscription.");
            hud.SetActive(true);
            Assert.That(level.text, Is.EqualTo("4"));
            Assert.That(amount.text, Is.EqualTo("12 / 50"));
        }
        finally
        {
            Object.DestroyImmediate(hud);
            Object.DestroyImmediate(channel);
            PlayerInstance.SetValue(null, previousPlayer);
            Object.DestroyImmediate(playerObject);
        }
    }

    private static TextMeshProUGUI Text(Transform parent, string name)
    {
        var obj = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        obj.transform.SetParent(parent, false);
        return obj.GetComponent<TextMeshProUGUI>();
    }

    private static UpgradeSelector Card(Transform parent, TextMeshProUGUI description)
    {
        var obj = new GameObject("Upgrade", typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        var highlight = new GameObject("Highlight");
        highlight.transform.SetParent(obj.transform, false);
        var use = new GameObject("Use", typeof(RectTransform), typeof(Button));
        use.transform.SetParent(obj.transform, false);
        var selector = obj.AddComponent<UpgradeSelector>();
        Set(selector, "_highlight", highlight);
        Set(selector, "_useButton", use.GetComponent<Button>());
        Set(selector, "_upgradeNameText", Text(obj.transform, "Name"));
        Set(selector, "_upgradeDescriptionText", description);
        return selector;
    }

    private static void Set(object target, string name, object value) =>
        target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);
}
