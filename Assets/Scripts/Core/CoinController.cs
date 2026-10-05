using UnityEngine;
using Core;

public class CoinController : MonoBehaviour
{
    private PersistentCoin m_PersistentCoin;

    private void Awake()
    {
        m_PersistentCoin =
            GetComponent<PersistentCoin>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        EventTriggers.AddCoinTrigger();

        if (m_PersistentCoin != null)
        {
            m_PersistentCoin.Collect();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
