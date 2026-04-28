using UnityEngine;
using UnityEngine.InputSystem;

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

                    if (string.IsNullOrEmpty(binding.groups) || !binding.groups.Contains("KeyboardMouse"))
                        continue;

                    string actionName = action.name;
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
    }
}
