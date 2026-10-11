using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerItemSelectionTests : InputTestFixture
{
    [Test]
    public void DpadUsesItsSlotAndSticksDoNotSelectOrUseItems()
    {
        var pad = InputSystem.AddDevice<Gamepad>();
        var keyboard = InputSystem.AddDevice<Keyboard>();
        var translator = ScriptableObject.CreateInstance<InputTranslator>();
        try
        {
            int uses = 0;
            int usedSlot = -1;
            int keyboardUses = 0;
            int flashes = 0;
            void Used(bool pressed, int slot) { if (pressed) { uses++; usedSlot = slot; } }
            translator.OnItem1Event += Used;
            translator.OnItem2Event += Used;
            translator.OnItem3Event += Used;
            translator.OnItem4Event += Used;
            translator.OnItemSlotUsedEvent += slot => flashes++;
            Assert.That(translator.SelectedItemSlot, Is.EqualTo(-1));
            Set(pad.leftStick, Vector2.left);
            Set(pad.rightStick, Vector2.right);
            Assert.That(translator.SelectedItemSlot, Is.EqualTo(-1));
            Assert.That(uses, Is.Zero);
            Assert.That(flashes, Is.Zero);
            Press(pad.dpad.up);
            Assert.That(usedSlot, Is.Zero);
            Release(pad.dpad.up);
            Press(pad.dpad.right);
            Assert.That(usedSlot, Is.EqualTo(1));
            Release(pad.dpad.right);
            Press(pad.dpad.down);
            Assert.That(usedSlot, Is.EqualTo(2));
            Release(pad.dpad.down);
            Press(pad.dpad.left);
            Assert.That(usedSlot, Is.EqualTo(3));
            Assert.That(flashes, Is.EqualTo(4));
            Assert.That(uses, Is.EqualTo(4));
            Release(pad.dpad.left);
            Press(pad.rightShoulder);
            Press(pad.leftTrigger);
            Assert.That(uses, Is.EqualTo(4));
            translator.OnItem1Event += (pressed, slot) => { if (pressed) keyboardUses++; };
            Press(keyboard.digit1Key);
            Assert.That(keyboardUses, Is.EqualTo(1));
            Assert.That(flashes, Is.EqualTo(4), "Keyboard hotkeys do not flash controller slots.");
            translator.PlayerInputs.Gameplay.Disable();
            Press(pad.dpad.up);
            Assert.That(flashes, Is.EqualTo(4));
        }
        finally
        {
            Object.DestroyImmediate(translator);
        }
    }
}
