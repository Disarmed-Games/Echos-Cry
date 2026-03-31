using System;
using UnityEngine;

[Serializable]
public abstract class TargetStrategy : MonoBehaviour, ITargetStrategy
{
    public abstract Vector3 Execute(Transform origin);
}
