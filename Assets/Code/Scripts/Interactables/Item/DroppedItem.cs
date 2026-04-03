using UnityEngine;

public class DroppedItem : ItemDropHandler
{
    public InventoryItemData item;

    protected override void OnTriggerEnter(Collider other)
    {
        if (InventoryManager.Instance.IsFull(item)) return;

        base.OnTriggerEnter(other);
    }

    protected override void OnInteraction(Collider other)
    {
        if (item != null)
        {
            InventoryManager.Instance.Add(item);
        }
    }
}
