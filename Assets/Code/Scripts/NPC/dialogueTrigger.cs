using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;
using Unity.Cinemachine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private TextAsset inkJSON;
    [SerializeField] private GameObject ToolTipPrefab;
    [SerializeField] private InputTranslator _inputTranslator;
    private Collider currentPlayer;

    void OnEnable()
    {
        _inputTranslator.OnInteractEvent += InteractCheck;
    }
    void OnDisable()
    {
        _inputTranslator.OnInteractEvent -= InteractCheck;
    }

    private void InteractCheck()
    {
        if (currentPlayer != null && !DialogueManager.Instance.isDialoguePlaying)
        {
            DialogueManager.Instance.EnterDialogueMode(inkJSON);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ToolTipPrefab.GetComponent<ToolTip>().text =
                $"Press '{_inputTranslator.PlayerInputs.Gameplay.Interact.GetBindingDisplayString(InputBinding.MaskByGroup("KeyboardMouse"))}' to talk.";
            Instantiate(ToolTipPrefab, this.transform.position + new Vector3(0, 1, -1), Quaternion.identity);
            
            currentPlayer = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            currentPlayer = null;

            if (DialogueManager.Instance.isDialoguePlaying)
            {
                StartCoroutine(DialogueManager.Instance.ExitDialogueMode());
            }
        }
    }
}