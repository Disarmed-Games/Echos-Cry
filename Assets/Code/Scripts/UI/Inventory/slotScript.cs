using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class SlotScript : MonoBehaviour
{
    [SerializeField] private Image m_icon;
    [SerializeField] private TextMeshProUGUI m_num;
    [SerializeField] private TextMeshProUGUI keyTooltipText;
    [SerializeField] private InputActionReference useItemInput;

    private void Start()
    {
        InventoryManager.Instance.AddInventorySlot(this);
        keyTooltipText.text = useItemInput.action.GetBindingDisplayString(InputBinding.MaskByGroup("KeyboardMouse"));
    }

    public void Set(InventoryItem item)
    {
        if (item == null || item.stackSize < 1)
        {
            m_icon.enabled = false;
            m_icon.sprite = null;
            m_num.text = "";
            return;
        }

        m_icon.enabled = true;
        m_icon.sprite = item.data.icon;
        m_num.text = item.stackSize.ToString();
    }
}