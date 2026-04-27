using EchosCry.Combo;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class WeaponUpgradeManager : MonoBehaviour
{
    [SerializeField] private Player player;

    [Header("Effects")]
    [SerializeField] EffectData flameEffect;

    [Header("Event Channels (Subscribers)")]
    [SerializeField] EventChannel _flameEvent;
    
    private void OnEnable()
    {
        if (_flameEvent != null) _flameEvent.Channel += FlameUpgrade;
    }
    private void OnDisable()
    {
        if (_flameEvent != null) _flameEvent.Channel -= FlameUpgrade;
    }

    private void FlameUpgrade()
    {
        AddToActiveTier(flameEffect);

        Debug.Log("added effects");
        player.WeaponHolder.AddEffectPrimary(StateName.Light1, flameEffect);
        player.WeaponHolder.AddEffectPrimary(StateName.Light2, flameEffect);
        player.WeaponHolder.AddEffectPrimary(StateName.Light3, flameEffect);
        player.WeaponHolder.AddEffectPrimary(StateName.Light4, flameEffect);
        player.WeaponHolder.AddEffectPrimary(StateName.Light5, flameEffect);
    }

    private void AddToActiveTier(EffectData data)
    {
        switch (data.EffectTier)
        {
            case EchosCry.EffectTier.One:
                player.ActiveEffectsTier1.Add(data);
                break;
            case EchosCry.EffectTier.Two:
                player.ActiveEffectsTier2.Add(data);
                break;
            case EchosCry.EffectTier.Three:
                player.ActiveEffectsTier3.Add(data);
                break;
            default:
                break;
        }
    }
}
