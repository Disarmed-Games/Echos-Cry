using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Critical Hit", menuName = "Echo's Cry/Passive Effects/Critical Hit")]
public class CriticalHitPassiveEffect : PassiveEffect
{
    [Range(0f, 1f)]
    public float criticalChance = .2f;
    public float criticalMultiplier = 1.5f;

    public override void Use(Enemy enemy)
    {
        bool isCriticalHit = RollCriticalHit();
        if (isCriticalHit)
        {
            enemy.Health.SetDamageMultiplier(criticalMultiplier);
            enemy.StartCoroutine(ResetDamageMultiplier(.1f, enemy));
        }
    }
    private bool RollCriticalHit()
    {
        float randomVal = Random.Range(0f, 1f);
        return (randomVal <= criticalChance);
    }

    private IEnumerator ResetDamageMultiplier(float time, Enemy enemy)
    {
        yield return new WaitForSeconds(time);
        enemy.Health.SetDamageMultiplier(1f);
    }
}
