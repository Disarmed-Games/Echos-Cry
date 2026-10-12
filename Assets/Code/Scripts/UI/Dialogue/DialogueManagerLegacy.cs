using UnityEngine;
using Ink.Runtime;
using Unity.Hierarchy;
using System.Collections;

public class DialogueManagerLegacy : MonoBehaviour
{
    [Header("Ink Story")]
    [SerializeField] private TextAsset _inkJson;
    [SerializeField] private BoolEventChannel _lockMovementChannel;
    [SerializeField] private InputTranslator _inputTranslator;
    private Story _story;
    private int _currentChoiceIndex = -1;

    private bool _dialoguePlaying = false;

    private void Awake()
    {
        _story = new Story(_inkJson.text);
    }
    private void OnEnable()
    {
        DialogueEvents.Instance.OnEnterDiaglogue += EnterDialogue;
        DialogueEvents.Instance.OnSubmitPressed += SubmitPressed;
        DialogueEvents.Instance.OnUpdateChoiceIndex += UpdateChoiceIndex;
    }
    private void OnDisable()
    {
        DialogueGameplayPause.Release(this);
        DialogueEvents.Instance.OnEnterDiaglogue -= EnterDialogue;
        DialogueEvents.Instance.OnSubmitPressed -= SubmitPressed;
        DialogueEvents.Instance.OnUpdateChoiceIndex -= UpdateChoiceIndex;
    }

    private void UpdateChoiceIndex(int choiceIndex)
    {
        this._currentChoiceIndex = choiceIndex;
    }

    private void SubmitPressed()
    {
        if (!_dialoguePlaying) return;

        ContinueOrExitStory();
    }

    private void EnterDialogue(string knotName)
    {
        if (_dialoguePlaying) return;
        _dialoguePlaying = true;
        DialogueGameplayPause.Acquire(this);

        _inputTranslator.PlayerInputs.Gameplay.Disable();
        _inputTranslator.PlayerInputs.Dialogue.Enable();

        DialogueEvents.Instance.DialogueStarted();
        CameraManager.Instance.ZoomInCamera(5f, .8f);

        _lockMovementChannel.Invoke(true);

        if (!knotName.Equals(""))
        {
            _story.ChoosePathString(knotName);
        }

        ContinueOrExitStory();
    }

    private void ContinueOrExitStory()
    {
        if (_story.currentChoices.Count > 0 && _currentChoiceIndex != -1)
        {
            _story.ChooseChoiceIndex(_currentChoiceIndex);
            _currentChoiceIndex = -1;
        }

        if (_story.canContinue)
        {
            string dialogueLine = _story.Continue();
            DialogueEvents.Instance.DisplayDialogue(dialogueLine, _story.currentChoices);
        }
        else if (_story.currentChoices.Count == 0)
        {
            StartCoroutine(ExitDialogue());
        }
    }
    private IEnumerator ExitDialogue()
    {
        yield return null; //Avoiding race conditions.
        //Debug.Log("Exiting dialogue.");
        _dialoguePlaying = false;
        DialogueGameplayPause.Release(this);

        _inputTranslator.PlayerInputs.Gameplay.Enable();
        _inputTranslator.PlayerInputs.Dialogue.Disable();

        DialogueEvents.Instance.DialogueEnded();
        CameraManager.Instance.ZoomOutCamera(.8f);

        _lockMovementChannel.Invoke(false);

        _story.ResetState();
    }
}
