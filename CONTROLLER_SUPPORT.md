Controller support
==================

Both input schemes remain usable. The most recent deliberate keyboard, mouse,
or gamepad activity chooses the displayed button bindings, including while
gameplay actions are disabled for menus/dialogue. Releases, small stick drift,
absolute pointer position changes, and mouse deltas below two pixels do not switch
prompts. Disconnecting the last-used controller returns prompts to keyboard/mouse.

Default gamepad layout (Xbox / PlayStation labels)
------------------------------------------------

| Action | Binding |
| --- | --- |
| Move / face | Left stick |
| Light attack | X / Square |
| Heavy attack | Y / Triangle |
| Dash | B / Circle |
| Interact / dialogue continue / UI submit | A / Cross |
| Special attack | Right trigger |
| Select and use item slot 1 / 2 / 3 / 4 | D-pad up / right / down / left |
| Drop last-used inventory slot | L1 / Left bumper |
| Pause / resume | Start / Options |
| Upgrades | View / Share |
| Close shop | B / Circle |
| Developer teleport | Right stick click (still uses the mouse position) |

The HUD upgrade reminder reads the Upgrade action's active binding. It switches
between Tab and the controller's View/Select label when the input device changes,
and refreshes after rebinding. UpgradeReminderPrompt is already attached to the
reminder text in the MainCanvas prefab.

ControllerUIFocus is attached to the EventSystems in the main scenes. Other scenes
receive it automatically when entering Play Mode or loading a scene. For a custom
EventSystem instantiated later, add ControllerUIFocus to that EventSystem; the
ControllerFocusOutline graphic is created automatically and needs no manual setup.

The Unity UI input module's existing default bindings handle menu navigation and
submission. A runtime component on each EventSystem supplies selection when the
controller is active and the current selection is missing or disabled. Buttons,
sliders, toggles, and other selected controls receive a gold tint and display a bright
gold frame with a dark border while controller input is active. The frame uses a
separate screen overlay so menu masks and canvas sorting cannot obscure it. The frame hides when keyboard
or mouse input takes over and does not block clicks or affect the UI layout. Inventory
settings tabs support selection. Gameplay inventory slots are excluded from
EventSystem navigation, so neither stick changes their focus. Each D-pad direction
selects and uses its corresponding slot in one press. No slot is highlighted by
default; a press flashes that slot's gold border for 0.2 seconds, then clears it.
Slot labels show their D-pad bindings. Keyboard keys 1–4 still use their slots
directly, and mouse hover plus Q still drops the hovered item. L1 drops the last
slot used with the controller; R1/L2 no longer select or use items.

The controls page lists bindings for both schemes. Rebinding restricts candidates
to the selected scheme, pauses the translator's enabled actions during capture,
supports Escape cancellation, and restores their previous enabled state. Changes
refresh prompts. Binding persistence remains as previously implemented by the project;
this change does not add saving across application restarts.

Pause navigation follows Continue, Settings, Main Menu, Exit in both directions.
The Main Menu confirmation selects its Yes/No buttons and returns focus to Main
Menu when canceled. Resolution dropdown options keep focus during the opening fade
and scroll into view as the controller moves between options; Submit chooses one.

The bindings list limits up/down navigation to fully visible buttons. Focus stops
at the first/last visible row. Use the right stick to scroll the list, or move right
to its scrollbar. If scrolling hides the selected binding, focus moves to the nearest
visible row. Mouse scrolling and clicking continue to work.
From a binding row, Left returns to the Controls tab and Right moves to the scrollbar.
From the scrollbar, Left returns to the last visible focused row and Right moves to
Back; Up/Down still scroll. From the Controls tab, Down re-enters the list and Up
moves to Back. These routes are restored to their previous settings when the controls
page closes, so they do not change navigation on other settings pages.

The upgrade menu selects the first available Use button on opening. Up/Down moves
between available upgrades and Back; Left from any Use button returns to Back,
and Right from Back returns to the first available upgrade. Focus shows the upgrade
card highlight and description. Locked upgrades remain disabled until an upgrade
point is available; after spending the final point, focus returns to Back.
The cards' shared description label is updated by the upgrade menu, so an unfocused
card cannot overwrite another card's stats. Mouse hover and controller selection
use the most recently active input device. The XP HUD refreshes from PlayerXP when
it opens, including level changes that occurred while the HUD was hidden.

Validation
----------

Run `ControllerInputTests` from Unity's EditMode Test Runner. The tests cover
device switching with gameplay disabled, drift/releases, mouse activity, movement
and attack callbacks, bindings on every action, disconnection, and runtime overrides.
`ControllerNavigationTests` covers the pause navigation chain, binding-list boundaries
and selection after scrolling, and opening/navigating/submitting a dropdown option.

For a physical controller check, open gameplay and verify movement, each attack,
dash, interaction, item hotkeys, pause/resume, settings navigation/rebinding, shop
navigation, and dialogue choices. While a prompt is visible, press W, then a
controller button, and confirm it updates immediately. Repeat while paused, and
unplug the controller. Check selected-slot dropping and tooltip placement as well.
