using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinController : MonoBehaviour
{
    public int QuantityOfCoins { get; private set; }

    private void OnEnable()
    {
        Interact.OnInteracted += HandleInteract;
    }

    private void OnDisable()
    {
        Interact.OnInteracted -= HandleInteract;
    }

    void Start()
    {
        transform.position = new Vector3(Random.Range(-10f, 10f), Random.Range(1.03f, 1.7f), Random.Range(-10f, 10f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Interact.InteractInvoke(); // incrementa o valor
            Interact.LoadInvoke(QuantityOfCoins);    //  atualiza no texto
            gameObject.SetActive(false);
        }
    }

    private void HandleInteract()
    {
        QuantityOfCoins += 1;
    }
}
