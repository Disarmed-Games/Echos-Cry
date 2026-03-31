using UnityEngine;

public class RadiusTargetStrategy : TargetStrategy
{
    [SerializeField] private float radius = 5.0f;
    public override Vector3 Execute(Transform origin)
    {
        Vector3 point = Random.onUnitSphere * radius;
        point.y = 0;
        return origin.position + point;
    }
}