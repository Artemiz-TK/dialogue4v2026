using UnityEngine;

public class CoinController : MonoBehaviour
{
    public int QuantityOfCoins { get; private set; }

    private void OnEnable()
    {
        EventTriggers.OnInteracted += HandleInteract;
    }

    private void OnDisable()
    {
        EventTriggers.OnInteracted -= HandleInteract;
    }

    void Start()
    {
        transform.position = new Vector3(Random.Range(-10f, 10f), Random.Range(1.03f, 1.7f), Random.Range(-10f, 10f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventTriggers.InteractInvoke(); // incrementa o valor
            gameObject.SetActive(false);
        }
    }

    private void HandleInteract()
    {
        QuantityOfCoins += 1;

        EventTriggers.LoadInvoke(QuantityOfCoins); //  atualiza no texto
    }
}
