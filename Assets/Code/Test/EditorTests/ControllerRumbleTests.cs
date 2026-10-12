using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerRumbleTests : InputTestFixture
{
    public class RecordingGamepad : Gamepad
    {
        public float Low, High;
        public override void SetMotorSpeeds(float lowFrequency, float highFrequency)
        {
            Low = lowFrequency;
            High = highFrequency;
        }
        public override void ResetHaptics() { Low = High = 0f; }
    }

    private ControllerRumble rumble;
    private RecordingGamepad pad;

    public override void Setup()
    {
        base.Setup();
        InputSystem.RegisterLayout<RecordingGamepad>();
        pad = InputSystem.AddDevice<RecordingGamepad>();
        rumble = new GameObject("Rumble test").AddComponent<ControllerRumble>();
    }

    public override void TearDown()
    {
        Object.DestroyImmediate(rumble.gameObject);
        base.TearDown();
    }

    [Test]
    public void OverlappingPulsesKeepStrongestClampedMotorSpeeds()
    {
        rumble.Pulse(pad, 2f, 0.6f, 1f);
        rumble.Pulse(pad, 0.2f, 0.3f, 0.1f);
        Assert.That(pad.Low, Is.EqualTo(1f));
        Assert.That(pad.High, Is.EqualTo(0.6f));
        rumble.Stop();
        Assert.That(pad.Low, Is.Zero);
        Assert.That(pad.High, Is.Zero);
    }

    [Test]
    public void ExpiredPulseStopsBothMotors()
    {
        rumble.Pulse(pad, 0.5f, 0.7f, 1f);
        typeof(ControllerRumble).GetField("_until",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            .SetValue(rumble, Time.unscaledTime - 1f);
        rumble.SendMessage("Update");
        Assert.That(pad.Low, Is.Zero);
        Assert.That(pad.High, Is.Zero);
    }

    [Test]
    public void DisablingRumbleStopsBothMotors()
    {
        rumble.Pulse(pad, 0.5f, 0.7f, 1f);
        rumble.enabled = false;
        Assert.That(pad.Low, Is.Zero);
        Assert.That(pad.High, Is.Zero);
    }

    [Test]
    public void SwitchingControllersStopsPreviousController()
    {
        var second = InputSystem.AddDevice<RecordingGamepad>();
        rumble.Pulse(pad, 0.5f, 0.7f, 1f);
        rumble.Pulse(second, 0.2f, 0.3f, 0.1f);
        Assert.That(pad.Low, Is.Zero);
        Assert.That(pad.High, Is.Zero);
        Assert.That(second.Low, Is.EqualTo(0.2f));
        Assert.That(second.High, Is.EqualTo(0.3f));
    }
}
