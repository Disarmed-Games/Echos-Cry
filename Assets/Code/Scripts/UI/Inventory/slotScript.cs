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
    [SerializeField] private Color selectedSlotColor = new Color(0.65f, 0.85f, 1f, 1f);
    private Image slotBackground;
    private Color originalSlotColor;
    private bool controllerSelected;

    private void Awake()
    {
        if (GetComponent<Selectable>() == null) gameObject.AddComponent<Selectable>();
        var navigation = GetComponent<Selectable>().navigation;
        navigation.mode = Navigation.Mode.None;
        GetComponent<Selectable>().navigation = navigation;
        slotBackground = GetComponent<Image>();
        if (slotBackground != null) originalSlotColor = slotBackground.color;
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
        inputTranslator.OnItemSlotUsedEvent += FlashSlot;
        UpdateBinding();
    }
    private void OnDisable()
    {
        inputTranslator.OnDropEvent -= DropItem;
        inputTranslator.OnBindingsChanged -= UpdateBinding;
        inputTranslator.OnItemSlotUsedEvent -= FlashSlot;
        ClearFlash();
        canDrop = false;
        if (UITip.Instance != null) UITip.Instance.StopMessage(this);
    }

    private void UpdateBinding()
    {
        RefreshControllerSelection();
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
        if (inputTranslator.ActiveControlScheme == InputTranslator.ControllerScheme)
        {
            var apply = inputTranslator.PlayerInputs.Gameplay.Get().FindAction("Use Selected Item");
            updatedKey += " / " + inputTranslator.GetBindingDisplayString(apply);
        }
        keyTooltipText.text = updatedKey;
        if (canDrop && slotItem != null && UITip.Instance != null)
            UITip.Instance.UpdateMessage(GetItemMessage(), this);
    }

    private void FlashSlot(int index)
    {
        RefreshControllerSelection();
    }

    private void RefreshControllerSelection()
    {
        bool wasSelected = controllerSelected;
        controllerSelected = inputTranslator.ActiveControlScheme == InputTranslator.ControllerScheme
            && inputTranslator.SelectedItemSlot == slotIndex;
        if (slotBackground != null)
        {
            Color tint = controllerSelected ? selectedSlotColor : originalSlotColor;
            tint.a = originalSlotColor.a;
            slotBackground.color = tint;
        }
        if (UITip.Instance == null) return;
        if (controllerSelected && slotItem != null && slotItem.stackSize > 0)
            UITip.Instance.StartMessage(GetItemMessage(), this, transform);
        else if (wasSelected || controllerSelected)
            UITip.Instance.StopMessage(this);
    }

    private void ClearFlash()
    {
        controllerSelected = false;
        if (slotBackground != null) slotBackground.color = originalSlotColor;
    }

    private void DropItem()
    {
        bool controller = inputTranslator.ActiveControlScheme == InputTranslator.ControllerScheme;
        if ((controller ? inputTranslator.SelectedItemSlot == slotIndex : canDrop) && slotItem != null)
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
            RefreshControllerSelection();
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
        RefreshControllerSelection();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (inputTranslator.ActiveControlScheme == InputTranslator.ControllerScheme) return;
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
        if (controllerSelected) return;
        UITip.Instance.StopMessage(this);
    }
}
