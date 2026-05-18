using System;

public static class EventTriggers
{
    public static event Action<int> OnLoaded;
    public static event Action OnAddCoin;

    public static void LoadInvoke(int value)
    {
        OnLoaded?.Invoke(value);
    }

    public static void AddCoinInvoke()
    {
        OnAddCoin?.Invoke();
    }
}
