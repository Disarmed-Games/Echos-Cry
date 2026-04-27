using AudioSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Echo's Cry/Combo System/Attack Data")]
public class AttackData : ScriptableObject
{
    public AnimationClip       AnimationClip;
    public soundEffect         AttackSound;
    public float               BaseDamage;
    public float               BaseForce;
    public ThreePassiveEffects PassiveEffects;
    public int                 HeatCost;
}

[System.Serializable]
public class ThreePassiveEffects
{
    public EffectData OneThirdEffect;
    public EffectData TwoThirdsEffect;
    public EffectData FullEffect;
}
