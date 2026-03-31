using AudioSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class UpgradeAnvil : MonoBehaviour
{
    private bool playerInRange = false;
    [SerializeField] private GameObject ToolTipPrefab;
    [SerializeField] private soundEffect shopOpenSFX;
    [SerializeField] private InputTranslator translator;

    private void OpenShop()
    {
        SoundEffectManager.Instance.Builder
            .SetSound(shopOpenSFX)
            .SetSoundPosition(this.transform.position)
            .ValidateAndPlaySound();

        MenuManager.Instance.EnableUpgradeMenu();
    }

    void OnTriggerEnter(Collider other)
    {
        ToolTipPrefab.GetComponent<ToolTip>().text =
            $"Press '{translator.PlayerInputs.Gameplay.Interact.GetBindingDisplayString(InputBinding.MaskByGroup("KeyboardMouse"))}' to Upgrade";
        Instantiate(ToolTipPrefab, this.transform.position + new Vector3(0, 1, -1), Quaternion.identity);
        playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        playerInRange = false;
    }

    void RequestOpenShop()
    {
        if (playerInRange)
        {
            OpenShop();
        }
    }

    void Start()
    {
        translator.OnInteractEvent += RequestOpenShop;
    }
    void OnDestroy()
    {
        translator.OnInteractEvent -= RequestOpenShop;
    }
}
