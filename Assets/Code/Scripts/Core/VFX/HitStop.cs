using System.Collections;
using UnityEngine;

public class HitStop : Singleton<HitStop>
{
    readonly float _cooldown = 1f;
    bool _canHit = true;
    public static bool IsStopping { get; private set; }
    public void Execute(float duration)
    {
        if (!_canHit || DialogueGameplayPause.IsPaused) return;
        Time.timeScale = 0;
        IsStopping = true;
        StartCoroutine(TimeDuration(duration));
        //StartCoroutine(Cooldown());
    }
    IEnumerator TimeDuration(float duration)
    {
        _canHit = false;
        yield return new WaitForSecondsRealtime(duration);
        IsStopping = false;
        StartCoroutine(Cooldown());
        if (!DialogueGameplayPause.IsPaused) Time.timeScale = 1;
    }
    IEnumerator Cooldown()
    {
        yield return new WaitForSecondsRealtime(_cooldown);
        _canHit = true;
    }
}
