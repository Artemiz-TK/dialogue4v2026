using System;
using UnityEngine;

public class PortaController : MonoBehaviour
{
    public Animator anim;
    private bool isOpen;
    private bool isInteractable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !isInteractable)
        {
            InteractOM.OnInteract += OpenClose;
            isInteractable = true;
            InteractOM.ShowInteraction(isInteractable);
            InteractOM.PositionChange(transform.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && isInteractable)
        {
            InteractOM.OnInteract -= OpenClose;
            isInteractable = false;
            InteractOM.ShowInteraction(isInteractable);
        }
    }

    private void OpenClose()
    {
        if (!isInteractable) return;

        if (!isOpen)
        {
            anim.StopPlayback();
            anim.Play("PortaAbrindo");
            isOpen = true;
        }
        else
        {
            anim.StopPlayback();
            anim.Play("PortaFechando");
            isOpen = false;
        }
    }
}
