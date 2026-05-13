using UnityEngine;
using System;

public static class Interact
{
    public static event Action OnInteract;
    
    public static void InteractInvoke()
    {
        OnInteract?.Invoke();
    }
}
