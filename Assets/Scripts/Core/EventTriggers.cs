using System;

public static class EventTriggers
{
    public static event Action<int> OnLoaded;

    public static void LoadInvoke(int value)
    {
        OnLoaded?.Invoke(value);
    }
}
