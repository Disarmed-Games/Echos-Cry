using AudioSystem;
using System.Collections.Generic;
using UnityEngine;

/// Original Author: Abby
/// All Contributors Since Creation: Abby, Andrew

public class InventoryManager : Singleton<InventoryManager>
{
    private Dictionary<InventoryItemData, InventoryItem> itemDictionary;
    public List<InventoryItem> inventoryList { get; private set; }  

    [SerializeField] private InventoryDisplay _inventoryDisplay;
    [SerializeField] private InputTranslator _inputTranslator;
    [SerializeField] private soundEffect _useItemSound;
    [SerializeField] private soundEffect _invalidItemSound;

    private Player _player;

    void Start()
    {
        inventoryList = new List<InventoryItem>();
        itemDictionary = new Dictionary<InventoryItemData, InventoryItem>();
        _player = GameObject.FindWithTag("Player")?.GetComponent<Player>();
    }

    private void OnEnable()
    {
        _inputTranslator.OnItem1Event += UseItem;
        _inputTranslator.OnItem2Event += UseItem;
        _inputTranslator.OnItem3Event += UseItem;
        _inputTranslator.OnItem4Event += UseItem;
    }
    private void OnDisable()
    {
        _inputTranslator.OnItem1Event -= UseItem;
        _inputTranslator.OnItem2Event -= UseItem;
        _inputTranslator.OnItem3Event -= UseItem;
        _inputTranslator.OnItem4Event -= UseItem;
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
        if (inventoryList.Count <= 0 || inventoryList.Count <= index) return;

        InventoryItem usedItem = inventoryList[index];

        if (usedItem.data.CanUse(_player))
        {
            EchosCry.Sound.PlaySFX(_useItemSound, PlayerRef.Transform, 0);
            usedItem.data.Use(_player);
            Remove(usedItem.data);
        }
        else
            EchosCry.Sound.PlaySFX(_invalidItemSound, PlayerRef.Transform, 0);
    }

    public bool IsFull(InventoryItemData referenceData)
    {
        if (referenceData.isStackable && itemDictionary.ContainsKey(referenceData))
            return false;
        else
            return inventoryList.Count >= _inventoryDisplay.slotScriptArray.Length; 
    }
    public InventoryItem Get(InventoryItemData referenceData)
    {
        if (itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            return value;
        }
        return null;
    }
    public void Add(InventoryItemData referenceData)
    {
        if (referenceData.isStackable)
        {
            if (itemDictionary.TryGetValue(referenceData, out InventoryItem value))
            {
                value.AddToStack();
            }
            else
            {
                InventoryItem newItem = new InventoryItem(referenceData);
                inventoryList.Add(newItem);
                itemDictionary.Add(referenceData, newItem);
            }
        }
        else
        {
            InventoryItem newItem = new InventoryItem(referenceData);
            inventoryList.Add(newItem);
        }
        _inventoryDisplay.UpdateInventory();
    }
    public void Remove(InventoryItemData referenceData)
    {
        if (referenceData.isStackable)
        {
            if (itemDictionary.TryGetValue(referenceData, out InventoryItem value))
            {
                value.RemoveFromStack();
                if (value.stackSize == 0)
                {
                    inventoryList.Remove(value);
                    itemDictionary.Remove(referenceData);
                }
            }
        }
        else
        {
            InventoryItem itemToRemove = inventoryList.Find(item => item.data == referenceData);
            if (itemToRemove != null)
            {
                inventoryList.Remove(itemToRemove);
            }
        }
        _inventoryDisplay.UpdateInventory();
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
        stackSize = Mathf.Max(0, stackSize - 1);
    }
}
