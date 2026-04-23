using UnityEngine;

[CreateAssetMenu(menuName = "Echo's Cry/Weapon/Combo Data")]
public class ComboWeaponData : ScriptableObject
{
    [SerializeField] private AttackData[] _attackData;
    [SerializeField] private float _comboResetTime;

    public AttackData[] AttackData { get => _attackData; }
    public float ComboResetTime { get => _comboResetTime; }
}