using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

[CreateAssetMenu(menuName = "Echo's Cry/Input System/Input Translator")]

public class InputTranslator : ScriptableObject, 
    PlayerInputs.IGameplayActions,
    PlayerInputs.IPauseMenuActions,
    PlayerInputs.IShopMenuActions,
    PlayerInputs.IDialogueActions
{
    private void Awake()
    {
        _playerInputs = new PlayerInputs();
        EnsureUpgradeControllerBinding();
        _playerInputs.Gameplay.SetCallbacks(this);
        ConfigureItemControls();
        _playerInputs.PauseMenu.SetCallbacks(this);
        _playerInputs.ShopMenu.SetCallbacks(this);
        _playerInputs.Dialogue.SetCallbacks(this);
    }
    private void OnEnable()
    {
        ClearItemSelection();
        InputSystem.onEvent += OnDeviceInput;
        InputSystem.onDeviceChange += OnDeviceChange;
        RemapOptionHandler.OnActionRemappedEvent += NotifyBindingsChanged;
        if (_playerInputs == null)
        {
            _playerInputs = new PlayerInputs();
            EnsureUpgradeControllerBinding();
            _playerInputs.Gameplay.SetCallbacks(this);
            ConfigureItemControls();
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
        InputSystem.onEvent -= OnDeviceInput;
        InputSystem.onDeviceChange -= OnDeviceChange;
        RemapOptionHandler.OnActionRemappedEvent -= NotifyBindingsChanged;
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
        _playerInputs.Dispose();
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
        if (SelectControllerItemSlot(context, 0)) return;
        if (context.started) OnItem1Event?.Invoke(true, 0);
        else if (context.canceled) OnItem1Event?.Invoke(false, 0);
    }
    public void OnItem2(InputAction.CallbackContext context)
    {
        if (SelectControllerItemSlot(context, 1)) return;
        if (context.started) OnItem2Event?.Invoke(true, 1);
        else if (context.canceled) OnItem2Event?.Invoke(false, 1);
    }
    public void OnItem3(InputAction.CallbackContext context)
    {
        if (SelectControllerItemSlot(context, 2)) return;
        if (context.started) OnItem3Event?.Invoke(true, 2);
        else if (context.canceled) OnItem3Event?.Invoke(false, 2);
    }
    public void OnItem4(InputAction.CallbackContext context)
    {
        if (SelectControllerItemSlot(context, 3)) return;
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

    public const string KeyboardMouseScheme = "KeyboardMouse";
    public const string ControllerScheme = "Controller";
    public string ActiveControlScheme { get; private set; } = KeyboardMouseScheme;
    public event Action OnBindingsChanged;
    private InputDevice _lastDevice;
    private static InputDevice _cursorDevice;

    // Listen globally so cursor switching also works in menus and across scene changes.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeCursorVisibility()
    {
        _cursorDevice = null;
        Cursor.visible = true;
        InputSystem.onEvent -= UpdateCursorDevice;
        InputSystem.onEvent += UpdateCursorDevice;
        InputSystem.onDeviceChange -= RestoreCursorOnDisconnect;
        InputSystem.onDeviceChange += RestoreCursorOnDisconnect;
    }

    private static void UpdateCursorDevice(InputEventPtr eventPtr, InputDevice device)
    {
        if (!IsMeaningfulInput(eventPtr, device)) return;
        _cursorDevice = device;
        Cursor.visible = !(device is Gamepad);
    }

    private static void RestoreCursorOnDisconnect(InputDevice device, InputDeviceChange change)
    {
        if (device != _cursorDevice || (change != InputDeviceChange.Disconnected &&
            change != InputDeviceChange.Removed && change != InputDeviceChange.Disabled)) return;
        _cursorDevice = null;
        Cursor.visible = true;
    }
    public int SelectedItemSlot { get; private set; } = -1;
    public event Action<int> OnItemSlotUsedEvent;

    private void ConfigureItemControls()
    {
        var map = _playerInputs.Gameplay.Get();
        foreach (string name in new[] { "Previous Item Slot", "Next Item Slot", "Use Selected Item" })
            map.FindAction(name)?.RemoveAction();
        ItemAction(map, "Item 1", "<Gamepad>/dpad/up");
        ItemAction(map, "Item 2", "<Gamepad>/dpad/right");
        ItemAction(map, "Item 3", "<Gamepad>/dpad/down");
        ItemAction(map, "Item 4", "<Gamepad>/dpad/left");
        var apply = map.FindAction("Use Selected Item") ?? map.AddAction("Use Selected Item", InputActionType.Button);
        apply.AddBinding("<Gamepad>/leftTrigger", groups: ControllerScheme);
        apply.performed += ApplySelectedItem;
        var drop = _playerInputs.Gameplay.Drop;
        for (int i = 0; i < drop.bindings.Count; i++)
            if (InputBinding.MaskByGroup(ControllerScheme).Matches(drop.bindings[i]) &&
                drop.bindings[i].path == "<Gamepad>/dpad/down") drop.ChangeBinding(i).WithPath("<Gamepad>/leftShoulder");
    }

    private static InputAction ItemAction(InputActionMap map, string name, string path)
    {
        var action = map.FindAction(name) ?? map.AddAction(name, InputActionType.Button);
        action.expectedControlType = "Button";
        for (int i = 0; i < action.bindings.Count; i++)
        {
            var binding = action.bindings[i];
            if (!InputBinding.MaskByGroup(ControllerScheme).Matches(binding)) continue;
            return action;
        }
        action.AddBinding(path, groups: ControllerScheme);
        return action;
    }

    private bool SelectControllerItemSlot(InputAction.CallbackContext context, int index)
    {
        if (!(context.control.device is Gamepad)) return false;
        if (!context.started) return true;
        SelectedItemSlot = SelectedItemSlot == index ? -1 : index;
        OnItemSlotUsedEvent?.Invoke(SelectedItemSlot);
        return true;
    }

    private void ApplySelectedItem(InputAction.CallbackContext context)
    {
        if (SelectedItemSlot < 0) return;
        switch (SelectedItemSlot)
        {
            case 0: OnItem1Event?.Invoke(true, 0); break;
            case 1: OnItem2Event?.Invoke(true, 1); break;
            case 2: OnItem3Event?.Invoke(true, 2); break;
            case 3: OnItem4Event?.Invoke(true, 3); break;
        }
    }
    public void ClearItemSelection()
    {
        SelectedItemSlot = -1;
        OnItemSlotUsedEvent?.Invoke(-1);
    }
    public string GetBindingDisplayString(InputAction action)
    {
        // References from the inspector belong to the asset, not our runtime copy.
        var runtimeAction = _playerInputs.asset.FindAction(action.id.ToString()) ?? action;
        return runtimeAction.GetBindingDisplayString(InputBinding.MaskByGroup(ActiveControlScheme));
    }

    private void EnsureUpgradeControllerBinding()
    {
        var action = _playerInputs.Gameplay.Upgrade;
        foreach (var binding in action.bindings)
            if (InputBinding.MaskByGroup(ControllerScheme).Matches(binding)) return;
        // Keep the menu accessible if Unity regenerates the wrapper from an older action asset.
        action.AddBinding("<Gamepad>/select", groups: ControllerScheme);
    }

    private void NotifyBindingsChanged() => OnBindingsChanged?.Invoke();

    private void SetActiveDevice(InputDevice device)
    {
        _lastDevice = device;
        string scheme = device is Gamepad ? ControllerScheme : KeyboardMouseScheme;
        if (ActiveControlScheme == scheme) return;
        ActiveControlScheme = scheme;
        NotifyBindingsChanged();
    }

    private void OnDeviceInput(InputEventPtr eventPtr, InputDevice device)
    {
        if (IsMeaningfulInput(eventPtr, device)) SetActiveDevice(device);
    }

    public static bool IsMeaningfulInput(InputEventPtr eventPtr, InputDevice device)
    {
        if (eventPtr.type != StateEvent.Type && eventPtr.type != DeltaStateEvent.Type) return false;
        if (!(device is Keyboard) && !(device is Mouse) && !(device is Gamepad)) return false;

        foreach (var control in eventPtr.EnumerateChangedControls(device, 0.25f))
        {
            if (control is ButtonControl button)
            {
                if (button.ReadValueFromEvent(eventPtr) < button.pressPointOrDefault) continue;
            }
            else if (device is Mouse mouse)
            {
                // Absolute pointer position and tiny delta jitter are not deliberate input.
                if (control != mouse.delta.x && control != mouse.delta.y &&
                    control != mouse.scroll.x && control != mouse.scroll.y) continue;
                if (Mathf.Abs(((AxisControl)control).ReadValueFromEvent(eventPtr)) < 2f) continue;
            }
            else if (!(device is Gamepad)) continue;

            return true;
        }
        return false;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device != _lastDevice || (change != InputDeviceChange.Disconnected &&
            change != InputDeviceChange.Removed && change != InputDeviceChange.Disabled)) return;
        _lastDevice = null;
        if (ActiveControlScheme == KeyboardMouseScheme) return;
        ActiveControlScheme = KeyboardMouseScheme;
        NotifyBindingsChanged();
    }

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
