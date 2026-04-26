using UnityEngine;
using EchosCry.Combo;

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
        Debug.Log("added effects");
        player.WeaponHolder.AddEffectPrimary(StateName.Light1, flameEffect);
        player.WeaponHolder.AddEffectPrimary(StateName.Light2, flameEffect);
        player.WeaponHolder.AddEffectPrimary(StateName.Light3, flameEffect);
        player.WeaponHolder.AddEffectPrimary(StateName.Light4, flameEffect);
        player.WeaponHolder.AddEffectPrimary(StateName.Light5, flameEffect);
    }
}
