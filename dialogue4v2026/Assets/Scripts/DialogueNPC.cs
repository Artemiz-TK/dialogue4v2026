using System;
using UnityEngine;

public class DialogueNPC : MonoBehaviour
{
    [SerializeField] private DialogueNPCSO dialogueNpcSo;
    
    public string NPCName => dialogueNpcSo.npcName;
    public Sprite NPCImage => dialogueNpcSo.npcImage;
    public Color NPCColor => dialogueNpcSo.npcColor;
    public string[] DialogueLines => dialogueNpcSo.dialogueLines.ToArray();
    
    private bool isInteractable;

    private void Start()
    {
        GetComponent<MeshRenderer>().material.color = NPCColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !isInteractable)
        {
            isInteractable = true;
            InteractOM.OnPlayerInteracted += ShowDialogue;
            InteractOM.ShowInteraction(isInteractable);
            InteractOM.PositionChange(transform.position);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && isInteractable)
        {
            isInteractable = false;
            InteractOM.OnPlayerInteracted -= ShowDialogue;
            InteractOM.ShowInteraction(isInteractable);
        }
    }

    private void ShowDialogue(ref bool IsInteracting)
    {
        IsInteracting = isInteractable;
        Debug.Log(NPCName+": "+DialogueLines[0]);
        DialogueOM.SubmitName(NPCName);
        DialogueOM.SubmitImage(NPCImage);
        DialogueOM.SubmitDialogue(DialogueLines[0]);
        DialogueOM.StartDialogue();
    }
}
