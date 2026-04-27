using Ink.Parsed;
using PlasticPipe.PlasticProtocol.Messages;
using System.Linq;
using UnityEngine;

public class PassiveNotifierHandler : MonoBehaviour
{
    [SerializeField] private GameObject notifierObject;
    private GameObject[] ActiveNotifers;
    
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

        /*
        foreach (var newNotification in GetActivePassivesList)
        {
            GameObject notifierPrefab = Instantiate(notifierObject, transform);
            notifierPrefab.GetComponent<PassiveNotifier>().SetupNotification(newNotification.effect);
            ActiveNotifers.Append<GameObject>(notifierPrefab);
        }
        */
    }
}
