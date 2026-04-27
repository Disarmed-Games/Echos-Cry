using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PassiveNotifier : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI _notificationText;
    [SerializeField] private Image _iconImage;
    private EffectData currentPassive;

    public void SetupNotification(EffectData effect)
    {
        currentPassive = effect;
        _iconImage.enabled = true;
        _iconImage.sprite = currentPassive.EffectIcon;
        _notificationText.text = currentPassive.EffectName;
    }

    public void DeleteNotification()
    {
        Destroy(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentPassive != null)
        {
            UITip.Instance.StartMessage(currentPassive.EffectDescription);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UITip.Instance.StopMessage();
    }
}
