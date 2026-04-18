using UnityEngine;

[CreateAssetMenu(fileName = "Lucky Clover", menuName = "Echo's Cry/Inventory/Lucky Clover")]
public class LuckyCloverItemData: InventoryItemData
{
    public override void Use(Player player)
    {
            //make it so that player has 10% increased xp drops from enemies or 10% soul drops from enemies
    }
    public override bool CanUse(Player player)
    {
        return true;
    }

}
