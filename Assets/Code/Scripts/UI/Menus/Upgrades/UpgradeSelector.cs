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
    private bool pointerHovered;
    private bool controllerFocused;
    private UpgradeManager.UpgradeType currentUpgrade;
    public Button UseButton => _useButton;
    public TextMeshProUGUI DescriptionText => _upgradeDescriptionText;
    public bool IsPresentationFocused
    {
        get
        {
            var menu = GetComponentInParent<UpgradeMenuNavigation>();
            return menu != null ? (menu.IsUsingController ? controllerFocused : pointerHovered) :
                pointerHovered || controllerFocused;
        }
    }
    public string Description => !isActive ? "You need to level up before you can view these upgrades!" :
        IsPresentationFocused && UpgradeManager.Instance != null ? UpgradeManager.Instance.GetDescription(currentUpgrade) :
        "Select an upgrade to see the improvements!";

    public void SetUpgrade(UpgradeManager.UpgradeType upgrade)
    {
        currentUpgrade = upgrade;
        _upgradeNameText.text = currentUpgrade.ToString();
        RefreshPresentation();
    }

    public void UseUpgrade()
    {
        if (!isActive || UpgradeManager.Instance == null || UpgradeManager.Instance.GetAvailablePoints() <= 0) return;
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
        }
        else
        {
            _upgradeNameText.text = currentUpgrade.ToString().Replace("_", " ");
        }
        RefreshPresentation();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerHovered = true;
        RefreshPresentation();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerHovered = false;
        RefreshPresentation();
    }

    public void SetControllerFocus(bool focused)
    {
        if (controllerFocused == focused) return;
        controllerFocused = focused;
        RefreshPresentation();
    }

    private void RefreshPresentation()
    {
        bool focused = IsPresentationFocused;
        _highlight.SetActive(focused);
        var menu = GetComponentInParent<UpgradeMenuNavigation>();
        if (menu != null && menu.isActiveAndEnabled) menu.RefreshDescriptions();
        else _upgradeDescriptionText.text = Description;
    }

    private void OnDisable()
    {
        pointerHovered = controllerFocused = false;
        RefreshPresentation();
    }
}
