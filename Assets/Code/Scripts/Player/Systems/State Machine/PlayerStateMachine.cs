using UnityEngine;

public class PlayerStateMachine : AbstractStateMachine<PlayerActionState>
{
    private bool _isMoving;
    private bool _usingPrimaryAction, _usingSecondaryAction, _usingSpecialAction;
    private bool _isDashing;
    private bool _canDash = true;
    private bool _canPush = true;
    private Vector2 _locomotion;

    public bool IsMoving { get => _isMoving; }
    public bool UsingPrimaryAction { get => _usingPrimaryAction; set => _usingPrimaryAction = value; }
    public bool UsingSecondaryAction { get => _usingSecondaryAction; set => _usingSecondaryAction = value; }
    public bool UsingSpecialAction { get => _usingSpecialAction; set => _usingSpecialAction = value; }
    public bool IsDashing { get => _isDashing; set => _isDashing = value; }
    public bool CanDash { get => _canDash; set => _canDash = value; }
    public bool CanPush { get => _canPush; set => _canPush = value; }
    public Vector2 Locomotion { get => _locomotion; }

    public void BindInputs(InputTranslator translator)
    {
        if (translator == null) return;
        translator.OnMovementEvent += HandleMovement;
        translator.OnSecondaryActionEvent += HandleSecondaryAction;
        translator.OnPrimaryActionEvent += HandlePrimaryAction;
        translator.OnDashEvent += HandleDash;
        translator.OnSpecialAttackEvent += HandleSpecialAction;
    }
    public void UnbindInputs(InputTranslator translator)
    {
        if (translator == null) return;
        translator.OnMovementEvent -= HandleMovement;
        translator.OnSecondaryActionEvent -= HandleSecondaryAction;
        translator.OnPrimaryActionEvent -= HandlePrimaryAction;
        translator.OnDashEvent -= HandleDash;
        translator.OnSpecialAttackEvent -= HandleSpecialAction;
    }

    public void HandleMovement(Vector2 locomotion)
    {
        _locomotion = locomotion;

        if (_locomotion != Vector2.zero)
        {
            _isMoving = true;
        }
        else
        {
            _isMoving = false;
        }
    }
    public void HandlePrimaryAction(bool buttonPressed)
    {
        if (TempoConductor.Instance.IsOnBeat() && buttonPressed && !SpamPrevention.InputLocked)
        {
            _usingPrimaryAction = true;
        }
        else
        {
            _usingPrimaryAction = false;
        }
    }
    public void HandleSecondaryAction(bool buttonPressed)
    {
        if (TempoConductor.Instance.IsOnBeat() && buttonPressed && !SpamPrevention.InputLocked)
        {
            _usingSecondaryAction = true;
        }
        else
        {
            _usingSecondaryAction = false;
        }
    }

    public void HandleSpecialAction(bool buttonPressed)
    {
        if(TempoConductor.Instance.IsOnBeat() && buttonPressed && !SpamPrevention.InputLocked)
        {
            _usingSpecialAction = true;
        }
        else
        {
            _usingSpecialAction = false;
        }
    }
    public void HandleDash(bool buttonPressed)
    {
        //Checks if moving because it would cause bug where player could enter dash immediately as they started moving
        if (buttonPressed && !SpamPrevention.InputLocked && _isMoving) _isDashing = true;
        else _isDashing = false;
    }
}
