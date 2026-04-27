using UnityEngine;

[CreateAssetMenu(fileName = "Dash Attack", menuName = "Echo's Cry/Inventory/Dash Attack")]
public class DashAttackItemData : InventoryItemData
{
    [SerializeField] EffectData _dashAttack;

    public override void Use(Player player)
    {
        player.DashHandler.AddEffect(_dashAttack);
    }

    public override bool CanUse(Player player)
    {
        return true;
    }
}