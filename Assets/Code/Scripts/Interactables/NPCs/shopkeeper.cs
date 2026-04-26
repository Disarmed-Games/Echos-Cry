using AudioSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShopKeeper : MonoBehaviour
{
    [SerializeField] private TextAsset inkJSON;
    [SerializeField] private GameObject ToolTipPrefab;
    [SerializeField] private soundEffect shopOpenSFX;
    [SerializeField] private InputTranslator translator;
    [SerializeField] private float _distanceCheck;
    private bool _playerInRange;

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
                $"'{translator.PlayerInputs.Gameplay.Interact.GetBindingDisplayString(InputBinding.MaskByGroup("KeyboardMouse"))}' to Shop";
            Instantiate(ToolTipPrefab, this.transform.position + new Vector3(0, 0.5f, -1), Quaternion.identity);
        }
    }

    private void Update()
    {
        _playerInRange = (Vector3.Distance(Player.Instance.transform.position, transform.position) <= _distanceCheck);
    }

    private void RequestOpenShop()
    {
        if (_playerInRange)
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
