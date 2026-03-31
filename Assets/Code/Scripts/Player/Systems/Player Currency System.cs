using System;
using UnityEngine;


public class PlayerCurrencySystem : Singleton<PlayerCurrencySystem>
{

    public void IncrementGoldCurrency(int amount)
    {
        _goldCurrency += amount;
        OnCurrencyChangeEvent?.Invoke(_goldCurrency);
    }
    public void DecrementGoldCurrency(int amount)
    {
        _goldCurrency -= amount;
        OnCurrencyChangeEvent?.Invoke(_goldCurrency);
    }

    public void SetGoldCurrency(int amount)
    {
        _goldCurrency = amount;
        OnCurrencyChangeEvent?.Invoke(_goldCurrency);
    }

    public int GetGoldCurrency()
    {
        return _goldCurrency;
    }

    private void Start()
    {
        if(_playerCurrencyConfig == null)
        {
            Debug.LogWarning("Player Currency Configuration file is null");
            return;
        }
        _goldCurrency = _playerCurrencyConfig.StartingGoldCurrency;
        OnCurrencyChangeEvent?.Invoke(_goldCurrency);
    }

    [Header("Configuration Object")]
    [SerializeField] private PlayerCurrencyConfig _playerCurrencyConfig;

    private int _goldCurrency;
    public int GoldCurrency { get { return _goldCurrency; } }

    public static event Action<int> OnCurrencyChangeEvent;
}
