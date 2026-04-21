using AudioSystem;
using System.Collections;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private BoolEventChannel _lockMovementChannel;
    [SerializeField] private DoubleIntEventChannel _dashUpdateChannel;
    private bool _isMovementLocked = false;

    public void Move(Vector2 playerInputLocomotion, float movementMultiplier)
    {
        if (_isMovementLocked) return;
        if (_playerMovementConfig == null) return;

        Vector3 moveDirection = ((playerInputLocomotion.y * forwardVector) + (playerInputLocomotion.x * rightVector)).normalized;
        Vector3 targetVelocity = movementMultiplier * _moveSpeed * moveDirection;
        Vector3 currentVelocity = _playerRigidbody.linearVelocity;

        Vector3 velocityChange = targetVelocity - currentVelocity;
        velocityChange.y = 0;

        _playerRigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
    }
    public void Dash()
    {
        if (_isMovementLocked) return;
        if (_playerMovementConfig == null) return;

        Vector3 dashDirection = _playerRigidbody.linearVelocity;
        dashDirection.y = 0f; //Ensure dash is only affecting horizontal velocity.
        dashDirection.Normalize();

        _playerRigidbody.AddForce(dashDirection * _dashSpeed, ForceMode.VelocityChange);

        _dashCount--;
        _dashUpdateChannel.Invoke(_dashCount, _maxDashCount);
        StartDashCooldown();
    }

    private IEnumerator AddDashCoroutine()
    {
        yield return new WaitForSeconds(_dashCooldown);
        _dashCount++;
        _dashUpdateChannel.Invoke(_dashCount, _maxDashCount);
    }
    public void StartDashCooldown()
    {
        StartCoroutine(AddDashCoroutine());
    }

    public void MomentumPush()
    {
        if (_isMovementLocked) return;
        if (_playerMovementConfig == null) return;
        //Working on making momentum maintain when attacking in opposite direction
        //float directionDot = Vector3.Dot(direction.normalized, _playerRigidbody.linearVelocity.normalized);
        _playerRigidbody.AddForce(_playerMovementConfig.AttackMomentumSpeed * _playerRigidbody.linearVelocity.normalized, ForceMode.VelocityChange);
    }

    private void OnEnable()
    {
        _lockMovementChannel.Channel += (state) => _isMovementLocked = state;
        _dashCount = _playerMovementConfig.DashCount;
    }
    private void OnDisable()
    {
        _lockMovementChannel.Channel -= (state) => _isMovementLocked = state;
        StopAllCoroutines();
    }

    void Start()
    {
        if(_playerMovementConfig == null)
        {
            Debug.LogWarning("Player Movement Configuration file is null!");
            return;
        }
        else
        {
            _dashSpeed = _playerMovementConfig.DashSpeed;
            _moveSpeed = _playerMovementConfig.PlayerSpeed;
            _maxDashCount = _playerMovementConfig.DashCount;
            _dashCount = _maxDashCount;
            _dashCooldown = _playerMovementConfig.DashCooldown;
            _dashUpdateChannel.Invoke(_dashCount, _maxDashCount);
        }

        if (Camera.main != null)
        {
            forwardVector = Camera.main.transform.forward.normalized;
            forwardVector.y = 0f;

            rightVector = Camera.main.transform.right.normalized;
            rightVector.y = 0f;
        }
    }

    [Header("Configuration Object")]
    [SerializeField] private PlayerMovementConfig _playerMovementConfig;
    public PlayerMovementConfig PlayerMovementConfig { get { return _playerMovementConfig; } }
    
    [Header("Player Movement System Dependencies")]
    [SerializeField] private Rigidbody _playerRigidbody;

    private float _dashSpeed;
    private float _moveSpeed;
    private float _dashCooldown;
    private int _dashCount;
    private int _maxDashCount;
    public float DashSpeed { get => _dashSpeed; set => _dashSpeed = value; }
    public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
    public float DashCooldown { get => _dashCooldown; set => _dashCooldown = value; }
    public int DashMaxCount { get => _maxDashCount; set => _maxDashCount = value; }
    public int DashCount { get => _dashCount; set => _dashCount = value; }

    public bool HasDash => _dashCount > 0;

    private Vector3 forwardVector;
    private Vector3 rightVector;
}