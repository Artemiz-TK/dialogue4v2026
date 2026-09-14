using System;

public static class EventTriggers
{
    public static event Action<int> OnLoaded;
    public static event Action OnAddCoin;

    public static event Action<string> OnSecondFaseSaved;

    public static void LoadTrigger(int value)
    {
        OnLoaded?.Invoke(value);
    }

    public static void AddCoinTrigger()
    {
        OnAddCoin?.Invoke();
    }

    public static void SecondFaseSavedTrigger(string arg)
    {
        OnSecondFaseSaved?.Invoke(arg);
    }
}
