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

    private HitQuality _currentHitQuality;
    private HitQuality _currentOffHitQuality;
    public HitQuality CurrentHitQuality { get { return _currentHitQuality; } }
    public HitQuality CurrentOffHitQuality {  get {  return _currentOffHitQuality; } }

    //Beat Timing Values
    private float _currentBeatProgress = 0;

    //Hit Time Window --> Higher values are easier to hit within!
    private float _excellentPercent;
    private float _goodPercent;

    //            Tempo Threshold
    //  End                           Start
    //   |--|-|-|---------------|-|-|---|
    // BEAT 2                         BEAT 1

    // Events
    public event Action BeatTickEvent;

    public bool IsOnBeat()
    {
        return (_currentHitQuality == HitQuality.Good || _currentHitQuality == HitQuality.Excellent);
    }
    void Update()
    {
        UpdateHitQuality();
    }

    private HitQuality GetHitQuality(float progress)
    {
        if (progress <= _excellentPercent || progress >= (1f - _excellentPercent))
            return HitQuality.Excellent;

        if (progress <= _goodPercent || progress >= (1f - _goodPercent))
            return HitQuality.Good;

        return HitQuality.Miss;
    }

    private void UpdateHitQuality()
    {
        //--------------------
        // On Beat Check
        //--------------------
        //Closer to 0 or 1 is perfectly on beat. Close to 0.5 is offbeat.
        _currentBeatProgress = BeatManager.Instance.BeatProgress;
        _currentHitQuality = GetHitQuality(_currentBeatProgress);

        //--------------------
        // Off Beat Check
        //--------------------
        //Off Beats are inbetween regular beats. Denoted by &: 1 & 2 & 3 & 4 (4/4 signature)
        int currentBeat = MusicManager.Instance.GetBeatInMeasure(); //Beats are held as 0 - 3
        float offbeatProgress = _currentBeatProgress - 0.5f;
        if (offbeatProgress < 0) offbeatProgress += 1f;

        if (currentBeat == 0 || currentBeat == 1)
        { //Check the offbeat between beats 0 - 1 (1 & 2).
            _currentOffHitQuality = GetHitQuality(offbeatProgress);
        }
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