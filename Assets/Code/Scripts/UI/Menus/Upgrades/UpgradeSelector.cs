using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _highlight;
    [SerializeField] private TextMeshProUGUI _upgradeNameText;
    [SerializeField] private TextMeshProUGUI _upgradeDescriptionText;
    [SerializeField] private Button _useButton;

    private bool isActive = true;
    private UpgradeManager.UpgradeType currentUpgrade;

    public void SetUpgrade(UpgradeManager.UpgradeType upgrade)
    {
        currentUpgrade = upgrade;
        _upgradeNameText.text = currentUpgrade.ToString();
    }

    public void UseUpgrade()
    {
        UpgradeManager.Instance.ApplyUpgrade(currentUpgrade);
        UpgradeManager.Instance.Roll();
    }

    public void SetSelectorState(bool state)
    {
        isActive = state;

        _useButton.interactable = isActive;
        if (!isActive)
        {
            _upgradeNameText.text = "???";
            _upgradeDescriptionText.text = "You need to level up before you can view these upgrades!";
        }
        else
        {
            _upgradeNameText.text = currentUpgrade.ToString().Replace("_", " ");
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        _highlight.SetActive(true);
        if (isActive)
        { 
            _upgradeDescriptionText.text = UpgradeManager.Instance.GetDescription(currentUpgrade);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _highlight.SetActive(false);
        if (isActive)
        {
            _upgradeDescriptionText.text = "Hover over an upgrade to see the improvements!";
        }
    }
}
