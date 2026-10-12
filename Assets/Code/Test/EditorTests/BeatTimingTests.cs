using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class BeatTimingTests
{
    private GameObject root;
    private AudioClip clip;
    private AudioSource source;
    private BeatManager beat;
    private TempoConductor tempo;
    private BeatManager previous;

    [SetUp]
    public void Setup()
    {
        previous = BeatManager.Instance;
        root = new GameObject("Beat timing test");
        root.SetActive(false);
        source = root.AddComponent<AudioSource>();
        clip = AudioClip.Create("Timing", 480000, 1, 48000, false);
        source.clip = clip;
        beat = root.AddComponent<BeatManager>();
        tempo = root.AddComponent<TempoConductor>();
        Set(beat, "_audioSource", source);
        Set(beat, "_bpm", 120f);
        Set(tempo, "_excellentPercent", 0.1f);
        Set(tempo, "_goodPercent", 0.2f);
        BeatManager.Instance = beat;
    }

    private static void Set(object target, string name, object value) =>
        target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);

    [TearDown]
    public void Cleanup()
    {
        BeatManager.Instance = previous;
        Object.DestroyImmediate(root);
        Object.DestroyImmediate(clip);
    }

    [Test]
    public void ScoringReadsSamplesWithoutUpdate()
    {
        source.timeSamples = 24000;
        Assert.That(tempo.CurrentHitQuality, Is.EqualTo(TempoConductor.HitQuality.Excellent));
        source.timeSamples = 30000;
        Assert.That(beat.BeatProgress, Is.EqualTo(0.25f).Within(0.0001f));
        Assert.That(tempo.CurrentHitQuality, Is.EqualTo(TempoConductor.HitQuality.Miss));
    }

    [TestCase(0.96f, TempoConductor.HitQuality.Excellent)]
    [TestCase(1.04f, TempoConductor.HitQuality.Excellent)]
    [TestCase(0.92f, TempoConductor.HitQuality.Good)]
    [TestCase(1.08f, TempoConductor.HitQuality.Good)]
    [TestCase(0.88f, TempoConductor.HitQuality.Miss)]
    [TestCase(1.12f, TempoConductor.HitQuality.Miss)]
    public void EarlyAndLateWindowsMatch(float position, TempoConductor.HitQuality expected)
    {
        source.timeSamples = Mathf.RoundToInt(position * 24000);
        Assert.That(tempo.CurrentHitQuality, Is.EqualTo(expected));
    }

    [TestCase(0.92f, 1)]
    [TestCase(1.08f, 1)]
    [TestCase(2.92f, 2)]
    [TestCase(3.08f, 2)]
    [TestCase(1.12f, 0)]
    [TestCase(2f, 0)]
    public void SpecialAttacksUseNearestSecondOrFourthBeat(float position, int expected)
    {
        source.timeSamples = Mathf.RoundToInt(position * 24000);
        Assert.That(tempo.GetSpecialAttackIndex(), Is.EqualTo(expected));
    }
}
