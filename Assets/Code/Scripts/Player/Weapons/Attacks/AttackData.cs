using AudioSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Echo's Cry/Combo System/Attack Data")]
public class AttackData : ScriptableObject
{
    public AnimationClip              AnimationClip;
    public soundEffect                AttackSound;
    public float                      BaseDamage;
    public ThreePassiveEffects        PassiveEffects;
}

[System.Serializable]
public class ThreePassiveEffects
{
    public PassiveEffect OneThirdEffect;
    public PassiveEffect TwoThirdsEffect;
    public PassiveEffect FullEffect;
}
