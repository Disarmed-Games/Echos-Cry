using UnityEngine;

public interface ITargetStrategy
{
    Vector3 Execute(Transform origin);
}