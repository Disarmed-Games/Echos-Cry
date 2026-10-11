using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControlRemapManager : MonoBehaviour
{
    [SerializeField] private GameObject _controlsContainer;
    [SerializeField] private GameObject _remapOptionPrefab;
    [SerializeField] private InputTranslator _inputTranslator;

    private void Start()
    {
        foreach (var actionMap in _inputTranslator.PlayerInputs.asset.actionMaps)
        {
            foreach (var action in actionMap.actions)
            {
                for (int i = 0; i < action.bindings.Count; i++)
                {
                    var binding = action.bindings[i];

                    if (binding.isComposite)
                        continue;

                    bool controller = InputBinding.MaskByGroup(InputTranslator.ControllerScheme).Matches(binding);
                    bool keyboardMouse = InputBinding.MaskByGroup(InputTranslator.KeyboardMouseScheme).Matches(binding);
                    if (!controller && !keyboardMouse)
                        continue;

                    string actionName = action.name + (controller ? " [Controller]" : " [Keyboard/Mouse]");
                    if (binding.isPartOfComposite)
                    {
                        actionName += $" ({binding.name})";
                    }

                    GameObject instance = Instantiate(_remapOptionPrefab, _controlsContainer.transform);

                    instance.GetComponent<RemapOptionHandler>().SetupAction(
                        actionName,
                        action.GetBindingDisplayString(i),
                        action,
                        i
                    );
                }
            }
        }
        var scroll = _controlsContainer.GetComponentInParent<ScrollRect>();
        if (scroll != null && _controlsContainer.GetComponent<VisibleBindingNavigation>() == null)
            _controlsContainer.AddComponent<VisibleBindingNavigation>().Setup(scroll);
    }
}
