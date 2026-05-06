using UnityEngine;
using System;

public class GameEventSystem : MonoBehaviour
{
    //public static GameEventSystem Instance;
    public static event Action OnPlayerCollidedWithDoor;

    public static void Invoke()
    {
        OnPlayerCollidedWithDoor?.Invoke();
    }
}
