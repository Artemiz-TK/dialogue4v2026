using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUIController : MonoBehaviour
{
    private CanvasGroup canvasGroup;
   
    [SerializeField] private TMP_Text nameText;
   
    [SerializeField] private Image portraitImage;
   
    [SerializeField] private TMP_Text dialogueText;
   
    [SerializeField] private float typingSpeed;
   


    private string _textToDisplay;

    private string _currentText;

    private bool m_CanEndDialogue;

    private Coroutine dialogueRoutine;


    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        DialogueOM.OnNameSubmit += SetName;
        DialogueOM.OnImageSubmit += SetPortrait;
        DialogueOM.OnDialogueSubmit += SetDialogueText;
        DialogueOM.OnStartDialogue += DisplayDialogue;
        DialogueOM.OnDialogueFinished += EndDialogue;
    }

    private void OnDisable()
    {
        DialogueOM.OnNameSubmit -= SetName;
        DialogueOM.OnImageSubmit -= SetPortrait;
        DialogueOM.OnDialogueSubmit -= SetDialogueText;
        DialogueOM.OnStartDialogue -= DisplayDialogue;
        DialogueOM.OnDialogueFinished -= EndDialogue;
    }

    private void SetName(string npcName)
    {
        nameText.text = npcName;
    }

    private void SetPortrait(Sprite npcImage)
    {
        portraitImage.sprite = npcImage;
    }

    private void SetDialogueText(string text)
    {
        _textToDisplay = text;
        _currentText = "";
    }

    private IEnumerator ShowDialogue()
    {
        while (StringComparer.Ordinal.Compare(_currentText, _textToDisplay) != 0)
        {
            _currentText = _textToDisplay.Substring(0, _currentText.Length + 1);
            dialogueText.text = _currentText;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(0.6f);

        m_CanEndDialogue = true;
    }

    private void DisplayDialogue()
    {
        if (dialogueRoutine != null)
            StopCoroutine(dialogueRoutine);

        canvasGroup.alpha = 1;
        dialogueRoutine = StartCoroutine(ShowDialogue());
    }

    private void EndDialogue()
    {
        if (!m_CanEndDialogue) return;
        canvasGroup.alpha = 0;
        m_CanEndDialogue = false;

        PlayerController.InteractionPerformed();
    }
}
