using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class SpecialAttackHeatTests
{
    private static void Set(object target, string field, object value) =>
        target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);

    [TestCase(6, 0)]
    [TestCase(9, 3)]
    [TestCase(12, 6)]
    public void SpecialSpendsSixWithUpgradedCapacity(int stored, int remaining)
    {
        var root = new GameObject("Heat test");
        root.SetActive(false);
        var channel = ScriptableObject.CreateInstance<DoubleIntEventChannel>();
        try
        {
            var heat = root.AddComponent<HeatGauge>();
            Set(heat, "_heatUpdateEvent", channel);
            heat.MaxCharge = 12;
            heat.IncreaseCharge(stored);
            Assert.That(heat.UseCharge(HeatGauge.SpecialAttackCost), Is.True);
            Assert.That(heat.CurrentCharge, Is.EqualTo(remaining));
        }
        finally
        {
            Object.DestroyImmediate(root);
            Object.DestroyImmediate(channel);
        }
    }

    [TestCase(6)]
    [TestCase(9)]
    [TestCase(3)]
    public void UpgradedGaugeShowsOnlyStoredHeat(int stored)
    {
        var root = new GameObject("Heat UI test");
        root.SetActive(false);
        var lit = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);
        var empty = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);
        try
        {
            var ui = root.AddComponent<HeatGaugeUI>();
            var icons = new List<GameObject>();
            for (int i = 0; i < 12; i++)
            {
                var icon = new GameObject("Heat icon", typeof(RectTransform), typeof(Image), typeof(Animator));
                icon.transform.SetParent(root.transform);
                icons.Add(icon);
            }
            Set(ui, "heatGuages", icons);
            Set(ui, "defaultHeatImage", lit);
            Set(ui, "maxHeatImage", lit);
            Set(ui, "disabledHeatImage", empty);
            typeof(HeatGaugeUI).GetMethod("UpdateHeatGauge", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(ui, new object[] { stored, 12 });
            for (int i = 0; i < 12; i++)
                Assert.That(icons[i].GetComponent<Image>().sprite, Is.SameAs(i < stored ? lit : empty));
        }
        finally
        {
            Object.DestroyImmediate(root);
            Object.DestroyImmediate(lit);
            Object.DestroyImmediate(empty);
        }
    }
}
