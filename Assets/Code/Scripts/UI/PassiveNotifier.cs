using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PassiveNotifier : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI _notificationText;
    [SerializeField] private TextMeshProUGUI _tierText;
    [SerializeField] private Image _iconImage;
    private bool isActive = false;
    private EffectData currentPassive;

    public void SetupNotification(EffectData effect, bool active)
    {
        if (!active) _iconImage.color = Color.black;
        else _iconImage.color = Color.white;

        isActive = active;
        currentPassive = effect;
        _iconImage.enabled = true;
        _iconImage.sprite = currentPassive.EffectIcon;
        _notificationText.text = currentPassive.EffectName;
        switch (currentPassive.EffectTier)
        {
            case EchosCry.EffectTier.One:
                _tierText.text = "I";
                break;
            case EchosCry.EffectTier.Two:
                _tierText.text = "II";
                break;
            case EchosCry.EffectTier.Three:
                _tierText.text = "III";
                break;
        }
    }

    public void DeleteNotification()
    {
        Destroy(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentPassive != null)
        {
            if (isActive)
                UITip.Instance.StartMessage(currentPassive.EffectDescription, this);
            else
                UITip.Instance.StartMessage($"<color=red>This effect is not currently active.</color>\n{currentPassive.EffectDescription}", this);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UITip.Instance.StopMessage(this);
    }
}
