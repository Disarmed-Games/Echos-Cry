using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SlotScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image slotIcon;
    [SerializeField] private TextMeshProUGUI stackAmountText;
    [SerializeField] private TextMeshProUGUI keyTooltipText;
    [SerializeField] private InputActionReference useItemInput;
    private string description;

    private void Start()
    {
        InventoryManager.Instance.AddInventorySlot(this);
        keyTooltipText.text = useItemInput.action.GetBindingDisplayString(InputBinding.MaskByGroup("KeyboardMouse"));
    }

    public void Set(InventoryItem item)
    {
        if (item == null || item.stackSize < 1)
        {
            slotIcon.enabled = false;
            slotIcon.sprite = null;
            stackAmountText.text = "";
            description = "";
            return;
        }

        slotIcon.enabled = true;
        slotIcon.sprite = item.data.icon;
        stackAmountText.text = item.stackSize.ToString();
        description = item.data.description;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (description != "")
        {
            UITip.Instance.StartMessage(description);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UITip.Instance.StopMessage();
    }
}