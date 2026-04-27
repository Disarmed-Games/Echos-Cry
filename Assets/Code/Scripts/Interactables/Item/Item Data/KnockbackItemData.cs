using UnityEngine;

[CreateAssetMenu(fileName = "Knockback Effect Item", menuName = "Echo's Cry/Inventory/Knockback Effect Item")]
public class KnockbackItemData : InventoryItemData
{
    [SerializeField] private EffectData effect;

    public override void Use(Player player)
    {
        player.ActiveEffectsTier1.Add(effect);
        player.WeaponHolder.AddEffectPrimary(EchosCry.Combo.StateName.Heavy1, effect);
        player.WeaponHolder.AddEffectPrimary(EchosCry.Combo.StateName.Heavy2, effect);
        player.WeaponHolder.AddEffectPrimary(EchosCry.Combo.StateName.Heavy3, effect);
    }
}
