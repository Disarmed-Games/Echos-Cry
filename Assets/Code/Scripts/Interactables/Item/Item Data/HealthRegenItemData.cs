using UnityEngine;

[CreateAssetMenu(fileName = "Health Regen", menuName = "Echo's Cry/Inventory/Health Regen")]
public class HealthRegenItemData : InventoryItemData
{
    [SerializeField] EventChannel _regenHealthChannel;

    public override void Use(Player player)
    {
        _regenHealthChannel.Invoke();
    }
}