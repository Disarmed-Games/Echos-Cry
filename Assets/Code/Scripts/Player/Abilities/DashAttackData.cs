using UnityEngine;

[CreateAssetMenu(fileName = "Dash Attack Data", menuName = "Echo's Cry/Player Data/Dash Attack")]
public class DashAttackData : ScriptableObject
{
    [SerializeField] float damage;
    [SerializeField] float force;

    public float Damage { get => damage;}
    public float Force { get => force; }
}