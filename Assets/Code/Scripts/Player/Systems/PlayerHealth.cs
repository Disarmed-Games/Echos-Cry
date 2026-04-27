using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private HealthSystem _healthSystem;

    public bool HasArmor => _healthSystem.CurrentArmor > 0;
    public bool HasHealth => _healthSystem.CurrentHealth > 0;

    public float CurrentHealth => _healthSystem.CurrentHealth;
    public float CurrentArmor => _healthSystem.CurrentArmor;
    public float MaxHealth => _healthSystem.MaxHealth;
    public float MaxArmor => _healthSystem.MaxArmor;
    public bool IsInvincible { get => _healthSystem.IsInvincible; set => _healthSystem.IsInvincible = value; }

    [Header("(Player Only) Event Channels")]
    [SerializeField] DoubleFloatEventChannel _healthChannel;
    [SerializeField] DoubleFloatEventChannel _armorChannel;

    private float _regenHealthTickTime = 1.5f;
    private float _regenHealthAmount = 2f;
    private float _regenHealthDuration = 30f;
    private bool _canRegenHealth = false;
    private Coroutine _regenHealthTickCoroutine;
    public float RegenHealthAmount { get => _regenHealthAmount; set => _regenHealthAmount = value; }

    private float _regenArmorTickTime = 2f;
    private float _regenArmorAmount = 1f;
    private float _regenArmorDuration = 20f;
    private bool _canRegenArmor = false;
    private Coroutine _regenArmorTickCoroutine;
    public float RegenArmorAmount { get => _regenArmorAmount; set => _regenArmorAmount = value; }

    private void OnEnable()
    {
        //These are called to update the starting values of each bar.
        if (_healthChannel != null)
            _healthChannel.Invoke(_healthSystem.CurrentHealth, _healthSystem.MaxHealth);
        if (_armorChannel != null)
            _armorChannel.Invoke(_healthSystem.CurrentArmor, _healthSystem.MaxArmor);
    }

    public void HealFullHealthAndArmor()
    {
        HealArmor(_healthSystem.MaxArmor);
        HealHealth(_healthSystem.MaxHealth);
    }

    public void ResetHealth()
    {
        _healthSystem.ResetSystem();
        _healthChannel.Invoke(_healthSystem.CurrentHealth, _healthSystem.MaxHealth);
        _armorChannel.Invoke(_healthSystem.CurrentArmor, _healthSystem.MaxArmor);
    }

    /// <summary>
    /// MAX HEALTH INCREASES
    /// </summary>
    public void IncreaseMaxHealth(float amount)
    {
        _healthSystem.MaxHealth += amount;
        _healthSystem.HealHealth(amount);
        _healthChannel.Invoke(_healthSystem.CurrentHealth, _healthSystem.MaxHealth);
    }
    public void IncreaseMaxArmor(float amount)
    {
        _healthSystem.MaxArmor += amount;
        _healthSystem.HealArmor(amount);
        _armorChannel.Invoke(_healthSystem.CurrentArmor, _healthSystem.MaxArmor);
    }

    /// <summary>
    /// HEALTH DAMAGE AND HEAL
    /// </summary>
    public void Damage(float damage)
    {
        _healthSystem.Damage(damage);

        if (DamageLabelManager.Instance != null && DamageLabelManager.Instance.isActiveAndEnabled)
            DamageLabelManager.Instance.SpawnPopup(damage, Player.Instance.transform.position, Color.red);

        _armorChannel.Invoke(_healthSystem.CurrentArmor, _healthSystem.MaxArmor); 
        _healthChannel.Invoke(_healthSystem.CurrentHealth, _healthSystem.MaxHealth);
    }
    public void HealHealth(float amount)
    {
        //Debug.Log($"Healing health by amount: ${amount}");
        //Debug.Log($"invoking current health as: {_healthSystem.CurrentHealth} and max health as: {_healthSystem.MaxHealth}");

        if (DamageLabelManager.Instance != null && DamageLabelManager.Instance.isActiveAndEnabled)
            DamageLabelManager.Instance.SpawnPopup(amount, Player.Instance.transform.position, Color.green);

        _healthSystem.HealHealth(amount);
        _healthChannel.Invoke(_healthSystem.CurrentHealth, _healthSystem.MaxHealth);
    }
    public void HealArmor(float amount)
    {
        //Debug.Log($"Healing armor by amount: ${amount}");
        //Debug.Log($"invoking current armor as: {_healthSystem.CurrentArmor} and max armor as: {_healthSystem.MaxArmor}");
        if (DamageLabelManager.Instance != null && DamageLabelManager.Instance.isActiveAndEnabled)
            DamageLabelManager.Instance.SpawnPopup(amount, Player.Instance.transform.position, Color.blue);

        _healthSystem.HealArmor(amount);
        _armorChannel.Invoke(_healthSystem.CurrentArmor, _healthSystem.MaxArmor);
    }

    /// <summary>
    /// HEALTH REGEN
    /// </summary>
    public void EnableHealthRegen()
    {
        if (_canRegenHealth) return;
        _canRegenHealth = true;
        _regenHealthTickCoroutine = StartCoroutine(RegenTickRate());
        StartCoroutine(DisableHealthRegenTimer());
    }
    private IEnumerator DisableHealthRegenTimer()
    {
        yield return new WaitForSeconds(_regenHealthDuration);
        _canRegenHealth = false;
        StopCoroutine(_regenHealthTickCoroutine);
    }
    private IEnumerator RegenTickRate()
    {
        while (_canRegenHealth)
        {
            yield return new WaitForSeconds(_regenHealthTickTime);

            if (_canRegenHealth && CurrentHealth < MaxHealth)
            {
                HealHealth(_regenHealthAmount);
            }   
        }
    }

    /// <summary>
    /// SHIELD REGEN
    /// </summary>
    public void EnableArmorRegen()
    {
        if (_canRegenArmor) return;
        _canRegenArmor = true;
        _regenArmorTickCoroutine = StartCoroutine(RegenArmorTickRate());
        StartCoroutine(DisableArmorRegenTimer());
    }
    private IEnumerator DisableArmorRegenTimer()
    {
        yield return new WaitForSeconds(_regenArmorDuration);
        _canRegenArmor = false;
        StopCoroutine(_regenArmorTickCoroutine);
    }
    private IEnumerator RegenArmorTickRate()
    {
        while (_canRegenArmor)
        {
            yield return new WaitForSeconds(_regenArmorTickTime);

            if (_canRegenArmor && CurrentArmor < MaxArmor)
            {
                HealArmor(_regenArmorAmount);
            }
        }
    }
    public HealthSystem GetPlayerHealthSystemRef()
    {
        return _healthSystem;
    }
}
