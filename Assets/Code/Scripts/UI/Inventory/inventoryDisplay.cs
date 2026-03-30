using System;
using System.Collections;
using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    public slotScript[] slotScriptArray = new slotScript[4];

    private void OnUpdateInventory()
    {
        int i = 0;
        foreach(InventoryItem item in InventoryManager.Instance.inventory)
        {
            if(i == 0){
                slotScriptArray[i].Set(item);
                i++;
            }else if(i == 1){
                slotScriptArray[i].Set(item);
                i++;
            }else if(i == 2){
                slotScriptArray[i].Set(item);
                i++;
            }else if(i == 3){
                slotScriptArray[i].Set(item);
            }
        }
        slotScriptArray[i].Set(null);
    }

    void Update()
    { //ISSUE: There is no reason to call this every frame. Should be updated only when items are added to inventory.
        OnUpdateInventory();
    }
}
