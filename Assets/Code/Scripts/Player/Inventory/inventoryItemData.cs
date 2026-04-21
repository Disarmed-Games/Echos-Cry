using UnityEngine;

public abstract class InventoryItemData : ScriptableObject
{
    public string displayName;
    public string description;
    public Sprite icon;
    public bool isStackable;
    public abstract void Use(Player player);
    public abstract bool CanUse(Player player);
}
