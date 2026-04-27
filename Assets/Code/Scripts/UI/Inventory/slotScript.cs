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

    private string description;
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
            description = "";
            return;
        }

        slotIcon.enabled = true;
        slotIcon.sprite = item.data.icon;

        if (item.data.isStackable)
            stackAmountText.text = item.stackSize.ToString();
        else
            stackAmountText.text = "";
        
        description = item.data.description;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        canDrop = true;

        if (slotItem != null)
        {
            string dropText = $"\nPress '{inputTranslator.PlayerInputs.Gameplay.Drop.GetBindingDisplayString(InputBinding.MaskByGroup("KeyboardMouse"))}' to drop.";
            UITip.Instance.StartMessage(description + dropText);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        canDrop = false;

        UITip.Instance.StopMessage();
    }
}