using UnityEngine;

[CreateAssetMenu(fileName = "Critical Hit", menuName = "Echo's Cry/Passive Effects/Critical Hit")]
public class CriticalHitPassiveEffect : PassiveEffect
{
    [Range(0f, 1f)]
    public float criticalChance = .2f;
    public float criticalMultiplier = 1.5f;

    public bool RollCriticalHit()
    {
        float randomVal = Random.Range(0f, 1f);
        return (randomVal <= criticalChance);
    }
}
