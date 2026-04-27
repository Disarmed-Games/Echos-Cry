using UnityEngine;

public class PlayerUpgradeManager : MonoBehaviour
{
    [SerializeField] Player player;

    [Header("Event Channels (Subscribers)")]
    [SerializeField] EventChannel _upgradeDamageChannel;
    [SerializeField] EventChannel _moveSpeedChannel;
    [SerializeField] EventChannel _dashSpeedChannel;
    [SerializeField] EventChannel _healthChannel;
    [SerializeField] EventChannel _armorChannel;
    [SerializeField] EventChannel _regenHealthChannel;
    [SerializeField] EventChannel _regenArmorChannel;
    [SerializeField] EventChannel _dashCountChannel;
    [SerializeField] EventChannel _dashCooldownChannel;
    [SerializeField] EventChannel _knockbackChannel;
    [SerializeField] EventChannel _heatgaugeChannel;

    private void OnEnable()
    {
        if (_upgradeDamageChannel != null) _upgradeDamageChannel.Channel += UpgradeBaseDamageMultiplier;
        if (_healthChannel != null) _healthChannel.Channel += UpgradeMaxHealth;
        if (_armorChannel != null) _armorChannel.Channel += UpgradeMaxArmor;
        if (_regenHealthChannel != null) _regenHealthChannel.Channel += UpgradeHealthRegen;
        if (_regenArmorChannel != null) _regenArmorChannel.Channel += UpgradeArmorRegen;
        if (_moveSpeedChannel != null) _moveSpeedChannel.Channel += UpgradeMoveSpeed;
        if (_dashSpeedChannel != null) _dashSpeedChannel.Channel += UpgradeDashSpeed;
        if(_dashCooldownChannel != null) _dashCooldownChannel.Channel += UpgradeDashCooldown;
        if(_dashCountChannel != null) _dashCountChannel.Channel += UpgradeDashCount;
        if (_knockbackChannel != null) _knockbackChannel.Channel += UpgradeKnockback;
        if (_heatgaugeChannel != null) _heatgaugeChannel.Channel += UpgradeHeatGauge;
    }
    private void OnDisable()
    {
        if (_upgradeDamageChannel != null) _upgradeDamageChannel.Channel -= UpgradeBaseDamageMultiplier;
        if (_healthChannel != null) _healthChannel.Channel -= UpgradeMaxHealth;
        if (_armorChannel != null) _armorChannel.Channel -= UpgradeMaxArmor;
        if (_regenHealthChannel != null) _regenHealthChannel.Channel -= UpgradeHealthRegen;
        if (_regenArmorChannel != null) _regenArmorChannel.Channel -= UpgradeArmorRegen;
        if (_moveSpeedChannel != null) _moveSpeedChannel.Channel -= UpgradeMoveSpeed;
        if (_dashSpeedChannel != null) _dashSpeedChannel.Channel -= UpgradeDashSpeed;
        if (_dashCooldownChannel != null) _dashCooldownChannel.Channel -= UpgradeDashCooldown;
        if (_dashCountChannel != null) _dashCountChannel.Channel -= UpgradeDashCount;
        if (_knockbackChannel != null) _knockbackChannel.Channel -= UpgradeKnockback;
        if (_heatgaugeChannel != null) _heatgaugeChannel.Channel -= UpgradeHeatGauge;
    }

    //Currently using unmutable variables but will eventually change to handle configuration or scaling upgrades eventually
    void UpgradeBaseDamageMultiplier()
    {
        player.Stats.DamageMultiplier *= 1.1f;
    }
    void UpgradeDashSpeed()
    {
        player.Movement.DashSpeed *= 1.15f;
    }
    void UpgradeDashCount()
    {
        player.Movement.DashCount++;
        player.Movement.DashMaxCount++;
    }
    void UpgradeDashCooldown()
    {
        player.Movement.DashCooldown *= 0.80f;
    }
    void UpgradeMoveSpeed()
    {
        player.Movement.MoveSpeed *= 1.1f;
    }
    void UpgradeMaxHealth()
    {
        player.Health.IncreaseMaxHealth(5f);
        player.Health.HealHealth(5f);
    }
    void UpgradeMaxArmor()
    {
        player.Health.IncreaseMaxArmor(5f);
        player.Health.HealArmor(5f);
    }
    void UpgradeHealthRegen()
    {
        player.Health.RegenHealthAmount++;
        player.Health.EnableHealthRegen();
    }
    void UpgradeArmorRegen()
    {
        player.Health.RegenArmorAmount++;
        player.Health.EnableArmorRegen();
    }

    void UpgradeKnockback()
    {
        player.Stats.KnockbackMultiplier *= 1.05f;
    }
    void UpgradeHeatGauge()
    {
        player.HeatGauge.AddCharges(1);
    }
}
