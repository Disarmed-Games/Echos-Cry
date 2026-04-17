using AudioSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Echo's Cry/Enemy/Sound/Sound Config")]
public class EnemySoundConfig : ScriptableObject
{
    public soundEffect HitSFX;
    public soundEffect AttackSFX;
    public soundEffect ArmorHitSFX;
    public soundEffect ArmorBreakSFX;
}