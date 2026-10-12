using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;

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
    [Header("Leaf Cutaway")]
    [SerializeField] private bool leafCutawayEnabled = true;
    [SerializeField, Min(0.01f)] private float leafCutawayRadius = 2.5f;
    [SerializeField, Min(0.01f)] private float leafCutawayEdgeSoftness = 0.8f;
    [SerializeField] private float leafCutawayAnchorHeight = 1f;
    [SerializeField] private float leafCutawayMinimumHeight = 1.5f;
    [SerializeField, Min(0.01f)] private float leafCutawayHeightBlend = 0.5f;

    private static readonly int LeafAnchorId = Shader.PropertyToID("_LeafCutawayAnchor");
    private static readonly int LeafSettingsId = Shader.PropertyToID("_LeafCutawaySettings");
    private Transform leafCutawayPlayer;
    public static Camera MainCamera { get { return _mainCamera; } }

    private void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += UpdateLeafCutaway;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= UpdateLeafCutaway;
        Shader.SetGlobalVector(LeafAnchorId, Vector4.zero);
    }

    private void UpdateLeafCutaway(ScriptableRenderContext context, Camera renderingCamera)
    {
        // Runs after Cinemachine has positioned the camera. Other cameras get no effect.
        if (renderingCamera != _mainCamera || !leafCutawayEnabled)
        {
            Shader.SetGlobalVector(LeafAnchorId, Vector4.zero);
            return;
        }

        if (target != null)
            leafCutawayPlayer = target;
        else if (leafCutawayPlayer == null && Player.Instance != null)
            leafCutawayPlayer = Player.Instance.transform;

        if (leafCutawayPlayer == null)
        {
            Shader.SetGlobalVector(LeafAnchorId, Vector4.zero);
            return;
        }

        Vector3 anchor = leafCutawayPlayer.position + Vector3.up * leafCutawayAnchorHeight;
        Shader.SetGlobalVector(LeafAnchorId, new Vector4(anchor.x, anchor.y, anchor.z, 1f));
        Shader.SetGlobalVector(LeafSettingsId, new Vector4(
            Mathf.Max(0.01f, leafCutawayRadius), Mathf.Max(0.01f, leafCutawayEdgeSoftness),
            leafCutawayPlayer.position.y + leafCutawayMinimumHeight, Mathf.Max(0.01f, leafCutawayHeightBlend)));
    }

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
