using System.Collections;
using UnityEngine;

public class HitStop : Singleton<HitStop>
{
    readonly float _cooldown = 1f;
    bool _canHit = true;
    public void Execute(float duration)
    {
        if (!_canHit) return;
        Time.timeScale = 0;
        StartCoroutine(TimeDuration(duration));
        //StartCoroutine(Cooldown());
    }
    IEnumerator TimeDuration(float duration)
    {
        _canHit = false;
        yield return new WaitForSecondsRealtime(duration);
        StartCoroutine(Cooldown());
        Time.timeScale = 1;
    }
    IEnumerator Cooldown()
    {
        yield return new WaitForSecondsRealtime(_cooldown);
        _canHit = true;
    }
}
