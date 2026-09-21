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
        if (QuantityManager.Singleton == null)
        {
            Debug.LogWarning(
                "[GUIManager] QuantityManager não encontrado."
            );

            return;
        }

        txtQuantity.text =
            QuantityManager.Singleton.Quantity.ToString();
    }

    private void Load(int value)
    {
        txtQuantity.text = $"{value}";
    }
}
