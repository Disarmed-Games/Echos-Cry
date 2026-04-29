using UnityEngine;

[CreateAssetMenu(fileName = "Critical Chance Item", menuName = "Echo's Cry/Inventory/Critical Chance Item")]
public class CriticalChanceEffect : InventoryItemData
{
    [SerializeField] private EffectData effect;

    public override void Use(Player player)
    {
        player.ActiveEffectsTier1.Add(effect);
        player.WeaponHolder.AddEffectPrimary(EchosCry.Combo.StateName.Light1, effect);
        player.WeaponHolder.AddEffectPrimary(EchosCry.Combo.StateName.Light2, effect);
        player.WeaponHolder.AddEffectPrimary(EchosCry.Combo.StateName.Light3, effect);
        player.WeaponHolder.AddEffectPrimary(EchosCry.Combo.StateName.Light4, effect);
        player.WeaponHolder.AddEffectPrimary(EchosCry.Combo.StateName.Light5, effect);
    }
}
