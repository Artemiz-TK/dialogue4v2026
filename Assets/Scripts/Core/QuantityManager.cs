using System;
using UnityEngine;

public class QuantityManager : MonoBehaviour
{
    private int m_TotalQuantity = 0;

    private void OnEnable()
    {
        EventTriggers.OnAddCoin += AddCoin;
    }

    private void OnDisable()
    {
        EventTriggers.OnAddCoin -= AddCoin;
    }

    public void AddCoin()
    {
        m_TotalQuantity++;
        EventTriggers.LoadInvoke(m_TotalQuantity);
    }

    public int Quantity => m_TotalQuantity;
}

