using UnityEngine;

[CreateAssetMenu(fileName = "Dash Attack", menuName = "Echo's Cry/Inventory/Dash Attack")]
public class DashAttackItemData : InventoryItemData
{
    [SerializeField] EventChannel _dashAttackChannel;

    public override void Use(Player player)
    {
        _dashAttackChannel.Invoke();
    }

    public override bool CanUse(Player player)
    {
        return true;
    }
}
