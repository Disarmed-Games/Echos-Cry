using System.Collections;
using Unity.VisualScripting.YamlDotNet.Core;
using UnityEngine;

[CreateAssetMenu(fileName = "Marked For Death Effect", menuName = "Echo's Cry/Passive Effects/Marked For Death")]
public class MarkedForDeathPassive : PassiveEffect
{
    public float damageMultiplier = 1.2f;
    public float duration = 2f;

    public override void Use(Enemy enemy)
    {
        enemy.Health.SetDamageMultiplier(damageMultiplier);
        enemy.StartCoroutine(ResetDamageMultiplier(duration, enemy));
    }
    private IEnumerator ResetDamageMultiplier(float time, Enemy enemy)
    {
        yield return new WaitForSeconds(time);
        enemy.Health.SetDamageMultiplier(1f);
    }
}
