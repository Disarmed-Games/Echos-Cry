using System;
using UnityEditor;
using UnityEngine;
using Ink.Runtime;
using System.Collections.Generic;

public class DialogueEvents : MonoBehaviour
{
    public static DialogueEvents Instance { get; private set; }

    public event Action<string> OnEnterDiaglogue;
    public event Action OnSubmitPressed;
    public event Action OnDialogueStarted;
    public event Action OnDialogueEnded;
    public event Action<string, List<Choice>> OnDisplayDialogue;
    public event Action<int> OnUpdateChoiceIndex;
    public bool InDialogue = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple instances of DialogueEvents detected. There should only be one instance of DialogueEvents in the scene.");
        }
        Instance = this;
    }
    public void EnterDialogue(string knotName)
    {
        OnEnterDiaglogue?.Invoke(knotName);
    }

    public void SubmitPressed()
    {
        OnSubmitPressed?.Invoke();
    }

    public  void DialogueStarted()
    {
        OnDialogueStarted?.Invoke();
        InDialogue = true;
    }

    public void DialogueEnded()
    {
        OnDialogueEnded?.Invoke();
        InDialogue = false;
    }

    public void DisplayDialogue(string dialogueLine, List<Choice> dialogueChoices)
    {
        OnDisplayDialogue?.Invoke(dialogueLine, dialogueChoices);
    }

    public void UpdateChoiceIndex(int choiceIndex)
    {
        OnUpdateChoiceIndex?.Invoke(choiceIndex);
    }
}

// Dialogue owns its pause and restores the speed that was active before it opened.
internal static class DialogueGameplayPause
{
    private static readonly HashSet<object> owners = new HashSet<object>();
    private static float previousTimeScale = 1f;
    public static bool IsPaused => owners.Count > 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset()
    {
        owners.Clear();
        previousTimeScale = 1f;
    }

    public static void Acquire(object owner)
    {
        if (owners.Contains(owner)) return;
        if (owners.Count == 0) previousTimeScale = HitStop.IsStopping ? 1f : Time.timeScale;
        owners.Add(owner);
        Time.timeScale = 0f;
    }

    public static void Release(object owner)
    {
        if (!owners.Remove(owner) || owners.Count > 0) return;
        Time.timeScale = previousTimeScale;
    }
}
