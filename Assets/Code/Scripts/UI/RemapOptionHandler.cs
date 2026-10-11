using System;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.InputSystem;

public class RemapOptionHandler : MonoBehaviour
{
    public static Action OnActionRemappedEvent;
    [SerializeField] private TextMeshProUGUI _ActionNameText;
    [SerializeField] private TextMeshProUGUI _ActionBindingText;

    private InputAction _inputAction;
    private int _bindingIndex;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperationKeyboard;
    private readonly List<InputAction> previouslyEnabledActions = new List<InputAction>();

    public void SetupAction(string name, string binding, InputAction action, int bindingIndex)
    {
        _ActionNameText.text = name;
        _ActionBindingText.text = binding;
        _inputAction = action;
        _bindingIndex = bindingIndex;
    }

    public void OnRemapButtonPressed()
    {
        if (rebindingOperationKeyboard != null) return;
        _ActionBindingText.text = "Awaiting New Input...";
        // Prevent pause/interact bindings from closing this menu during a rebind.
        previouslyEnabledActions.Clear();
        if (_inputAction.actionMap?.asset != null)
        {
            foreach (var map in _inputAction.actionMap.asset.actionMaps)
                foreach (var action in map.actions)
                    if (action.enabled) previouslyEnabledActions.Add(action);
        }
        else if (_inputAction.enabled) previouslyEnabledActions.Add(_inputAction);
        foreach (var action in previouslyEnabledActions) action.Disable();
        rebindingOperationKeyboard = _inputAction.PerformInteractiveRebinding()
            .WithTargetBinding(_bindingIndex)
            .WithControlsExcluding("<Mouse>/position")
            .WithControlsExcluding("<Mouse>/delta")
            .WithCancelingThrough("<Keyboard>/escape")
            .OnCancel(OnRebindingOperationCanceled)
            .OnComplete(OnRebindingOperationComplete)
            .OnMatchWaitForAnother(0.1f);
        bool controller = InputBinding.MaskByGroup(InputTranslator.ControllerScheme).Matches(_inputAction.bindings[_bindingIndex]);
        if (controller) rebindingOperationKeyboard.WithControlsHavingToMatchPath("<Gamepad>");
        else
        {
            rebindingOperationKeyboard.WithControlsHavingToMatchPath("<Keyboard>");
            rebindingOperationKeyboard.WithControlsHavingToMatchPath("<Mouse>");
        }
        rebindingOperationKeyboard.Start();
    }

    private void OnRebindingOperationComplete(InputActionRebindingExtensions.RebindingOperation operation)
    {
        RestoreActions();
        _ActionBindingText.text = operation.action.GetBindingDisplayString(_bindingIndex);
        rebindingOperationKeyboard.Dispose();
        rebindingOperationKeyboard = null;
        OnActionRemappedEvent?.Invoke();
    }

    private void OnRebindingOperationCanceled(InputActionRebindingExtensions.RebindingOperation operation)
    {
        RestoreActions();
        _ActionBindingText.text = operation.action.GetBindingDisplayString(_bindingIndex);
        operation.Dispose();
        rebindingOperationKeyboard = null;
    }

    private void OnDisable()
    {
        rebindingOperationKeyboard?.Cancel();
    }

    private void RestoreActions()
    {
        foreach (var action in previouslyEnabledActions) action.Enable();
        previouslyEnabledActions.Clear();
    }
}
