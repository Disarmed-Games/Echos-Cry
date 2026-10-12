using System;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    public static BeatManager Instance;

    [SerializeField] private float _bpm;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Intervals[] _intervals;
    // Read playback on demand: input callbacks run before MonoBehaviour.Update.
    public double BeatPosition => _audioSource != null && _audioSource.clip != null && _bpm > 0
        ? (double)_audioSource.timeSamples / _audioSource.clip.frequency * _bpm / 60.0
        : 0.0;
    public bool HasSong => _audioSource != null && _audioSource.clip != null && _bpm > 0;
    public int BeatInMeasure => (int)(Math.Floor(BeatPosition) % 4);
    public float BeatProgress { get { double position = BeatPosition; return (float)(position - Math.Floor(position)); } }
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
        if (!HasSong || _intervals == null) return;
        foreach (var interval in _intervals)
        {
            float sampledTime = (_audioSource.timeSamples / (_audioSource.clip.frequency * interval.GetIntervalLength(_bpm)));

            interval.CheckForNewInterval(sampledTime);
        }
    }

    public float GetTimeBetweenBeats()
    {
        return _bpm > 0 ? 60f / _bpm : 0f;
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
