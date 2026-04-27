using System;
using System.Collections.Generic;
using UnityEngine;

public class ChestDropHandler : ItemDropHandler
{
    private InventoryItemData dropItem;

    private int CalculateDroppedItemIndex()
    {
        float totalFrequency = 0;
        foreach (var item in DropManager.Instance.EffectDropList)
        {
            totalFrequency += item.frequency;
        }
        float randomChance = UnityEngine.Random.Range(0f, totalFrequency);

        totalFrequency = 0;
        for (int i = 0; i < DropManager.Instance.EffectDropList.Count; i++)
        {
            totalFrequency += DropManager.Instance.EffectDropList[i].frequency;

            if (totalFrequency >= randomChance)
            {
                return i;
            }
        }
        return -1;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (dropItem == null)
        {
            int index = CalculateDroppedItemIndex();
            if (index >= 0)
            {
                dropItem = DropManager.Instance.EffectDropList[index].item;
                DropManager.Instance.EffectDropList.RemoveAt(index);
            }
            else
            {
                Debug.Log("Out of items to give to player!");
                return;
            }
        }

        if (InventoryManager.Instance.IsFull(dropItem)) return;

        base.OnTriggerEnter(other);
    }

    protected override void OnInteraction(Collider other)
    {
        if (dropItem != null)
        {
            CameraManager.Instance.ScreenShake(0.8f, 0.75f);
            InventoryManager.Instance.Add(dropItem);
        }
    }
}

