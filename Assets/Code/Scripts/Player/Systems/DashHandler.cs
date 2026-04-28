using Ink.Parsed;
using UnityEngine;

using System.Collections.Generic;
using System;

public class DashHandler : MonoBehaviour
{
    private Collider dashCollider;
    private readonly List<EffectData> effects = new();
    private TempoConductor.HitQuality hitQuality = TempoConductor.HitQuality.Miss;
    public static event Action OnDashEffectAdded;

    private void OnEnable()
    {
        dashCollider.enabled = false;
    }

    private void Awake()
    {
        dashCollider = GetComponent<Collider>();
    }
    public void AddEffect(EffectData effect)
    {
        if (effect == null) return;
        OnDashEffectAdded?.Invoke();
        effects.Add(effect);
    }
    public void ClearEffects()
    {
        effects.Clear();
    }
    public void EnableCollider(TempoConductor.HitQuality hitQuality)
    {
        dashCollider.enabled = true;
        this.hitQuality = hitQuality;
    }
    public void DisableCollider()
    {
        dashCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(effects.Count <= 0) return;  
        AttackInfo attack = new AttackInfo.Builder()
            .SetDamage(0)
            .SetHitQuality(hitQuality)
            .SetOrigin(Player.Instance.transform)
            .SetEffects(effects.ToArray())
            .Build();
        other.GetComponent<IDamageable>()?.Execute(attack);
    }
}
