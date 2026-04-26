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
    [SerializeField] private float _distanceCheck;
    private bool _playerInRange;

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
        if (DialogueManager.Instance == null) Debug.Log("Whered you go");
        if (_playerInRange && !DialogueManager.Instance.isDialoguePlaying)
        {
            DialogueManager.Instance.EnterDialogueMode(inkJSON);
        }
    }

    private void Update()
    {
        _playerInRange = (Vector3.Distance(Player.Instance.transform.position, transform.position) <= _distanceCheck);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ToolTipPrefab.GetComponent<ToolTip>().text =
                $"'{_inputTranslator.PlayerInputs.Gameplay.Interact.GetBindingDisplayString(InputBinding.MaskByGroup("KeyboardMouse"))}'";
            Instantiate(ToolTipPrefab, this.transform.position + new Vector3(0, 1, -1), Quaternion.identity);
        }
    }
}