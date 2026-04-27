using UnityEngine;

[CreateAssetMenu(fileName = "Blazing Steps", menuName = "Echo's Cry/Inventory/Blazing Steps Item Data")]
public class BlazingStepsItemData : InventoryItemData
{
    [SerializeField] EffectData _effect;

    public override void Use(Player player)
    {
        player.DashHandler.AddEffect(_effect);
        player.ActiveEffectsTier2.Add(_effect);
    }

}