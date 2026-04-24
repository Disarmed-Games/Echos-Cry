using System.Collections;
using UnityEngine;

public abstract class PassiveEffect : ScriptableObject
{
    public string effectName;
    public string effectDescription;
    public Sprite effectIcon;
    public bool isEffectOneTime = false;
    public float effectUseInterval;
    public float effectDuration;

    public abstract void Use(Enemy enemy);
}