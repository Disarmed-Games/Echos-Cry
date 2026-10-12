using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class DashEffectNotificationTests
{
    [Test]
    public void UsingDashItemRegistersEffectBeforeNotifyingHud()
    {
        var root = new GameObject("Dash effect test");
        root.SetActive(false);
        var item = ScriptableObject.CreateInstance<DashAttackItemData>();
        var effect = ScriptableObject.CreateInstance<EffectData>();
        Action listener = null;
        try
        {
            var player = root.AddComponent<Player>();
            var dash = root.AddComponent<DashHandler>();
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            typeof(Player).GetField("_dashHandler", flags).SetValue(player, dash);
            typeof(DashAttackItemData).GetField("_dashAttack", flags).SetValue(item, effect);
            int notifications = 0;
            listener = () =>
            {
                notifications++;
                Assert.That(player.ActiveEffectsTier1, Does.Contain(effect));
                var registered = (List<EffectData>)typeof(DashHandler).GetField("effects", flags).GetValue(dash);
                Assert.That(registered, Does.Contain(effect));
            };
            DashHandler.OnDashEffectAdded += listener;
            item.Use(player);
            Assert.That(notifications, Is.EqualTo(1));
        }
        finally
        {
            DashHandler.OnDashEffectAdded -= listener;
            UnityEngine.Object.DestroyImmediate(root);
            UnityEngine.Object.DestroyImmediate(item);
            UnityEngine.Object.DestroyImmediate(effect);
        }
    }
}
