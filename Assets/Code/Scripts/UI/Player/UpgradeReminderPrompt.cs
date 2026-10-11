using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UpgradeReminderPrompt : MonoBehaviour
{
    [SerializeField] private InputTranslator translator;
    private TextMeshProUGUI prompt;

    private void OnEnable()
    {
        prompt = GetComponent<TextMeshProUGUI>();
        if (translator == null)
        {
            var player = Player.Instance;
            if (player != null) translator = player.InputTranslator;
        }
        if (translator != null) translator.OnBindingsChanged += Refresh;
        Refresh();
    }

    private void Start() => Refresh();

    private void OnDisable()
    {
        if (translator != null) translator.OnBindingsChanged -= Refresh;
    }

    private void Refresh()
    {
        if (translator == null || translator.PlayerInputs == null) return;
        prompt.text = $"Press '{translator.GetBindingDisplayString(translator.PlayerInputs.Gameplay.Upgrade)}' to upgrade!";
    }
}
