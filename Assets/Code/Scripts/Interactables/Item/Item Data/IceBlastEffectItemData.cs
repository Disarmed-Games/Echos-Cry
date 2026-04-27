using UnityEngine;

[CreateAssetMenu(fileName = "Ice Blast Effect Item", menuName = "Echo's Cry/Inventory/Ice Blast Effect Item")]
public class IceBlastEffectItemData : InventoryItemData
{
    [SerializeField] private EffectData effect;

    public override void Use(Player player)
    {
        player.ActiveEffectsTier2.Add(effect);
        player.WeaponHolder.AddEffectSpecial(EchosCry.Combo.StateName.Light1, effect);
        player.WeaponHolder.AddEffectSpecial(EchosCry.Combo.StateName.Light2, effect);
        player.WeaponHolder.AddEffectSpecial(EchosCry.Combo.StateName.Light3, effect);
        player.WeaponHolder.AddEffectSpecial(EchosCry.Combo.StateName.Light4, effect);
        player.WeaponHolder.AddEffectSpecial(EchosCry.Combo.StateName.Light5, effect);
    }
}