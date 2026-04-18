using System.Collections;
using UnityEngine;

public class HitStop : Singleton<HitStop>
{
    public void Execute(float duration)
    {
        Time.timeScale = 0;
        StartCoroutine(TimeDuration(duration));
    }
    IEnumerator TimeDuration(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;
    }
}
