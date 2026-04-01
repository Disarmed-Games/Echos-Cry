using UnityEngine;

public abstract class InventoryItemData : ScriptableObject
{
    public string displayName;
    public string description;
    public Sprite icon;
    public abstract void Use(Player player);
}
