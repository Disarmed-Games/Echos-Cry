using UnityEngine;
using System.Collections.Generic;
using AudioSystem;
/// Original Author: Abby
/// All Contributors Since Creation: Abby
/// Last Modified By:

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager _instance;
    public static InventoryManager Instance { get { return _instance; } }

    private Dictionary<InventoryItemData, InventoryItem> m_itemDictionary;
    public List<InventoryItem> inventory { get; private set; }  

    [SerializeField] private InventoryDisplay _inventoryDisplay;
    [SerializeField] private InputTranslator _inputTranslator;
    [SerializeField] private soundEffect _useItemSound;
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
    void Start()
    {
        inventory = new List<InventoryItem>();
        m_itemDictionary = new Dictionary<InventoryItemData, InventoryItem>();

        _inputTranslator.OnItem1Event += UseItem;
        _inputTranslator.OnItem2Event += UseItem;
        _inputTranslator.OnItem3Event += UseItem;
        _inputTranslator.OnItem4Event += UseItem;
    }
    void OnDestroy()
    {
        _inputTranslator.OnItem1Event -= UseItem;
        _inputTranslator.OnItem2Event -= UseItem;
        _inputTranslator.OnItem3Event -= UseItem;
        _inputTranslator.OnItem4Event -= UseItem;
    }

    private void UseHealthPotion()
    {
        GameObject playerRef = GameObject.FindWithTag("Player");
        if (playerRef != null)
        {
            playerRef.GetComponent<Player>().Health.HealHealth(15f);
        }
    }
    private void UseShieldPotion()
    {
        GameObject playerRef = GameObject.FindWithTag("Player");
        if (playerRef != null)
        {
            playerRef.GetComponent<Player>().Health.HealArmor(5f);
        }
    }

    public void AddInventorySlot(SlotScript _slotScript)
    {
        for (int i = 0; i < _inventoryDisplay.slotScriptArray.Length; i++)
        {
            if (_inventoryDisplay.slotScriptArray[i] == null)
            {
                _inventoryDisplay.slotScriptArray[i] = _slotScript;
                return;
            }
        }
    }
    private void UseItem(int index)
    {
        if (inventory.Count <= 0 || inventory.Count <= index) return;

        SoundEffectManager.Instance.Builder
                .SetSound(_useItemSound)
                .SetSoundPosition(PlayerRef.Transform.position)
                .ValidateAndPlaySound();

        InventoryItem usedItem = inventory[index];
        if (usedItem == null || usedItem.data == null) return;

        if (usedItem.data.id == "health")
        {
            UseHealthPotion();
        }
        else if (usedItem.data.id == "shield")
        {
            UseShieldPotion();
        }
        Remove(usedItem.data);
    }

    public InventoryItem Get(InventoryItemData referenceData)
    {
        if (m_itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            return value;
        }
        return null;
    }
    public void Add(InventoryItemData referenceData)
    {
        if (m_itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            value.AddToStack();
        } 
        else 
        {
            InventoryItem newItem = new InventoryItem(referenceData);
            inventory.Add(newItem);
            m_itemDictionary.Add(referenceData, newItem);
        }
        _inventoryDisplay.UpdateInventory();
    }
    public void Remove(InventoryItemData referenceData)
    {
        if (m_itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            value.RemoveFromStack();
            if (value.stackSize == 0)
            {
                inventory.Remove(value);
                m_itemDictionary.Remove(referenceData);
            }
            _inventoryDisplay.UpdateInventory();
        }
    }
}
public class InventoryItem
{
    public InventoryItemData data {get; private set; }
    public int stackSize {get; private set;}

    public InventoryItem(InventoryItemData source)
    {
        data = source;
        AddToStack();
    }

    public void AddToStack()
    {
        stackSize++;
    }

    public void RemoveFromStack()
    {
        stackSize--;
    }
}
