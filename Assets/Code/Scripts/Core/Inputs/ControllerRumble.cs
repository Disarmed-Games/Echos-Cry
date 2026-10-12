using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ControllerRumble : MonoBehaviour
{
    private static ControllerRumble _instance;
    private Gamepad _pad;
    private float _until;
    private float _low;
    private float _high;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        _instance = new GameObject("Controller Rumble").AddComponent<ControllerRumble>();
        DontDestroyOnLoad(_instance.gameObject);
    }

    public static void Play(InputTranslator input, float low, float high, float seconds)
    {
        if (_instance == null || input == null || input.ActiveControlScheme != InputTranslator.ControllerScheme ||
            !Application.isFocused || Time.timeScale <= 0f || DialogueGameplayPause.IsPaused) return;
        _instance.Pulse(input.ActiveGamepad, low, high, seconds);
    }

    public void Pulse(Gamepad pad, float low, float high, float seconds)
    {
        if (pad == null || !pad.added || !pad.enabled) return;
        if (pad != _pad) Stop();
        _pad = pad;
        // Overlapping effects retain the stronger pulse rather than weakening an impact.
        _low = Mathf.Max(_low, Mathf.Clamp01(low));
        _high = Mathf.Max(_high, Mathf.Clamp01(high));
        _until = Mathf.Max(_until, Time.unscaledTime + Mathf.Max(0f, seconds));
        _pad.SetMotorSpeeds(_low, _high);
    }

    public void Stop()
    {
        if (_pad != null && _pad.added) _pad.ResetHaptics();
        _pad = null;
        _low = _high = _until = 0f;
    }

    private void Update()
    {
        if (_pad != null && (Time.unscaledTime >= _until || Time.timeScale <= 0f ||
            DialogueGameplayPause.IsPaused || !_pad.added || !_pad.enabled)) Stop();
    }

    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnDisable()
    {
        Stop();
        InputSystem.onDeviceChange -= OnDeviceChange;
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= OnSceneChanged;
        if (_instance == this) _instance = null;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device == _pad && (change == InputDeviceChange.Disconnected ||
            change == InputDeviceChange.Removed || change == InputDeviceChange.Disabled)) Stop();
    }

    private void OnSceneChanged(Scene previous, Scene next) => Stop();
    private void OnApplicationFocus(bool focused) { if (!focused) Stop(); }
    private void OnApplicationPause(bool paused) { if (paused) Stop(); }
    private void OnApplicationQuit() => Stop();
}
