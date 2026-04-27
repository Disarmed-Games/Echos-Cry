using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PassiveNotifier : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI _notificationText;
    [SerializeField] private Image _iconImage;
<<<<<<< Updated upstream
    private EffectData currentPassive;
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
    private void UpdateNotification(EffectData effect)
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
            _iconImage.sprite = currentPassive.EffectIcon;
            _notificationText.text = currentPassive.EffectName;
        } 
=======
    private PassiveEffect currentPassive;

    public void SetupNotification(PassiveEffect effect)
    {
        currentPassive = effect;
        _iconImage.enabled = true;
        _iconImage.sprite = currentPassive.effectIcon;
        _notificationText.text = currentPassive.effectName;
    }

    public void DeleteNotification()
    {
        Destroy(gameObject);
>>>>>>> Stashed changes
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentPassive != null)
        {
<<<<<<< Updated upstream
            messageStarted = true;
            UITip.Instance.StartMessage(currentPassive.EffectDescription);
=======
            UITip.Instance.StartMessage(currentPassive.effectDescription);
>>>>>>> Stashed changes
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
<<<<<<< Updated upstream
        messageStarted = false;
=======
>>>>>>> Stashed changes
        UITip.Instance.StopMessage();
    }
}
