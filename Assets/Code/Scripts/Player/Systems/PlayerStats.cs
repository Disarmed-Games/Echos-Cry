using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Systems")]
    [SerializeField] PlayerMovement _movement;
    [SerializeField] PlayerHealth _health;
    [SerializeField] AbilityManager _abilities;
    [SerializeField] PlayerAttackDamage _playerAttackDamage;
    [SerializeField] PlayerKnockback _playerKnockback;

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
    [SerializeField] EventChannel _dashAttackChannel;
    [SerializeField] EventChannel _knockbackChannel;

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
        if (_dashAttackChannel != null) _dashAttackChannel.Channel += UpgradeDashAttack;
        if (_dashAttackChannel != null) _knockbackChannel.Channel += UpgradeKnockback;
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
        if (_dashAttackChannel != null) _dashAttackChannel.Channel -= UpgradeDashAttack;
        if (_dashAttackChannel != null) _knockbackChannel.Channel -= UpgradeKnockback;
    }

    //Currently using unmutable variables but will eventually change to handle configuration or scaling upgrades eventually
    void UpgradeBaseDamageMultiplier()
    {
        if (_playerAttackDamage != null)
        {
            _playerAttackDamage.BaseDamageMultiplier *= 1.1f;
        }
    }
    void UpgradeDashSpeed()
    {
        if (_movement != null) _movement.DashSpeed *= 1.15f;
    }
    void UpgradeDashCount()
    {
        if (_movement != null)
        {
            _movement.DashCount++;
            _movement.DashMaxCount++;
        }
    }
    void UpgradeDashCooldown()
    {
        if (_movement != null) _movement.DashCooldown *= 0.80f;
    }
    void UpgradeMoveSpeed()
    {
        if (_movement != null) _movement.MoveSpeed *= 1.1f;
    }
    void UpgradeMaxHealth()
    {
        if (_health != null)
        {
            _health.IncreaseMaxHealth(5f);
        }
    }
    void UpgradeMaxArmor()
    {
        if (_health != null)
        {
            _health.IncreaseMaxArmor(5f);
        }
    }
    void UpgradeHealthRegen()
    {
        if (_health != null)
        {
            _health.RegenHealthAmount++;
            _health.EnableHealthRegen();
        }    
    }
    void UpgradeArmorRegen()
    {
        if (_health != null)
        {
            _health.RegenArmorAmount++;
            _health.EnableArmorRegen();
        }
    }
    void UpgradeDashAttack()
    {
        _abilities.AddDashAttack();
    }
    void UpgradeKnockback()
    {
        if (_playerKnockback != null)
        {
            _playerKnockback.BaseKnockbackMultiplier *= 1.15f;
        }
    }
}
