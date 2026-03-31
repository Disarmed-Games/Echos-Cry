using UnityEngine;

[CreateAssetMenu(fileName = "Experience Potion", menuName = "Echo's Cry/Inventory/Experience Potion")]
public class ExperienceItemData : InventoryItemData
{
    public float experienceAmount;

    public override void Use(Player player)
    {
        player.XP.IncreaseXP(experienceAmount);
    }
}