using UnityEngine;

public abstract class InventoryItemData : ScriptableObject
{
    public string displayName;
    public Sprite icon;
    public abstract void Use(Player player);
}

[CreateAssetMenu(fileName = "Health Potion", menuName = "Echo's Cry/Inventory/Health Potion")]
public class HealthPotionData : InventoryItemData
{
    public float healAmount;

    public override void Use(Player player)
    {
        player.Health.HealHealth(healAmount);
    }
}

[CreateAssetMenu(fileName = "Shield Potion", menuName = "Echo's Cry/Inventory/Shield Potion")]
public class ShieldPotionData : InventoryItemData
{
    public float shieldAmount;

    public override void Use(Player player)
    {
        player.Health.HealArmor(shieldAmount);
    }
}
