using UnityEngine;

[CreateAssetMenu(fileName = "Effect Item", menuName = "Echo's Cry/Inventory/Effect Item")]
public class EffectItemData: InventoryItemData
{
    [SerializeField] private EventChannel effectChannel;

    public override void Use(Player player)
    {
        effectChannel.Invoke();
    }
    public override bool CanUse(Player player)
    {
        return true;
    }

}
