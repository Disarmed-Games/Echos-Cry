using System;
using UnityEngine;

[Serializable]
struct ItemChance
{
    public InventoryItemData item;
    public float frequency;
};

public class DroppedSoul : ItemDropHandler
{
    [SerializeField] private ItemChance[] itemChances;
    private InventoryItemData dropItem;

    private InventoryItemData CalculateDroppedItem()
    {
        float totalFrequency = 0;
        foreach (var item in itemChances)
        {
            totalFrequency += item.frequency;
        }
        float randomChance = UnityEngine.Random.Range(0f, totalFrequency);

        totalFrequency = 0;
        foreach (var item in itemChances)
        {
            totalFrequency += item.frequency;

            if (totalFrequency >= randomChance)
            {
                return item.item;
            }
        }
        return null;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (dropItem == null)
            dropItem = CalculateDroppedItem();
        
        if (InventoryManager.Instance.IsFull(dropItem)) return;

        base.OnTriggerEnter(other);
    }

    protected override void OnInteraction(Collider other)
    {
        if (dropItem != null)
        {
            InventoryManager.Instance.Add(dropItem);
        }
    }
}
