using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance;
    public static CameraManager Instance { get { return _instance; } }

    private static Camera _mainCamera;
    private CinemachineCamera cameraBrain;
    private CinemachineBasicMultiChannelPerlin cinemachinePerlin;

    private float shakeTimer;
    private float startingIntensity;
    private float shakeTimerTotal;

    private float startingOrthagaphicSize;

    [SerializeField] private Transform target;
    public static Camera MainCamera { get { return _mainCamera; } }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
            return;
        }
        _instance = this;

        cameraBrain = gameObject.GetComponentInChildren<CinemachineCamera>();
        cinemachinePerlin = cameraBrain.GetComponent<CinemachineBasicMultiChannelPerlin>();

        _mainCamera = GetComponentInChildren<Camera>();
    }
    private void Start()
    {
        cameraBrain.Target.TrackingTarget = target;

        if (cameraBrain != null)
        {
            startingOrthagaphicSize = cameraBrain.Lens.OrthographicSize;
        }
    }

    // This was provided from a tutorial by CodeMonkey on Youtube https://youtu.be/ACf1I27I6Tk?si=Ic2BCVnjAV80Pkvv
    public void ScreenShake(float intensity, float time)
    {
        cinemachinePerlin.AmplitudeGain = intensity;
        startingIntensity = intensity;
        shakeTimer = time;
        shakeTimerTotal = time;
    }

    public void ZoomInCamera(float zoomLevel, float zoomDuration)
    {
        StopAllCoroutines();
        StartCoroutine(ZoomRoutine(startingOrthagaphicSize, zoomLevel, zoomDuration));
    }

    public void ZoomOutCamera(float zoomDuration)
    {
        StopAllCoroutines();
        StartCoroutine(ZoomRoutine(cameraBrain.Lens.OrthographicSize, startingOrthagaphicSize, zoomDuration));
    }

    IEnumerator ZoomRoutine(float startZoom, float endZoom, float zoomDuration)
    {
        float time = 0f;
        while (time < zoomDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / zoomDuration;
            cameraBrain.Lens.OrthographicSize = Mathf.Lerp(startZoom, endZoom, t);
            yield return null;
        }

        cameraBrain.Lens.OrthographicSize = endZoom;
    }

    private void Update()
    {
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            cinemachinePerlin.AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, shakeTimer / shakeTimerTotal);
        }
        else
        {
            cinemachinePerlin.AmplitudeGain = 0f;
        }
    }
}
