using System;
using Core;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    private static GUIManager s_Instance;
    public static GUIManager Singleton => s_Instance;
    
    
    public GameObject Canvas;
    public TextMeshProUGUI txtQuantity;

    private void Awake()
    {
        s_Instance = this;
    }
    
    void OnEnable()
    {
        EventTriggers.OnLoaded += Load;
    }

    void OnDisable()
    {
        EventTriggers.OnLoaded -= Load;
    }

    private void Start()
    {
        if (SaveSystem.Singleton == null || !SaveSystem.Singleton!.LoadPlayerCoins(out var coins)) return;
        txtQuantity.text = coins.ToString();
        Debug.Log(coins);
    }

    private void Load(int value)
    {
        txtQuantity.text = $"{value}";
    }
}
