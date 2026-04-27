using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Echo's Cry/Input System/Input Translator")]

//ISSUE: This system needs to handle updating and sending invoke for when bindings are changed in settings menu, this updates supporting tool tips to know what the current binding is.

public class InputTranslator : ScriptableObject, 
    PlayerInputs.IGameplayActions,
    PlayerInputs.IPauseMenuActions,
    PlayerInputs.IShopMenuActions,
    PlayerInputs.IDialogueActions
{
    private void Awake()
    {
        _playerInputs = new PlayerInputs();
        _playerInputs.Gameplay.SetCallbacks(this);
        _playerInputs.PauseMenu.SetCallbacks(this);
        _playerInputs.ShopMenu.SetCallbacks(this);
        _playerInputs.Dialogue.SetCallbacks(this);
    }
    private void OnEnable()
    {
        if (_playerInputs == null)
        {
            _playerInputs = new PlayerInputs();
            _playerInputs.Gameplay.SetCallbacks(this);
            _playerInputs.PauseMenu.SetCallbacks(this);
            _playerInputs.ShopMenu.SetCallbacks(this);
            _playerInputs.Dialogue.SetCallbacks(this);
        }
        _playerInputs.Gameplay.Enable();
        _playerInputs.PauseMenu.Disable();
        _playerInputs.ShopMenu.Disable();
        _playerInputs.Dialogue.Disable();
    }
    private void OnDisable()
    {
        _playerInputs.Gameplay.Disable();
        _playerInputs.PauseMenu.Disable();
        _playerInputs.ShopMenu.Disable();
        _playerInputs.Dialogue.Disable();
    }
    private void OnDestroy()
    {
        _playerInputs.Gameplay.Disable();
        _playerInputs.PauseMenu.Disable();
        _playerInputs.ShopMenu.Disable();
        _playerInputs.Dialogue.Disable();

        _playerInputs.Gameplay.RemoveCallbacks(this);
        _playerInputs.PauseMenu.RemoveCallbacks(this);
        _playerInputs.ShopMenu.RemoveCallbacks(this);
        _playerInputs.Dialogue.RemoveCallbacks(this);
        _playerInputs = null;
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        OnMovementEvent?.Invoke(context.ReadValue<Vector2>());
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started) OnDashEvent?.Invoke(true);
        else if (context.canceled) OnDashEvent?.Invoke(false);
    }
    public void OnLightAttack(InputAction.CallbackContext context)
    {
        if (context.started) OnPrimaryActionEvent?.Invoke(true);
        else if(context.canceled) OnPrimaryActionEvent?.Invoke(false);
    }
    public void OnHeavyAttack(InputAction.CallbackContext context)
    {
        if (context.started) OnSecondaryActionEvent?.Invoke(true);
        else if (context.canceled) OnSecondaryActionEvent?.Invoke(false);
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started) OnPauseEvent?.Invoke();
    }
    public void OnUpgrade(InputAction.CallbackContext context)
    {
        if (context.started) OnUpgradeEvent?.Invoke();
    }
    public void OnResume(InputAction.CallbackContext context)
    {
        if (context.started) OnResumeEvent?.Invoke();
    }
    public void OnCloseShop(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnCloseShopEvent?.Invoke();
            _playerInputs.Gameplay.Enable();
            _playerInputs.ShopMenu.Disable();
        }
    }
    public void OnItem1(InputAction.CallbackContext context)
    {
        if (context.started) OnItem1Event?.Invoke(true, 0);
        else if (context.canceled) OnItem1Event?.Invoke(false, 0);
    }
    public void OnItem2(InputAction.CallbackContext context)
    {
        if (context.started) OnItem2Event?.Invoke(true, 1);
        else if (context.canceled) OnItem2Event?.Invoke(false, 1);
    }
    public void OnItem3(InputAction.CallbackContext context)
    {
        if (context.started) OnItem3Event?.Invoke(true, 2);
        else if (context.canceled) OnItem3Event?.Invoke(false, 2);
    }
    public void OnItem4(InputAction.CallbackContext context)
    {
        if (context.started) OnItem4Event?.Invoke(true, 3);
        else if (context.canceled) OnItem4Event?.Invoke(false, 3);
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if(context.started) OnInteractEvent?.Invoke();
    }
    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.started) OnSubmitEvent?.Invoke(true);
        else if (context.canceled) OnSubmitEvent?.Invoke(false);
    }
    public void OnTeleport(InputAction.CallbackContext context)
    {
        if (context.started) OnTeleportEvent?.Invoke();
    }
    public void OnDrop(InputAction.CallbackContext context)
    {
        if (context.started) OnDropEvent?.Invoke();
    }
    public void OnSpecialAttack(InputAction.CallbackContext context)
    {
        if (context.started) OnSpecialAttackEvent?.Invoke(true);
        else if(context.canceled) OnSpecialAttackEvent?.Invoke(false);
    }

    private PlayerInputs _playerInputs;
    public PlayerInputs PlayerInputs { get { return _playerInputs; } }  

    public event Action<Vector2>    OnMovementEvent;
    public event Action<bool>       OnDashEvent;
    public event Action             OnTeleportEvent;
    public event Action<bool>       OnPrimaryActionEvent;
    public event Action<bool>       OnSecondaryActionEvent;
    public event Action<bool>       OnSpecialAttackEvent;
    public event Action             OnInteractEvent;
    public event Action<bool>       OnSubmitEvent;
    public event Action             OnPauseEvent;
    public event Action             OnUpgradeEvent;
    public event Action             OnResumeEvent;
    public event Action             OnCloseShopEvent;
    public event Action             OnDropEvent;
    public event Action<bool, int>  OnItem1Event, OnItem2Event, OnItem3Event, OnItem4Event;
}