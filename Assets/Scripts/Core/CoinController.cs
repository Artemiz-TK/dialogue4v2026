using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinController : MonoBehaviour
{
    public static CoinController Instance;
    
    public static Action OnLoadQuantity;
    public int QuantityOfCoins { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnEnable()
    {
        Interact.OnInteract += HandleInteract;
    }

    void Start()
    {
        transform.position = new Vector3(Random.Range(-10f, 10f), Random.Range(1.03f, 2f), Random.Range(-10f, 10f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Interact.InteractInvoke();
            GUIManager.Instance.Load();
            gameObject.SetActive(false);
        }
    }

    private void HandleInteract()
    {
        QuantityOfCoins += 1;
    }
}
