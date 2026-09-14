using UnityEngine;

public class CoinController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        EventTriggers.AddCoinTrigger();
        gameObject.SetActive(false);
    }
}
