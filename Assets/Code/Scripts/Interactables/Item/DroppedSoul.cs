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

    protected override void OnInteraction(Collider other)
    {
        if (!InventoryManager.Instance.IsFull())
        {
            InventoryItemData dropItem = CalculateDroppedItem();
            if (dropItem != null)
            {
                InventoryManager.Instance.Add(dropItem);

            }
        }
    }
}
