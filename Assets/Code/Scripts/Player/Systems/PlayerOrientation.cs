using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerOrientation : MonoBehaviour
{
    [SerializeField] private InputTranslator inputTranslator;
    [SerializeField] private LayerMask groundLayer;

    public bool IsRotating = true;
    private Vector3 aimDirection;
    private Quaternion aimRotation;
    public Vector3 Direction { get { return aimDirection; } }
    public Quaternion Rotation { get { return aimRotation; } }

    private void OnEnable()
    {
        inputTranslator.OnMovementEvent += OnMovement;
    }
    private void OnDisable()
    {
        inputTranslator.OnMovementEvent -= OnMovement;
    }

    private void OnMovement(Vector2 dir)
    {
        if (dir == Vector2.zero) return;

        // Convert 2D input into a 3D direction
        aimDirection = new Vector3(dir.x, 0f, dir.y);
    }

    private void UpdateRotation()
    {
        if (aimDirection == Vector3.zero) return;

        aimRotation = Quaternion.LookRotation(aimDirection);
        transform.rotation = aimRotation;
    }

    private void Update()
    {
        if(IsRotating) UpdateRotation();
    }

    //private void UpdateRotation()
    //{
    //    Ray ray = CameraManager.MainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

    //    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
    //    {
    //        //Calculates the direction between the mouse mapped to world space and the players position.
    //        aimDirection = (hit.point - transform.parent.position).normalized;

    //        aimDirection.y = 0;
    //        aimRotation = Quaternion.LookRotation(aimDirection);
    //        transform.rotation = aimRotation;
    //    }
    //}
}
