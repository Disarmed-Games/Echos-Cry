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
        translator = ScriptableObject.CreateInstance<InputTranslator>();
    }

    public override void TearDown()
    {
        Object.DestroyImmediate(translator);
        base.TearDown();
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
    public void EveryActionHasBothSchemesAndDisconnectFallsBack()
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
