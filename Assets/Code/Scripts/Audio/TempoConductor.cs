using System;
using UnityEngine;

public class TempoConductor : Singleton<TempoConductor>
{
    public enum HitQuality
    {
        Miss = 0,
        Good,
        Excellent
    }

    public HitQuality CurrentHitQuality => EvaluateCurrentQuality(0f);
    public HitQuality CurrentOffHitQuality => EvaluateCurrentQuality(0.5f);

    //Hit Time Window --> Higher values are easier to hit within!
    private float _excellentPercent;
    private float _goodPercent;

    public float ExcellentPercent { get => _excellentPercent; }
    public float GoodPercent { get => _goodPercent; }

    //            Tempo Threshold
    //  End                           Start
    //   |--|-|-|---------------|-|-|---|
    // BEAT 2                         BEAT 1


    public bool IsOnBeat()
    {
        return CurrentHitQuality != HitQuality.Miss;
    }


    private HitQuality GetHitQuality(float progress)
    {
        float distance = Mathf.Min(progress, 1f - progress);
        // Percentages describe the total window, split evenly before and after a beat.
        if (distance <= _excellentPercent * 0.5f)
            return HitQuality.Excellent;

        if (distance <= _goodPercent * 0.5f)
            return HitQuality.Good;

        return HitQuality.Miss;
    }

    private HitQuality EvaluateCurrentQuality(float offset)
    {
        var beat = BeatManager.Instance;
        return beat != null && beat.HasSong
            ? GetHitQuality(Mathf.Repeat(beat.BeatProgress - offset, 1f)) : HitQuality.Miss;
    }

    // Shared by the indicator and both movement states. 1/2 are attacks on beats 2/4.
    public int GetSpecialAttackIndex()
    {
        var beat = BeatManager.Instance;
        if (beat == null || !beat.HasSong) return 0;
        double position = beat.BeatPosition;
        double nearest = Math.Floor(position + 0.5);
        if (Math.Abs(position - nearest) > _goodPercent * 0.5) return 0;
        int measureBeat = (int)(nearest % 4);
        return measureBeat == 1 ? 1 : measureBeat == 3 ? 2 : 0;
    }

    private void Awake()
    {
        //--------------------
        // Beat Accuracy Algorithm
        //--------------------
        //This works by creating values for every 10th percentile.
        //So if your accuracy is between 90% - 100%, then it is the hardest range of 0.1 for excellent and 0.2 for good.
        //The lowest accuracy of 0% - 10% creates a range of 0.3 for excellent and 0.4 for good.

        float accuracy = Mathf.Clamp01(CalibrationManager.HitAccuracy);

        float t = Mathf.InverseLerp(0.1f, 0.9f, accuracy);
        t = 1f - t;

        _excellentPercent = Mathf.Lerp(0.1f, 0.3f, t);
        _goodPercent = Mathf.Lerp(0.2f, 0.4f, t);

        _excellentPercent = Mathf.Clamp(_excellentPercent, 0f, 1f);
        _goodPercent = Mathf.Clamp(_goodPercent, 0f, 1f);

        Debug.Log($"Excellent:{_excellentPercent} Good:{_goodPercent}");
    }
}
