using UnityEngine;
using UnityEngine.InputSystem;

public abstract class DoorManager : MonoBehaviour
{
    [SerializeField] protected Animator doorAnimator;
    [SerializeField] protected GameObject ToolTipPrefab;
    [SerializeField] protected InputTranslator translator;
    [SerializeField] protected AudioSystem.soundEffect doorOpenSoundEffect;

    protected bool playerInRange = false;
    protected bool isOpen = false;
    protected bool isLocked = false;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!isOpen)
        {
            if (!isLocked)
            {
                ToolTipPrefab.GetComponent<ToolTip>().text =
                    $"'{translator.PlayerInputs.Gameplay.Interact.GetBindingDisplayString(InputBinding.MaskByGroup("KeyboardMouse"))}' to Open";
                Instantiate(ToolTipPrefab, this.transform.position + new Vector3(0, 2, -1), Quaternion.identity);
            }
            else
            {
                ToolTipPrefab.GetComponent<ToolTip>().text = "This Door is Locked.";
                Instantiate(ToolTipPrefab, this.transform.position + new Vector3(0, 2, -1), Quaternion.identity);
            }

            playerInRange = true;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        playerInRange = false;
    }

    protected virtual void OpenDoor()
    {
        if (playerInRange && !isLocked && !isOpen)
        {
            SoundEffectManager.Instance.Builder
            .SetSound(doorOpenSoundEffect)
            .SetSoundPosition(transform.position)
            .ValidateAndPlaySound();

            doorAnimator.SetTrigger("Interact");
            isOpen = true;
        }
    }

    protected virtual void OpenDoorWithoutPlayer()
    {
        if (!isLocked && !isOpen)
        {
            SoundEffectManager.Instance.Builder
            .SetSound(doorOpenSoundEffect)
            .SetSoundPosition(transform.position)
            .ValidateAndPlaySound();

            doorAnimator.SetTrigger("Interact");
            isOpen = true;
        }
    }

    protected void OnEnable()
    {
        translator.OnInteractEvent += OpenDoor;
    }

    protected void OnDisable()
    {
        translator.OnInteractEvent -= OpenDoor;
    }
}
