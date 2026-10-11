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
    private GameObject selectionFrame;
    private Coroutine flashRoutine;

    private void Awake()
    {
        if (GetComponent<Selectable>() == null) gameObject.AddComponent<Selectable>();
        var navigation = GetComponent<Selectable>().navigation;
        navigation.mode = Navigation.Mode.None;
        GetComponent<Selectable>().navigation = navigation;
        selectionFrame = new GameObject("Selected Item Slot", typeof(RectTransform), typeof(CanvasRenderer), typeof(ControllerFocusOutline));
        selectionFrame.layer = gameObject.layer;
        var frame = (RectTransform)selectionFrame.transform;
        frame.SetParent(transform, false);
        frame.anchorMin = Vector2.zero;
        frame.anchorMax = Vector2.one;
        frame.offsetMin = new Vector2(-7f, -7f);
        frame.offsetMax = new Vector2(7f, 7f);
        selectionFrame.SetActive(false);
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
        if (inputTranslator.ActiveControlScheme != InputTranslator.ControllerScheme) ClearFlash();
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

    private void FlashSlot(int index)
    {
        ClearFlash();
        if (index != slotIndex) return;
        selectionFrame.SetActive(true);
        flashRoutine = StartCoroutine(HideSlotFlash());
    }

    private IEnumerator HideSlotFlash()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        selectionFrame.SetActive(false);
        flashRoutine = null;
    }

    private void ClearFlash()
    {
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = null;
        if (selectionFrame != null) selectionFrame.SetActive(false);
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
