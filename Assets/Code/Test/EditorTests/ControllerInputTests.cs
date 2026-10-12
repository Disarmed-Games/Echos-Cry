using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class ControllerInputTests : InputTestFixture
{
    private InputTranslator translator;
    private Gamepad pad;
    private Keyboard keyboard;
    private Mouse mouse;

    public override void Setup()
    {
        base.Setup();
        pad = InputSystem.AddDevice<Gamepad>();
        keyboard = InputSystem.AddDevice<Keyboard>();
        mouse = InputSystem.AddDevice<Mouse>();
        typeof(InputTranslator).GetMethod("InitializeCursorVisibility",
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).Invoke(null, null);
        translator = ScriptableObject.CreateInstance<InputTranslator>();
    }

    public override void TearDown()
    {
        Object.DestroyImmediate(translator);
        base.TearDown();
    }

    [Test]
    public void EmulatedMouseBeforeTriggerInSameUpdateDoesNotAttack()
    {
        int normal = 0, special = 0;
        translator.OnPrimaryActionEvent += value => normal += value ? 1 : 0;
        translator.OnSpecialAttackEvent += value => special += value ? 1 : 0;
        InputSystem.QueueStateEvent(mouse, new MouseState().WithButton(MouseButton.Left));
        InputSystem.QueueStateEvent(pad, new GamepadState { rightTrigger = 1f });
        InputSystem.Update();
        Assert.That(normal, Is.Zero);
        Assert.That(special, Is.EqualTo(1));
    }

    [Test]
    public void PartialTriggerDoesNotRequestSpecialOrEmulatedNormalAttack()
    {
        int normal = 0, special = 0;
        translator.OnPrimaryActionEvent += value => normal += value ? 1 : 0;
        translator.OnSpecialAttackEvent += value => special += value ? 1 : 0;
        Set(pad.rightTrigger, 0.1f);
        Press(mouse.leftButton);
        Assert.That(normal, Is.Zero);
        Assert.That(special, Is.Zero);
        Set(pad.rightTrigger, 1f);
        Assert.That(special, Is.EqualTo(1));
    }

    [Test]
    public void TriggerOnlyRequestsSpecialEvenWithEmulatedMouseClick()
    {
        int normal = 0, special = 0;
        translator.OnPrimaryActionEvent += value => normal += value ? 1 : 0;
        translator.OnSecondaryActionEvent += value => normal += value ? 1 : 0;
        translator.OnSpecialAttackEvent += value => special += value ? 1 : 0;
        Press(pad.rightTrigger);
        Press(mouse.leftButton);
        Assert.That(normal, Is.Zero);
        Assert.That(special, Is.EqualTo(1));
    }

    [Test]
    public void PlayStationCrossDoesNotAttackEvenWithEmulatedSpace()
    {
        int attacks = 0;
        translator.OnPrimaryActionEvent += value => attacks += value ? 1 : 0;
        translator.OnSecondaryActionEvent += value => attacks += value ? 1 : 0;
        translator.OnSpecialAttackEvent += value => attacks += value ? 1 : 0;
        Press(pad.buttonSouth);
        Press(keyboard.spaceKey);
        Press(mouse.leftButton);
        Assert.That(attacks, Is.Zero);
    }

    [Test]
    public void RightStickCursorIgnoresEmulatedMouseAndOtherControllerInputHidesIt()
    {
        Set(pad.rightStick, Vector2.right);
        InputSystem.QueueStateEvent(mouse, new MouseState { delta = new Vector2(10, 0) });
        InputSystem.Update();
        Set(pad.rightStick, Vector2.zero);
        Assert.That(translator.ActiveControlScheme, Is.EqualTo("Controller"));
        Assert.That(Cursor.visible, Is.True);
        Press(pad.buttonSouth);
        Assert.That(Cursor.visible, Is.False);
    }

    [Test]
    public void RightStickCursorHidesAfterTimeout()
    {
        Set(pad.rightStick, Vector2.right);
        Set(pad.rightStick, Vector2.zero);
        typeof(InputTranslator).GetField("_stickCursorUntil",
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
            .SetValue(null, Time.realtimeSinceStartup - 1f);
        InputSystem.Update();
        Assert.That(Cursor.visible, Is.False);
        Assert.That(translator.ActiveControlScheme, Is.EqualTo("Controller"));
    }

    [Test]
    public void PromptsFollowLastDeviceEvenWhenGameplayIsDisabled()
    {
        translator.PlayerInputs.Disable();
        int changes = 0;
        translator.OnBindingsChanged += () => changes++;
        Press(pad.buttonSouth);
        Assert.That(translator.ActiveControlScheme, Is.EqualTo("Controller"));
        Assert.That(translator.GetBindingDisplayString(translator.PlayerInputs.Gameplay.Interact), Is.Not.Empty);
        Press(keyboard.wKey);
        Assert.That(translator.ActiveControlScheme, Is.EqualTo("KeyboardMouse"));
        Press(pad.buttonWest);
        Assert.That(translator.ActiveControlScheme, Is.EqualTo("Controller"));
        Assert.That(changes, Is.EqualTo(3));
    }

    [Test]
    public void ReleasesStickDriftAndPointerPositionDoNotSwitchPrompts()
    {
        Press(pad.buttonSouth);
        Release(pad.buttonSouth);
        Set(pad.leftStick, new Vector2(0.1f, 0.1f));
        InputSystem.QueueStateEvent(mouse, new MouseState { position = new Vector2(500, 500), delta = Vector2.one });
        InputSystem.Update();
        Assert.That(translator.ActiveControlScheme, Is.EqualTo("Controller"));
        Press(keyboard.wKey);
        Release(keyboard.wKey);
        Set(pad.leftStick, Vector2.zero);
        Assert.That(translator.ActiveControlScheme, Is.EqualTo("KeyboardMouse"));
        Set(pad.leftStick, Vector2.left);
        Assert.That(translator.ActiveControlScheme, Is.EqualTo("Controller"));
    }

    [Test]
    public void MouseClicksAndDeliberateMovementSwitchToKeyboardMouse()
    {
        Press(pad.buttonSouth);
        Press(mouse.leftButton);
        Assert.That(translator.ActiveControlScheme, Is.EqualTo("KeyboardMouse"));
        Press(pad.buttonWest);
        InputSystem.QueueStateEvent(mouse, new MouseState { delta = new Vector2(10, 0) });
        InputSystem.Update();
        Assert.That(translator.ActiveControlScheme, Is.EqualTo("KeyboardMouse"));
    }

    [Test]
    public void ControllerMovementAndAttackReachExistingCallbacks()
    {
        Vector2 movement = Vector2.zero;
        bool attacked = false;
        translator.OnMovementEvent += value => movement = value;
        translator.OnPrimaryActionEvent += value => attacked |= value;
        Set(pad.leftStick, Vector2.right);
        Press(pad.buttonWest);
        Assert.That(movement.x, Is.GreaterThan(0.9f));
        Assert.That(attacked, Is.True);
    }

    [Test]
    public void ActionsHaveBothSchemesAndDisconnectFallsBack()
    {
        foreach (var map in translator.PlayerInputs.asset.actionMaps)
            foreach (var action in map.actions)
            {
                Assert.That(action.GetBindingDisplayString(InputBinding.MaskByGroup("Controller")), Is.Not.Empty, action.name);
                Assert.That(action.GetBindingDisplayString(InputBinding.MaskByGroup("KeyboardMouse")), Is.Not.Empty, action.name);
            }
        Press(pad.buttonSouth);
        InputSystem.RemoveDevice(pad);
        Assert.That(translator.ActiveControlScheme, Is.EqualTo("KeyboardMouse"));
    }

    [Test]
    public void InspectorActionReferencesUseRuntimeOverrides()
    {
        using (var referenceInputs = new PlayerInputs())
        {
            var action = translator.PlayerInputs.Gameplay.Interact;
            action.ApplyBindingOverride(0, "<Keyboard>/f");
            Assert.That(translator.GetBindingDisplayString(referenceInputs.Gameplay.Interact),
                Is.EqualTo(action.GetBindingDisplayString(InputBinding.MaskByGroup("KeyboardMouse"))));
        }
    }
}
