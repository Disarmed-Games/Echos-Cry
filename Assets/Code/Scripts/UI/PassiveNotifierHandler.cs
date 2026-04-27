using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class PassiveNotifierHandler : MonoBehaviour
{
    [SerializeField] private GameObject notifierObject;
    private List<GameObject> ActiveNotifers;
    
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

        foreach (var effect in Player.Instance.ActiveEffects)
        {
            GameObject notifierPrefab = Instantiate(notifierObject, transform);
            notifierPrefab.GetComponent<PassiveNotifier>().SetupNotification(effect);
            ActiveNotifers.Append<GameObject>(notifierPrefab);
        }
    }
}
