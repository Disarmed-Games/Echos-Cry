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
    [SerializeField] private InputTranslator inputTranslator;
    [SerializeField] private GameObject droppedItem;

    private string itemDescription;
    private string itemName;
    private bool itemStackable;
    private InventoryItem slotItem;
    private bool canDrop = false;

    private void Start()
    {
        InventoryManager.Instance.AddInventorySlot(this);
        keyTooltipText.text = useItemInput.action.GetBindingDisplayString(InputBinding.MaskByGroup("KeyboardMouse"));
    }

    private void OnEnable()
    {
        inputTranslator.OnDropEvent += DropItem;
    }
    private void OnDisable()
    {
        inputTranslator.OnDropEvent -= DropItem;
    }

    private void DropItem()
    {
        if (canDrop && slotItem != null)
        {
            droppedItem.GetComponent<DroppedItem>().item = slotItem.data;
            Instantiate(droppedItem, Player.Instance.transform.position + new Vector3(0, 0, -1f), Quaternion.identity);
            InventoryManager.Instance.Remove(slotItem.data);
        }
    }

    public void Set(InventoryItem item)
    {
        slotItem = item;

        if (item == null || item.stackSize < 1)
        {
            slotIcon.enabled = false;
            slotIcon.sprite = null;
            stackAmountText.text = "";
            itemDescription = "";
            return;
        }

        slotIcon.enabled = true;
        slotIcon.sprite = item.data.icon;

        if (item.data.isStackable)
            stackAmountText.text = item.stackSize.ToString();
        else
            stackAmountText.text = "";
        
        itemDescription = item.data.description;
        itemName = item.data.displayName;
        itemStackable = item.data.isStackable;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        canDrop = true;

        if (slotItem != null)
        {
            string stackableText = "";
            if (!itemStackable) stackableText = "This item applies a passive effect.";
            else stackableText = "This item is single use.";
            string dropText = $"Press '{inputTranslator.PlayerInputs.Gameplay.Drop.GetBindingDisplayString(InputBinding.MaskByGroup("KeyboardMouse"))}' to drop.";
            UITip.Instance.StartMessage($"<color=#008080>{itemName}</color>:\n<color=yellow>{stackableText}</color>\n{itemDescription}\n<color=#C0C0C0>{dropText}</color>");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        canDrop = false;

        UITip.Instance.StopMessage();
    }
}