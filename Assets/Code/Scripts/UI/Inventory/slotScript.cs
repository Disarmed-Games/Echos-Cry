using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class SlotScript : MonoBehaviour
{
    [SerializeField] private Image m_icon;
    [SerializeField] private GameObject m_stackObj;
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
        if (item == null||item.stackSize < 1)
        {
            m_stackObj = null;
            m_num.text = "0";
            m_icon.sprite = null;
            m_icon.enabled = false;
            return;
        }
        else
        {
            m_icon.enabled = true;
            m_icon.sprite = item.data.icon;
            m_stackObj = item.data.prefab;
        }
    
        m_num.text = item.stackSize.ToString();
    }
}