using UnityEngine;

public class BeatPulseHandler : MonoBehaviour
{
    [SerializeField] private float pulseSize = 1.15f;
    [SerializeField] private float returnSpeed = 5f;
    private Vector3 startSize;

    private void Start()
    {
        startSize = transform.localScale;
    }
    private void OnEnable()
    {
        BeatManager.Instance.onWholeBeat += Pulse;
    }
    private void OnDisable()
    {
        if(BeatManager.Instance != null) BeatManager.Instance.onWholeBeat -= Pulse;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, startSize, Time.deltaTime * returnSpeed);
    }

    private void Pulse()
    {
        transform.localScale = startSize * pulseSize;
    }
}
