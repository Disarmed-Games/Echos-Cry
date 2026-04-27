using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemChance
{
    public InventoryItemData item;
    public float frequency;
};

public class DropManager : NonSpawnableSingleton<DropManager>
{
    [SerializeField] private List<ItemChance> effectDropChances;
    public List<ItemChance> EffectDropList { get => effectDropChances; set => effectDropChances = value; }
}
