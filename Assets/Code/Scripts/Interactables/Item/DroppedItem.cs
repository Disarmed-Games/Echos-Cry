using UnityEngine;

public class DroppedItem : ItemDropHandler
{
    public InventoryItemData item;

    protected override void OnInteraction(Collider other)
    {
        if (item != null)
        {
            InventoryManager.Instance.Add(item);
        }
    }
}
