using UnityEngine;

[CreateAssetMenu(fileName = "Health Potion", menuName = "Echo's Cry/Inventory/Health Potion")]
public class HealthitemData : InventoryItemData
{
    public float healAmount;

    public override void Use(Player player)
    {
        player.Health.HealHealth(healAmount);
    }
}