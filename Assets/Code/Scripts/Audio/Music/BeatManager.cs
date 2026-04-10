using System;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    public static BeatManager Instance;

    [SerializeField] private float _bpm;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Intervals[] _intervals;
    private float beatProgress;
    private int beatInMeasure;

    public int BeatInMeasure => beatInMeasure;
    public float BeatProgress => beatProgress;
    public float BPM => _bpm;

    public event Action onWholeBeat;

    public void PlaySong(AudioClip clip, float bpm)
    {
        _audioSource.clip = clip;
        _bpm = bpm;
        _audioSource.Play();
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        foreach (var interval in _intervals)
        {
            float sampledTime = (_audioSource.timeSamples / (_audioSource.clip.frequency * interval.GetIntervalLength(_bpm)));

            beatProgress = sampledTime - Mathf.Floor(sampledTime);
            beatInMeasure = (int)Mathf.Floor(sampledTime) % 4;
            //Debug.Log(beatInMeasure);

            interval.CheckForNewInterval(sampledTime);
        }
    }

    public float GetTimeBetweenBeats()
    {
        return _intervals[0].GetIntervalLength(_bpm);
    }

    public void ActivateWholeBeatEvent()
    {
        onWholeBeat?.Invoke();
    }
}

[System.Serializable]
public class Intervals
{
    [SerializeField] private float _steps;
    [SerializeField] private UnityEvent _trigger;
    private int LastInterval;

    public float GetIntervalLength(float bpm)
    {
        return 60f / (bpm * _steps);
    }

    public void CheckForNewInterval(float interval)
    {
        if (Mathf.FloorToInt(interval) != LastInterval)
        {
            LastInterval = Mathf.FloorToInt(interval);
            _trigger.Invoke();
        }
    }
}