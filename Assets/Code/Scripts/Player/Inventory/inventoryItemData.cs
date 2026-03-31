using UnityEngine;

public abstract class InventoryItemData : ScriptableObject
{
    public string displayName;
    public Sprite icon;
    public abstract void Use(Player player);
}
