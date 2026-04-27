using UnityEngine;

public class DashUpgradeManager : MonoBehaviour
{
    [SerializeField] private Player player;

    [Header("Effects")]
    [SerializeField] EffectData _damageEffect;

    [Header("Event Channels")]
    [SerializeField] EventChannel _dashAttackEvent;

    private void OnEnable()
    {
        if(_dashAttackEvent != null) _dashAttackEvent.Channel += AddDashAttack;
    }
    private void OnDisable()
    {
        if (_dashAttackEvent != null) _dashAttackEvent.Channel -= AddDashAttack;
    }
    void AddDashAttack()
    {
        player.DashHandler.AddEffect(_damageEffect);
    }
}