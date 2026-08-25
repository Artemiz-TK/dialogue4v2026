using System;
using Extensions;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventTriggers.AddCoinInvoke();
            gameObject.SetActive(false);
        }
    }
}
