using UnityEngine;

[CreateAssetMenu(fileName = "Armor Potion", menuName = "Echo's Cry/Inventory/Armor Potion")]
public class ArmorItemData : InventoryItemData
{
    public float armorAmount;

    public override void Use(Player player)
    {
        player.Health.HealArmor(armorAmount);
    }

    public override bool CanUse(Player player)
    {
        if (player.Health.CurrentArmor < player.Health.MaxArmor) return true;
        return false;
    }
}