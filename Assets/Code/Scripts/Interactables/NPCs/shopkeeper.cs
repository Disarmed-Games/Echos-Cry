using AudioSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShopKeeper : MonoBehaviour
{
    [SerializeField] private TextAsset inkJSON;
    [SerializeField] private GameObject ToolTipPrefab;
    [SerializeField] private soundEffect shopOpenSFX;
    [SerializeField] private InputTranslator translator;

    private Collider currentPlayer;
    
    private void CloseShop()
    {
        MenuManager.Instance.SetMenu("HUD");
        VolumeManager.Instance.SetDepthOfField(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ToolTipPrefab.GetComponent<ToolTip>().text =
                $"Press '{translator.PlayerInputs.Gameplay.Interact.GetBindingDisplayString(InputBinding.MaskByGroup("KeyboardMouse"))}' to Shop";
            Instantiate(ToolTipPrefab, this.transform.position + new Vector3(0, 1, -1), Quaternion.identity);

            currentPlayer = other;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            currentPlayer = null;
        }
    }

    private void RequestOpenShop()
    {
        if (currentPlayer != null)
        {
            DialogueManager.Instance.EnterDialogueMode(inkJSON);

            SoundEffectManager.Instance.Builder
                .SetSound(shopOpenSFX)
                .SetSoundPosition(this.transform.position)
                .ValidateAndPlaySound();
        }
    }

    void OnEnable()
    {
        translator.OnCloseShopEvent += CloseShop;
        translator.OnInteractEvent += RequestOpenShop;
    }
    void OnDisable()
    {
        translator.OnCloseShopEvent -= CloseShop;
        translator.OnInteractEvent -= RequestOpenShop;
    }
}
