using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    public static BeatManager Instance { get; private set; }
    public int BeatInMeasure = 0;
    public float BeatProgress = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [SerializeField] private float _bpm;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Intervals[] _intervals;

    private void Update()
    {
        foreach (var interval in _intervals)
        {
            float sampledTime = (_audioSource.timeSamples / (_audioSource.clip.frequency * interval.GetIntervalLength(_bpm)));

            Debug.Log(sampledTime - Mathf.Floor(sampledTime));

            interval.CheckForNewInterval(sampledTime);
        }
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