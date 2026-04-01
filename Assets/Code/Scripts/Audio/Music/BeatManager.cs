using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    public static BeatManager Instance { get; private set; }

    [SerializeField] private float _bpm;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Intervals[] _intervals;
    private float beatProgress;

    public int BeatInMeasure = 0;
    public float BeatProgress => beatProgress;
    public float BPM => _bpm;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        foreach (var interval in _intervals)
        {
            float sampledTime = (_audioSource.timeSamples / (_audioSource.clip.frequency * interval.GetIntervalLength(_bpm)));

            beatProgress = sampledTime - Mathf.Floor(sampledTime);

            interval.CheckForNewInterval(sampledTime);
        }
    }

    public float GetTimeBetweenBeats()
    {
        return _intervals[0].GetIntervalLength(_bpm);
    }
}

[System.Serializable]
public class Intervals
{
    [SerializeField] private float _steps;
    [SerializeField] private UnityEvent _trigger;
    public int LastInterval;

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