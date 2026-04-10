using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class DevTeleport : MonoBehaviour
{
    [SerializeField] private InputTranslator translator;
    [SerializeField] private LayerMask ground;
    [SerializeField] private Rigidbody _playerRigidbody;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        translator.OnTeleportEvent += TeleportPlayer;
    }
    private void OnDisable()
    {
        translator.OnTeleportEvent -= TeleportPlayer;
    }

    private void TeleportPlayer()
    {
        Ray ray = CameraManager.MainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ground))
        {
            _playerRigidbody.position = hit.point;
        }
    }
}
