using TMPro;
using UnityEngine;

public class GUIManager : MonoBehaviour
{

    public GameObject Canvas;
    public static GUIManager Instance;
    
    public TextMeshProUGUI txtQuantity;
    private int qtCoin = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(Canvas);
    }

    public void Load()
    {
        txtQuantity.text = $"Quantity: {CoinController.Instance.QuantityOfCoins}";
    }

    public void HandleInteract()
    {
        qtCoin += 1;
    }
}
