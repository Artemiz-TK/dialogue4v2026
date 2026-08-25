using System;

namespace Core
{
    using UnityEngine;
    
    public class SaveTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                
            }
        }
    }
}