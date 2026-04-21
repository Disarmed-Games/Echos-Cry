using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class DialogueManager : NonSpawnableSingleton<DialogueManager>
{
    public static event Action onDialogueStarted;
    public static event Action onDialogueEnded;

    [SerializeField] private InputTranslator _inputTranslator;

    [Header("Dialogue Box UI")]
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Dialogue Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    private Story currentStory;
    public bool isDialoguePlaying { get; private set; }
    private bool canContinueToNextLine;
    private bool interruptTextDisplayer = false;

    [SerializeField] private float typingSpeed = 0.1f;
    
    private Coroutine displayLineCoroutine;
    private EventSystem currentEventSystem;


    void Start()
    {
        if (dialogueText == null) throw new Exception("Dialogue Text is null.");
        if (choices.Length == 0) throw new Exception("Choices List for UI Buttons is null.");
        if (_inputTranslator == null) throw new Exception("Input Translator is null.");

        interruptTextDisplayer = false;
        isDialoguePlaying = false;

        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
        currentEventSystem = EventSystem.current;
    }

    private void OnEnable()
    {
        _inputTranslator.OnSubmitEvent += ContinueIfPossible;
    }
    private void OnDisable()
    {
        _inputTranslator.OnSubmitEvent -= ContinueIfPossible;
    }
    public void EnterDialogueMode(TextAsset inkJSON)
    {       
        currentStory = new Story(inkJSON.text);
        isDialoguePlaying = true;
        onDialogueStarted?.Invoke(); 

        CameraManager.Instance.ZoomInCamera(5f, .8f);
        _inputTranslator.PlayerInputs.Gameplay.Disable();
        _inputTranslator.PlayerInputs.Dialogue.Enable();

        ContinueStory();
    }

    public IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.1f);
        isDialoguePlaying = false;
        dialogueText.text = "";
        onDialogueEnded?.Invoke();

        CameraManager.Instance.ZoomOutCamera(.8f);
        _inputTranslator.PlayerInputs.Dialogue.Disable();
        _inputTranslator.PlayerInputs.Gameplay.Enable();
    }

    public void ExitDialogueModeInstant()
    {
        isDialoguePlaying = false;
        dialogueText.text = "";
        CameraManager.Instance.ZoomOutCamera(.8f);
        _inputTranslator.PlayerInputs.Dialogue.Disable();
        _inputTranslator.PlayerInputs.Gameplay.Enable();
    }

    public void ContinueIfPossible(bool isPressed)
    {
        if (!isPressed) return;

        if (!canContinueToNextLine)
        {
            interruptTextDisplayer = true;
            return;
        }

        if (currentStory.currentChoices.Count > 0) return;

        ContinueStory();
    }

    private string ProcessLineVariables(string line)
    {
        if (currentStory.variablesState.GlobalVariableExistsWithName("actionName"))
        {
            string actionName = (string)currentStory.variablesState["actionName"];
            var action = _inputTranslator.PlayerInputs.asset.FindAction(actionName);
            string buttonName = action.GetBindingDisplayString(InputBinding.MaskByGroup("KeyboardMouse"));
            line = line.Replace("REPLACE_BUTTON", buttonName);
        }
        return line;
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue) 
        {
            if (displayLineCoroutine != null) 
            {
                StopCoroutine(displayLineCoroutine);
            }
            string nextLine = currentStory.Continue();

            //Check for shop tag
            foreach (string tag in currentStory.currentTags)
            {
                if (tag == "OPEN_SHOP")
                {
                    ExitDialogueModeInstant();
                    MenuManager.Instance.EnableShopMenu();
                    return;
                }
            }

            //Add in key binding names
            nextLine = ProcessLineVariables(nextLine);

            if (nextLine.Equals("") && !currentStory.canContinue)
            {
                StartCoroutine(ExitDialogueMode());
            } 
            
            else 
            {
                displayLineCoroutine = StartCoroutine(DisplayLine(nextLine));
            }
        }
        else 
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    private void HideChoices()
    {
        foreach (GameObject choiceButton in choices)
        {
            choiceButton.SetActive(false);
        }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        if (currentChoices.Count == 0)
        {
            HideChoices();
            return;
        }

        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("More choices were given than the UI could support: " + currentChoices.Count);
        }

        int index = 0;
        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

     private IEnumerator DisplayLine(string line) 
    {
        interruptTextDisplayer = false;

        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;
        HideChoices();
        canContinueToNextLine = false;

        bool isAddingRichTextTag = false;

        foreach (char letter in line.ToCharArray())
        {
            // if the submit button is pressed, finish up displaying the line right away
            if (interruptTextDisplayer) 
            {
                dialogueText.maxVisibleCharacters = line.Length;
                interruptTextDisplayer = false;
                break;
            }

            // check for rich text tag, if found, add it without waiting
            if (letter == '<' || isAddingRichTextTag) 
            {
                isAddingRichTextTag = true;
                if (letter == '>')
                {
                    isAddingRichTextTag = false;
                }
            }
            // if not rich text, add the next letter and wait a small time
            else 
            {
                //play dialogue sound here
                dialogueText.maxVisibleCharacters++;
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        DisplayChoices();

        canContinueToNextLine = true;
    }

    private IEnumerator SelectFirstChoice()
    {
        if (currentEventSystem == null)
            yield break;

        if (choices.Length == 0 || !choices[0].activeInHierarchy)
            yield break;

        currentEventSystem.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        currentEventSystem.SetSelectedGameObject(choices[0]);
    }

    public void MakeChoice(int choiceIndex)
    {
        if (canContinueToNextLine)
        {
            currentStory.ChooseChoiceIndex(choiceIndex);
            ContinueStory();
        }
    }
}