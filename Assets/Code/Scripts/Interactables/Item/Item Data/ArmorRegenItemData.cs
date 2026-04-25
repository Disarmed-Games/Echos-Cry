using UnityEngine;

[CreateAssetMenu(fileName = "Armor Regen", menuName = "Echo's Cry/Inventory/Armor Regen")]
public class ArmorRegenItemData : InventoryItemData
{
    [SerializeField] EventChannel _regenArmorChannel;

    public override void Use(Player player)
    {
        _regenArmorChannel.Invoke();
    }
    public override bool CanUse(Player player)
    {
        return true;
    }
}