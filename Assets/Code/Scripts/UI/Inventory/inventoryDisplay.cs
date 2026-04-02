using System;
using System.Collections;
using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    public SlotScript[] slotScriptArray = new SlotScript[4];

    public void UpdateInventory()
    {
        int i = 0;
        foreach (InventoryItem item in InventoryManager.Instance.inventoryList)
        {
            if (i >= slotScriptArray.Length)
                break;

            slotScriptArray[i].Set(item);
            i++;
        }

        for (; i < slotScriptArray.Length; i++)
        {
            slotScriptArray[i].Set(null);
        }
    }
}
