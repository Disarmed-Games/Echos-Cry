using Ink.Parsed;
using UnityEngine;

using System.Collections.Generic;

public class DashHandler : MonoBehaviour
{
    private Collider dashCollider;
    private readonly List<EffectData> effects = new();
    private TempoConductor.HitQuality hitQuality = TempoConductor.HitQuality.Miss;

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
        AttackInfo attack = new AttackInfo.Builder()
            .SetDamage(0)
            .SetHitQuality(hitQuality)
            .SetOrigin(Player.Instance.transform)
            .SetEffects(effects.ToArray())
            .Build();
        other.GetComponent<IDamageable>()?.Execute(attack);
    }
}
