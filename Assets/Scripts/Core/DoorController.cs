using System;
using Unity.VisualScripting;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private bool isOpen = false;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("DoorController: Player collided with door.");
            GameEventSystem.Invoke();
        }
    }

    public void OpenDoor()
    {
        if (isOpen)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
            isOpen = false;
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            isOpen = true;
        }
    }
}
