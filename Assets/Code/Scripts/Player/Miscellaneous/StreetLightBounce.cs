using UnityEngine;

public class StreetLightBounce : MonoBehaviour
{
    [SerializeField] private Light _light;

    private float _Startingintensity;

    private void Awake()
    {
        _Startingintensity = _light.intensity;
    }

    void Update()
    {
        float t = BeatManager.Instance.BeatProgress;
        float pulse = 1 - Mathf.Sin(t * Mathf.PI);
        _light.intensity = Mathf.Lerp(_Startingintensity * 0.5f, _Startingintensity, pulse);
    }
}
