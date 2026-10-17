using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DevTools : MonoBehaviour
{
    [SerializeField] private InputTranslator translator;
    [SerializeField] private LayerMask ground;
    [SerializeField] private Rigidbody _playerRigidbody;
    private readonly Dictionary<Canvas, bool> _canvasVisibility = new Dictionary<Canvas, bool>();
    private bool _hudHidden;
    private bool _cursorWasVisible;

    void OnEnable()
    {
        translator.OnTeleportEvent += TeleportPlayer;
    }
    private void OnDisable()
    {
        translator.OnTeleportEvent -= TeleportPlayer;
        RestoreHud();
    }

    private void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.hKey.wasPressedThisFrame) return;

        if (_hudHidden) RestoreHud();
        else
        {
            _cursorWasVisible = Cursor.visible;
            _hudHidden = true;
        }
    }

    private void LateUpdate()
    {
        if (!_hudHidden) return;

        // Input device switching can show the cursor again, so keep it hidden with the HUD.
        Cursor.visible = false;

        // Include UI spawned after the toggle, without stopping its scripts or animations.
        foreach (var canvas in FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (!_canvasVisibility.ContainsKey(canvas))
                _canvasVisibility.Add(canvas, canvas.enabled);
            canvas.enabled = false;
        }
    }

    private void RestoreHud()
    {
        if (_hudHidden) Cursor.visible = _cursorWasVisible;

        foreach (var entry in _canvasVisibility)
            if (entry.Key != null) entry.Key.enabled = entry.Value;

        _canvasVisibility.Clear();
        _hudHidden = false;
    }

    private void TeleportPlayer()
    {
        if (Mouse.current == null || CameraManager.MainCamera == null) return;
        Ray ray = CameraManager.MainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ground))
        {
            _playerRigidbody.position = hit.point;
        }
    }
}
