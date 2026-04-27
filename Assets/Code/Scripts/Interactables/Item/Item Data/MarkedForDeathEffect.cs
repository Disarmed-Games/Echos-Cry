using UnityEngine;

[CreateAssetMenu(fileName = "Marked Death Item", menuName = "Echo's Cry/Inventory/Marked Death Item")]
public class MarkedForDeathEffect : InventoryItemData
{
    [SerializeField] private EffectData effect;

    public override void Use(Player player)
    {
        player.ActiveEffectsTier2.Add(effect);
        player.WeaponHolder.AddEffectPrimary(EchosCry.Combo.StateName.Light1, effect);
        player.WeaponHolder.AddEffectPrimary(EchosCry.Combo.StateName.Light2, effect);
        player.WeaponHolder.AddEffectPrimary(EchosCry.Combo.StateName.Light3, effect);
        player.WeaponHolder.AddEffectPrimary(EchosCry.Combo.StateName.Light4, effect);
        player.WeaponHolder.AddEffectPrimary(EchosCry.Combo.StateName.Light5, effect);
    }
}

