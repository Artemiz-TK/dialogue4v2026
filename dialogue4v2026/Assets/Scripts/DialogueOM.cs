using System;
using UnityEngine;

public static class DialogueOM
{
    public static event Action<string> OnNameSubmit;
    public static void SubmitName(string name)
    {
        OnNameSubmit?.Invoke(name);
    }
    
    public static event Action<Sprite> OnImageSubmit;
    public static void SubmitImage(Sprite sprite)
    {
        OnImageSubmit?.Invoke(sprite);
    }

    public static event Action<string> OnDialogueSubmit;
    public static void SubmitDialogue(string dialogue)
    {
        OnDialogueSubmit?.Invoke(dialogue);
    }
    
    public static event Action OnStartDialogue;
    public static void StartDialogue()
    {
        OnStartDialogue?.Invoke();
    }

    public static event Action OnDialogueFinished;
    public static void DialogueFinished()
    {
        OnDialogueFinished?.Invoke();
    }
}
