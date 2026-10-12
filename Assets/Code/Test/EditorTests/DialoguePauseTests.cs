using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class DialoguePauseTests
{
    private static readonly Type Pause = typeof(DialogueEvents).Assembly.GetType("DialogueGameplayPause");
    private static void Call(string method, object owner) =>
        Pause.GetMethod(method, BindingFlags.Public | BindingFlags.Static).Invoke(null, new[] { owner });

    [TestCase(1f)]
    [TestCase(0.5f)]
    [TestCase(0f)]
    public void DialogueRestoresPreviousSpeedAfterLastOwnerExits(float speed)
    {
        float original = Time.timeScale;
        object first = new object(), second = new object();
        try
        {
            Time.timeScale = speed;
            Call("Acquire", first);
            Assert.That(Time.timeScale, Is.Zero);
            Call("Acquire", first); // Duplicate entry must not overwrite the saved speed.
            Call("Acquire", second);
            Call("Release", first);
            Assert.That(Time.timeScale, Is.Zero);
            Call("Release", second);
            Assert.That(Time.timeScale, Is.EqualTo(speed));
            Call("Release", second);
            Assert.That(Time.timeScale, Is.EqualTo(speed));
        }
        finally
        {
            Call("Release", first);
            Call("Release", second);
            Time.timeScale = original;
        }
    }
}
