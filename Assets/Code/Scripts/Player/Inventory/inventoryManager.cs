using AudioSystem;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
/// Original Author: Abby
/// All Contributors Since Creation: Abby
/// Last Modified By:

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager _instance;
    public static InventoryManager Instance { get { return _instance; } }

    private Dictionary<InventoryItemData, InventoryItem> itemDictionary;
    public List<InventoryItem> inventoryList { get; private set; }  

    [SerializeField] private InventoryDisplay _inventoryDisplay;
    [SerializeField] private InputTranslator _inputTranslator;
    [SerializeField] private soundEffect _useItemSound;

    private Player _player;

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
        inventoryList = new List<InventoryItem>();
        itemDictionary = new Dictionary<InventoryItemData, InventoryItem>();
        _player = GameObject.FindWithTag("Player")?.GetComponent<Player>();

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

        SoundEffectManager.Instance.Builder
                .SetSound(_useItemSound)
                .SetSoundPosition(PlayerRef.Transform.position)
                .ValidateAndPlaySound();

        InventoryItem usedItem = inventoryList[index];
        usedItem.data.Use(_player);
        Remove(usedItem.data);
    }

    public bool IsFull(InventoryItemData referenceData)
    {
        if (itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            return false;
        }
        else
        {
            return inventoryList.Count >= _inventoryDisplay.slotScriptArray.Length;
        }   
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
        _inventoryDisplay.UpdateInventory();
    }
    public void Remove(InventoryItemData referenceData)
    {
        if (itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            value.RemoveFromStack();
            if (value.stackSize == 0)
            {
                inventoryList.Remove(value);
                itemDictionary.Remove(referenceData);
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
        stackSize = Mathf.Max(0, stackSize - 1);
    }
}
