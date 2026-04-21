using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Stats Configuration File")]
    [SerializeField] private StatsConfig _statsConfig;

    private float _currentHealth;
    private float _currentArmor;
    private float _maxHealth;
    private float _maxArmor;
    private bool _isInvincible = false;
    public float CurrentHealth { get => _currentHealth; }
    public float CurrentArmor { get => _currentArmor; }
    public float MaxHealth { get => _maxHealth; set => _maxHealth = value; }
    public float MaxArmor { get => _maxArmor; set => _maxArmor = value; }

    public bool IsInvincible { get => _isInvincible; set => _isInvincible = value; }

    private void Start()
    {
        _currentHealth = _statsConfig.maxHealth;
        _currentArmor = _statsConfig.maxArmor;
        _maxHealth = _statsConfig.maxHealth;
        _maxArmor = _statsConfig.maxArmor;
    }

    public void ResetSystem()
    {
        _currentHealth = _statsConfig.maxHealth;
        _currentArmor = _statsConfig.maxArmor;
        _maxHealth = _statsConfig.maxHealth;
        _maxArmor = _statsConfig.maxArmor;
    }

    public void Damage(float damage)
    {
        if (_currentArmor > 0)
        {
            DamageArmor(damage);

            if (_currentArmor < 0)
            {
                float healthDamage = _currentArmor;
                _currentArmor = 0;
                DamageHealth(healthDamage);
            }
        }
        else DamageHealth(damage);

        if(_currentHealth < 0) _currentHealth = 0;
    }

    public void HealHealth(float heal)
    {
        _currentHealth += Mathf.Abs(heal);
        if (_currentHealth > _maxHealth) _currentHealth = _maxHealth;
    }
    public void HealArmor(float heal)
    {
        _currentArmor += Mathf.Abs(heal);
        if (_currentArmor > _maxArmor) _currentArmor = _maxArmor;
    }
    public void DamageHealth(float damage)
    {
        _currentHealth -= Mathf.Abs(damage);
    }
    public void DamageArmor(float damage)
    {
        _currentArmor -= Mathf.Abs(damage);
    }

    //TODO: Move somewhere else
    private float _damageMultiplier = 1f;
    public float DamageMultiplier => _damageMultiplier;

    //ISSUE: This should be moved somewhere else, doesn't seem appropriate in HealthSystem, 
    //seems like it would be better in NPC Damageable so that the multiplier can be passed to damage already being done to enemy
    //Either that or having another system that handles debuffs/damage multipliers for enemies or player
    public void SetDamageMultiplier(float multiplier)
    {
        _damageMultiplier = multiplier;
    }
}