using UnityEngine;
using System;

namespace Core
{
    public class GameEventSystem : MonoBehaviour
    {
        public static event Action OnPlayerCollidedWithDoor;

        public static void Invoke()
        {
            OnPlayerCollidedWithDoor?.Invoke();
        }
    }    
}
