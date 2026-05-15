using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GUIManager : MonoBehaviour
{

    public GameObject Canvas;
    public static GUIManager Instance;

    public TextMeshProUGUI txtQuantity;

    void OnEnable()
    {
        Interact.OnLoaded += Load;
    }

    void OnDisable()
    {
        Interact.OnLoaded -= Load;
    }

    private void Start()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        Instance = this;
    }

    private void Load(int value)
    {
        txtQuantity.text = $"{value}";
    }
}
