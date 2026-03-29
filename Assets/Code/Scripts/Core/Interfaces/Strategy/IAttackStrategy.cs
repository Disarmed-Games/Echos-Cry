using UnityEngine;

public interface IAttackStrategy
{
    void Execute(float damage, Vector3 direction, Transform origin);
}