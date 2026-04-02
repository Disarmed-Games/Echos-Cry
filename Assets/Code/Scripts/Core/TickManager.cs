using System;
using System.Collections.Generic;
using UnityEngine;

public class TimerNode
{
    private float _timer;
    private readonly float _tickTime;
    public event Action Tick;

    public TimerNode(float tickTime)
    {
        _timer = 0;
        _tickTime = tickTime;
    }
    public void Update()
    {
        _timer += Time.deltaTime;
        if(_timer >= _tickTime)
        {
            Tick?.Invoke();
            _timer -= _tickTime;
        }
    }
}
//Other TODO: may or may not be relevant to this script but design way to separate out tick events across frames, specifically for enemies (could use queue?)
public class TickManager : Singleton<TickManager>
{
    struct Timer
    {
        public float tick;
        public TimerNode node;
    }

    private List<Timer> _timers;

    private TimerNode AddTimer(float tickTime)
    {
        Timer newTimer = new Timer
        {
            tick = tickTime,
            node = new TimerNode(tickTime),
        };

        _timers.Add(newTimer);
        return newTimer.node;
    }
    public TimerNode GetTimer(float tickTime)
    {
        for (int i = 0; i < _timers.Count; i++)
        {
            if (Mathf.Approximately(_timers[i].tick, tickTime))
            {
                return _timers[i].node;
            }
        }

        return AddTimer(tickTime);
    }
    private void UpdateTimers()
    {
        int count = _timers.Count;

        for (int i = 0; i < count; i++)
        {
            _timers[i].node.Update();
        }
    }

    protected override void OnAwake()
    {
        _timers = new();
    }
    void Update()
    {
        UpdateTimers();
    }
    private void OnDisable()
    {
        if(_timers != null) _timers.Clear();
    }
}