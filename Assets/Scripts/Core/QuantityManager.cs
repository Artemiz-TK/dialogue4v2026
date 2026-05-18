using UnityEngine;

public class QuantityManager : MonoBehaviour
{
    public static QuantityManager Instance { get; private set; }
    private int m_TotalQuantity = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddCoin()
    {
        m_TotalQuantity++;
        EventTriggers.LoadInvoke(m_TotalQuantity);
    }

    public int Quantity => m_TotalQuantity;
}

