using UnityEngine;

[CreateAssetMenu(fileName = "Health Potion", menuName = "Echo's Cry/Inventory/Health Potion")]
public class HealthItemData : InventoryItemData
{
    public float healAmount;

    public override void Use(Player player)
    {
        player.Health.HealHealth(healAmount);
    }
    public override bool CanUse(Player player)
    {
        if (player.Health.CurrentHealth < player.Health.MaxHealth) return true;
        return false;
    }
}