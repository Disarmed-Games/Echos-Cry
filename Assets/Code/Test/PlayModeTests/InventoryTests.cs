using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
public class InventoryTests
{
    InventoryManager inventoryInstance;
    InventoryItemData testItemData;

    [UnitySetUp]
    public IEnumerator SetupInventory()
    {
        GameObject inventoryGO = new GameObject("InventoryManager");
        inventoryInstance = inventoryGO.AddComponent<InventoryManager>();
        var inventoryBackingField = typeof(InventoryManager).GetField("<inventory>k__BackingField",System.Reflection.BindingFlags.NonPublic 
                |System.Reflection.BindingFlags.Instance);
        var dictionaryField = typeof(InventoryManager).GetField("m_itemDictionary",System.Reflection.BindingFlags.NonPublic 
                |System.Reflection.BindingFlags.Instance);

        inventoryBackingField.SetValue(inventoryInstance, new List<InventoryItem>());
        dictionaryField.SetValue(inventoryInstance, new Dictionary<InventoryItemData, InventoryItem>());

        var inputTranslator = ScriptableObject.CreateInstance<InputTranslator>();

        var inputField = typeof(InventoryManager).GetField("_inputTranslator", System.Reflection.BindingFlags.NonPublic
                |System.Reflection.BindingFlags.Instance);
        inputField.SetValue(inventoryInstance, inputTranslator);

        testItemData = ScriptableObject.CreateInstance<InventoryItemData>();

        yield break;
    }
    [UnityTest]
    public IEnumerator Inventory_NullCheck()
    {
        Assert.That(inventoryInstance.inventoryList, Is.Not.Null);
        yield break;
    }

    [UnityTest]
    public IEnumerator Inventory_AddItem_StoresCorrectly(){
        inventoryInstance.Add(testItemData);
        Assert.That(inventoryInstance.inventoryList.Count, Is.EqualTo(1));
        Assert.That(inventoryInstance.Get(testItemData), Is.Not.Null);
        Assert.That(inventoryInstance.Get(testItemData).stackSize, Is.EqualTo(1));
        yield break;
    }

    [UnityTest]
    public IEnumerator Inventory_Stacking_WorksCorrectly()
    {
        inventoryInstance.Add(testItemData);
        inventoryInstance.Add(testItemData);
        
        Assert.That(inventoryInstance.inventoryList.Count, Is.EqualTo(1));
        Assert.That(inventoryInstance.Get(testItemData).stackSize, Is.EqualTo(2));
        yield break;
    }

    [UnityTest]
    public IEnumerator Inventory_RemoveItem_RemovesWhenStackZero()
    {
        inventoryInstance.Add(testItemData);
        inventoryInstance.Remove(testItemData);
        Assert.That(inventoryInstance.inventoryList.Count, Is.EqualTo(0));
        Assert.That(inventoryInstance.Get(testItemData), Is.Null);
        yield break;
    }
}
