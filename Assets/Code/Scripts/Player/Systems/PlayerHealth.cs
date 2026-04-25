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

    private float _regenHealthCooldown = 5f;
    private float _regenHealthTickTime = 10f;
    private float _regenHealthAmount = 0f;
    private bool _canRegenHealth = false;
    private Coroutine _regenHealthTickCoroutine;
    public float RegenHealthAmount { get => _regenHealthAmount; set => _regenHealthAmount = value; }

    private float _regenArmorCooldown = 5f;
    private float _regenArmorTickTime = 10f;
    private float _regenArmorAmount = 0f;
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
            DamageLabelManager.Instance.SpawnPopup(damage, PlayerRef.Transform.position, Color.red);

        PauseHealthRegen();

        _armorChannel.Invoke(_healthSystem.CurrentArmor, _healthSystem.MaxArmor); 
        _healthChannel.Invoke(_healthSystem.CurrentHealth, _healthSystem.MaxHealth);
    }
    public void HealHealth(float amount)
    {
        //Debug.Log($"Healing health by amount: ${amount}");
        //Debug.Log($"invoking current health as: {_healthSystem.CurrentHealth} and max health as: {_healthSystem.MaxHealth}");

        if (DamageLabelManager.Instance != null && DamageLabelManager.Instance.isActiveAndEnabled)
            DamageLabelManager.Instance.SpawnPopup(amount, PlayerRef.Transform.position, Color.green);

        _healthSystem.HealHealth(amount);
        _healthChannel.Invoke(_healthSystem.CurrentHealth, _healthSystem.MaxHealth);
    }
    public void HealArmor(float amount)
    {
        //Debug.Log($"Healing armor by amount: ${amount}");
        //Debug.Log($"invoking current armor as: {_healthSystem.CurrentArmor} and max armor as: {_healthSystem.MaxArmor}");
        if (DamageLabelManager.Instance != null && DamageLabelManager.Instance.isActiveAndEnabled)
            DamageLabelManager.Instance.SpawnPopup(amount, PlayerRef.Transform.position, Color.blue);

        _healthSystem.HealArmor(amount);
        _armorChannel.Invoke(_healthSystem.CurrentArmor, _healthSystem.MaxArmor);
    }

    /// <summary>
    /// HEALTH REGEN
    /// </summary>
    public void EnableHealthRegen()
    {
        _canRegenHealth = true;
        _regenHealthTickCoroutine = StartCoroutine(RegenTickRate());
    }
    public void PauseHealthRegen()
    {
        _canRegenHealth = false;
        if (_regenHealthTickCoroutine != null)
        {
            StopCoroutine(_regenHealthTickCoroutine);
            StartCoroutine(RegenCooldown());
        }
    }
    private IEnumerator RegenCooldown() //After taking damage, how long should pass till health can regen once more.
    {
        yield return new WaitForSeconds(_regenHealthCooldown);
        EnableHealthRegen();
    }
    private IEnumerator RegenTickRate()
    {
        while (_canRegenHealth)
        {
            yield return new WaitForSeconds(_regenHealthTickTime);

            if (_canRegenHealth && CurrentHealth < MaxHealth)
            {
                HealHealth(_regenHealthAmount);
                _healthChannel.Invoke(_healthSystem.CurrentHealth, _healthSystem.MaxHealth);
            }   
        }
    }

    /// <summary>
    /// SHIELD REGEN
    /// </summary>
    public void EnableArmorRegen()
    {
        _canRegenArmor = true;
        _regenArmorTickCoroutine = StartCoroutine(RegenArmorTickRate());
    }
    public void PauseArmorRegen()
    {
        _canRegenArmor = false;
        if (_regenArmorTickCoroutine != null)
        {
            StopCoroutine(_regenArmorTickCoroutine);
            StartCoroutine(RegenArmorCooldown());
        }
    }
    private IEnumerator RegenArmorCooldown() //After taking damage, how long should pass till health can regen once more.
    {
        yield return new WaitForSeconds(_regenArmorCooldown);
        EnableArmorRegen();
    }
    private IEnumerator RegenArmorTickRate()
    {
        while (_canRegenArmor)
        {
            yield return new WaitForSeconds(_regenArmorTickTime);

            if (_canRegenArmor && CurrentArmor < MaxArmor)
            {
                HealArmor(_regenArmorAmount);
                _armorChannel.Invoke(_healthSystem.CurrentArmor, _healthSystem.MaxArmor);
            }
        }
    }

    public HealthSystem GetPlayerHealthSystemRef()
    {
        return _healthSystem;
    }
}
