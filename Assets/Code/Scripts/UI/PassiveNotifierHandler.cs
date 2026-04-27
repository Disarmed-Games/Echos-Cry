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
    }
    private void OnDisable()
    {
        PlayerComboMeter.OnComboMeterStateChanged -= UpdatePassiveNotifiers;
    }
    
    private void UpdatePassiveNotifiers()
    {
        foreach (var notification in ActiveNotifers)
        {
            notification.GetComponent<PassiveNotifier>().DeleteNotification();
        }
        ActiveNotifers.Clear();

        List<EffectData> effectList = null;

        switch (PlayerComboMeter.CurrentMeterState)
        {
            case PlayerComboMeter.MeterState.Starting:
                effectList = null;
                break;
            case PlayerComboMeter.MeterState.OneThird:
                effectList = Player.Instance.ActiveEffectsTier1;
                break;
            case PlayerComboMeter.MeterState.TwoThirds:
                effectList = Player.Instance.ActiveEffectsTier1;
                effectList.AddRange(Player.Instance.ActiveEffectsTier2);
                break;
            case PlayerComboMeter.MeterState.Full:
                effectList = Player.Instance.ActiveEffectsTier1;
                effectList.AddRange(Player.Instance.ActiveEffectsTier2);
                effectList.AddRange(Player.Instance.ActiveEffectsTier3);
                break;
            default:
                break;
        }

        if (effectList == null) return;
        foreach (var effect in effectList)
        {
            GameObject notifierPrefab = Instantiate(notifierObject, transform);
            notifierPrefab.GetComponent<PassiveNotifier>().SetupNotification(effect);
            ActiveNotifers.Add(notifierPrefab);
        }
    }
}
