using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalBeatHandler : MonoBehaviour
{
    [SerializeField] private DecalProjector decalProjector;
    [SerializeField] private Vector2 decalDimensions;
    [SerializeField] private float pulseScalar = 0.5f;

    private void Update()
    {
        float t = BeatManager.Instance.BeatProgress;
        float pulse = 1 - Mathf.Sin(t * Mathf.PI);
        Vector3 newSize = decalProjector.size;
        newSize.x = decalDimensions.x + pulse * pulseScalar;
        newSize.y = decalDimensions.x + pulse * pulseScalar;
        decalProjector.size = newSize;
    }
}