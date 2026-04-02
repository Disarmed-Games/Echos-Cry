using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ComboProgressNotifier : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI _notificationText;
    [SerializeField] private Image _iconImage;
    private PassiveEffect currentPassive;
    private bool messageStarted = false;

    void Start()
    {
        PlayerComboMeter.OnComboMeterPassiveUnlocked += UpdateNotification;
        UpdateNotification(null);
    }
    void OnDestroy()
    {
        PlayerComboMeter.OnComboMeterPassiveUnlocked -= UpdateNotification;
    }
    private void UpdateNotification(PassiveEffect effect)
    {
        currentPassive = effect;

        if (messageStarted && effect == null)
        {
            UITip.Instance.StopMessage();
            messageStarted = false;
        }

        if (currentPassive == null)
        {
            _iconImage.enabled = false;
            _notificationText.text = "No Passives";
        }
        else
        {
            _iconImage.enabled = true;
            _iconImage.sprite = currentPassive.effectIcon;
            _notificationText.text = currentPassive.effectName;
        } 
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentPassive != null)
        {
            messageStarted = true;
            UITip.Instance.StartMessage(currentPassive.effectDescription);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        messageStarted = false;
        UITip.Instance.StopMessage();
    }
}
