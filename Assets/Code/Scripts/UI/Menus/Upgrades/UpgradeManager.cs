using System;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private UpgradeSelector[] _upgradeSelectors;
    [SerializeField] private TextMeshProUGUI _currentLevelText;
    [SerializeField] private TextMeshProUGUI _xpRequiredText;
    [SerializeField] private TextMeshProUGUI _pointsAvailableText;
    [SerializeField] private FloatFloatIntEventChannel _playerXPChannel;
    [SerializeField] private IntEventChannel _playerLevelUpChannel;

    [Header("Event Channels (Broadcasters)")]
    [SerializeField] EventChannel _upgradeDamageChannel;
    [SerializeField] EventChannel _moveSpeedChannel;
    [SerializeField] EventChannel _dashSpeedChannel;
    [SerializeField] EventChannel _healthChannel;
    [SerializeField] EventChannel _armorChannel;
    [SerializeField] EventChannel _dashCountChannel;
    [SerializeField] EventChannel _dashCooldownChannel;
    [SerializeField] EventChannel _knockbackChannel;

    private int availablePoints = 0;
    public enum UpgradeType
    { //TODO: Add damage muiltiplier upgrades!
        Haste,
        Dash_Speed,
        Health,
        Defense,
        Dash_Count,
        Stamina,
        Strength,
        Knockback_Power,
    }

    private Dictionary<UpgradeType, string> _upgradeDescriptions = new Dictionary<UpgradeType, string>
    { //TODO: Use rich text to make this way less boring to look at!
        //[UpgradeType.] = "",
        [UpgradeType.Haste] = "Increase your base movement speed by 10%.",
        [UpgradeType.Dash_Speed] = "Increase the speed of your dashing by 15%.",
        [UpgradeType.Health] = "Increase your base health by +5 hp.",
        [UpgradeType.Defense] = "Increase your base armor by +5.",
        [UpgradeType.Dash_Count] = "Increase the total amount of dashes available by +1.",
        [UpgradeType.Stamina] = "Decrease the dash meter cooldown time by 20%.",
        [UpgradeType.Strength] = "Increase the base damage multiplier by 10%.",
        [UpgradeType.Knockback_Power] = "Increase the knockback force from attacks by 15%."
    };

    public static UpgradeManager Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        Roll();
    }
    private void OnEnable()
    {
        if(_playerXPChannel != null) _playerXPChannel.Channel += UpdateInfo;
        if(_playerLevelUpChannel != null) _playerLevelUpChannel.Channel += AddLevelPoint;


    }
    private void OnDisable()
    {
        if (_playerXPChannel != null) _playerXPChannel.Channel -= UpdateInfo;
        if (_playerLevelUpChannel != null) _playerLevelUpChannel.Channel -= AddLevelPoint;
    }


    public string GetDescription(UpgradeType upgrade)
    {
        return _upgradeDescriptions.GetValueOrDefault(upgrade);
    }

    public int GetAvailablePoints()
    {
        return availablePoints;
    }

    private void UpdateInfo(float xp, float goalXP, int currentLevel)
    {
        _xpRequiredText.text = $"[{xp} / {goalXP}] XP Till Level {currentLevel + 1}";
        _currentLevelText.text = $"Current Level: {currentLevel}";
        UpdateSelectors();
    }

    private void AddLevelPoint(int level)
    {
        availablePoints++;
        UpdateSelectors();
    }

    private void UpdateSelectors()
    {
        bool canUpgrade = availablePoints > 0;

        _pointsAvailableText.text = $"{availablePoints} Upgrade Points to Spend";
        if (canUpgrade)
            _pointsAvailableText.color = Color.white;
        else
            _pointsAvailableText.color = Color.red;

            foreach (UpgradeSelector selector in _upgradeSelectors)
            {
                selector.SetSelectorState(canUpgrade);
            }
    }

    public void Roll()
    {
        Array upgradeValues = Enum.GetValues(typeof(UpgradeType));
        List<UpgradeType> currentUpgrades = new List<UpgradeType>();

        for (int i = 0; i < _upgradeSelectors.Length; i++)
        {
            UpgradeType randomUpgrade;

            do
            {
                int randIndex = UnityEngine.Random.Range(0, upgradeValues.Length);
                randomUpgrade = (UpgradeType)upgradeValues.GetValue(randIndex);
            }
            while (currentUpgrades.Contains(randomUpgrade));

            currentUpgrades.Add(randomUpgrade);
        }

        int count = 0;
        foreach (UpgradeType upgrade in currentUpgrades)
        {
            _upgradeSelectors[count].SetUpgrade(upgrade);
            count++;
        }

        UpdateSelectors();
    }

    //Make this event-based using the void event channels for each event. The events can then be bound to the PlayerStats class
    public void ApplyUpgrade(UpgradeType upgradeType)
    {
        availablePoints--;

        switch (upgradeType)
        {
            case UpgradeType.Haste:
                if (_moveSpeedChannel != null) _moveSpeedChannel.Invoke();
                break;
            case UpgradeType.Dash_Speed:
                if (_dashSpeedChannel != null) _dashSpeedChannel.Invoke();
                break;
            case UpgradeType.Health:
                if(_healthChannel != null) _healthChannel.Invoke();
                break;
            case UpgradeType.Defense:
                if(_armorChannel != null) _armorChannel.Invoke();
                break;
            case UpgradeType.Dash_Count:
                if (_dashCountChannel != null) _dashCountChannel.Invoke();
                break;
            case UpgradeType.Stamina:
                if (_dashCooldownChannel != null) _dashCooldownChannel.Invoke();
                break;
            case UpgradeType.Strength:
                if (_upgradeDamageChannel != null) _upgradeDamageChannel.Invoke();
                break;
            case UpgradeType.Knockback_Power:
                if (_knockbackChannel != null) _knockbackChannel.Invoke();
                break;
            default:
                Debug.LogWarning("Unknown upgrade type.");
                break;
        }
    }
}
