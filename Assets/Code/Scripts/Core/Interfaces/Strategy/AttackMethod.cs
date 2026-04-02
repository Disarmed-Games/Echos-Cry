
using System;
using UnityEngine;

[Serializable]
public abstract class AttackMethod : MonoBehaviour, IAttackStrategy
{
    protected bool _attackEnded;
    public bool AttackEnded { get => _attackEnded; }
    public abstract void Execute(float damage, Vector3 direction, Transform origin);
}
