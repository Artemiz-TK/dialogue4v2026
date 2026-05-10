using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Core
{
    public class DoorController : MonoBehaviour
    {
        public static event Action<bool> OnOpenedOrLocked;
        private bool _isOpen;

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
            if (!_isOpen)
            {
                transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                _isOpen = true;
                OnOpenedOrLocked?.Invoke(_isOpen);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                _isOpen = false;
                OnOpenedOrLocked?.Invoke(_isOpen);
            }
        }
    }
}
