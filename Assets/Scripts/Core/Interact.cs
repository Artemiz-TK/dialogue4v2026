using System;

public static class Interact
{
    public static event Action OnInteracted;
    public static event Action<int> OnLoaded;
    
    public static void InteractInvoke()
    {
        OnInteracted?.Invoke();
    }

    public static void LoadInvoke(int value)
    {
        OnLoaded?.Invoke(value);
    }
}
