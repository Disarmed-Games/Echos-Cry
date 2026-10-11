using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SlotScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private Image slotIcon;
    [SerializeField] private TextMeshProUGUI stackAmountText;
    [SerializeField] private TextMeshProUGUI keyTooltipText;
    [SerializeField] private InputActionReference useItemInput;
    [SerializeField] private InputTranslator inputTranslator;
    [SerializeField] private GameObject droppedItem;
    [SerializeField] private int slotIndex;

    private string itemDescription;
    private string itemName;
    private bool itemStackable;
    private InventoryItem slotItem;
    private bool canDrop = false;

    private void Awake()
    {
        if (GetComponent<Selectable>() == null) gameObject.AddComponent<Selectable>();
    }

    private void Start()
    {
        InventoryManager.Instance.AddInventorySlot(this);
        UpdateBinding();
    }

    private void OnEnable()
    {
        inputTranslator.OnDropEvent += DropItem;
        inputTranslator.OnBindingsChanged += UpdateBinding;
        UpdateBinding();
    }
    private void OnDisable()
    {
        inputTranslator.OnDropEvent -= DropItem;
        inputTranslator.OnBindingsChanged -= UpdateBinding;
        canDrop = false;
        if (UITip.Instance != null) UITip.Instance.StopMessage(this);
    }

    private void UpdateBinding()
    {
        string updatedKey = "";
        switch (slotIndex)
        {
            case 0:
                updatedKey = inputTranslator.GetBindingDisplayString(inputTranslator.PlayerInputs.Gameplay.Item1);
                break;
            case 1:
                updatedKey = inputTranslator.GetBindingDisplayString(inputTranslator.PlayerInputs.Gameplay.Item2);
                break;
            case 2:
                updatedKey = inputTranslator.GetBindingDisplayString(inputTranslator.PlayerInputs.Gameplay.Item3);
                break;
            case 3:
                updatedKey = inputTranslator.GetBindingDisplayString(inputTranslator.PlayerInputs.Gameplay.Item4);
                break;
        }
        keyTooltipText.text = updatedKey;
        if (canDrop && slotItem != null && UITip.Instance != null)
            UITip.Instance.UpdateMessage(GetItemMessage(), this);
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
            UITip.Instance.StartMessage(GetItemMessage(), this);
        }
    }

    private string GetItemMessage()
    {
        string stackableText = itemStackable ? "This item is single use." : "This item applies a passive effect.";
        string dropText = $"Press '{inputTranslator.GetBindingDisplayString(inputTranslator.PlayerInputs.Gameplay.Drop)}' to drop.";
        return $"<color=#008080>{itemName}</color>:\n<color=yellow>{stackableText}</color>\n{itemDescription}\n<color=#C0C0C0>{dropText}</color>";
    }

    public void OnSelect(BaseEventData eventData)
    {
        canDrop = true;
        if (slotItem != null) UITip.Instance.StartMessage(GetItemMessage(), this, transform);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        OnPointerExit(null);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        canDrop = false;

        UITip.Instance.StopMessage(this);
    }
}
