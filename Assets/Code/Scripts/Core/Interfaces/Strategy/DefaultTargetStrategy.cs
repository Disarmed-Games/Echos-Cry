using UnityEngine;
using static UnityEngine.Rendering.STP;

public class DefaultTargetStrategy : TargetStrategy
{
    public override Vector3 Execute(Transform origin)
    {
        return origin.position;
    }
}
