using System;

public static class EventTriggers
{
    public static event Action OnInteracted;
    public static event Action<int> OnLoaded;
    public static event Action<DamageAction.CallbackContext> performed;

    public static void InteractInvoke()
    {
        OnInteracted?.Invoke();
    }

    public static void LoadInvoke(int value)
    {
        OnLoaded?.Invoke(value);
    }
}
