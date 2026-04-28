using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class PassiveNotifierHandler : MonoBehaviour
{
    [SerializeField] private GameObject notifierObject;
    private List<GameObject> ActiveNotifers = new();
    
    private void OnEnable()
    {
        PlayerComboMeter.OnComboMeterStateChanged += UpdatePassiveNotifiers;
        WeaponHolder.OnEffectAdded += UpdatePassiveNotifiers;
    }
    private void OnDisable()
    {
        PlayerComboMeter.OnComboMeterStateChanged -= UpdatePassiveNotifiers;
        WeaponHolder.OnEffectAdded -= UpdatePassiveNotifiers;
    }
    
    private void UpdatePassiveNotifiers()
    {
        foreach (var notification in ActiveNotifers)
        {
            notification.GetComponent<PassiveNotifier>().DeleteNotification();
        }
        ActiveNotifers.Clear();

        List<(EffectData effect, bool enabled)> effectList = new();

        void AddTier(List<EffectData> tier, bool enabled)
        {
            foreach (var effect in tier)
                effectList.Add((effect, enabled));
        }

        switch (PlayerComboMeter.CurrentMeterState)
        {
            case PlayerComboMeter.MeterState.Starting:
                AddTier(Player.Instance.ActiveEffectsTier1, false);
                AddTier(Player.Instance.ActiveEffectsTier2, false);
                AddTier(Player.Instance.ActiveEffectsTier3, false);
                break;
            case PlayerComboMeter.MeterState.OneThird:
                AddTier(Player.Instance.ActiveEffectsTier1, true);
                AddTier(Player.Instance.ActiveEffectsTier2, false);
                AddTier(Player.Instance.ActiveEffectsTier3, false);
                break;
            case PlayerComboMeter.MeterState.TwoThirds:
                AddTier(Player.Instance.ActiveEffectsTier1, true);
                AddTier(Player.Instance.ActiveEffectsTier2, true);
                AddTier(Player.Instance.ActiveEffectsTier3, false);
                break;
            case PlayerComboMeter.MeterState.Full:
                AddTier(Player.Instance.ActiveEffectsTier1, true);
                AddTier(Player.Instance.ActiveEffectsTier2, true);
                AddTier(Player.Instance.ActiveEffectsTier3, true);
                break;
            default:
                break;
        }

        foreach (var effect in effectList)
        {
            GameObject notifierPrefab = Instantiate(notifierObject, transform);
            notifierPrefab.GetComponent<PassiveNotifier>().SetupNotification(effect.effect, effect.enabled);
            ActiveNotifers.Add(notifierPrefab);
        }
    }
}
