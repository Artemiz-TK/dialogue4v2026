using UnityEngine;

public class CoinController : MonoBehaviour
{
    void Start()
    {
        transform.position = new Vector3(Random.Range(-10f, 10f), Random.Range(1.03f, 1.7f), Random.Range(-10f, 10f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventTriggers.AddCoinInvoke();
            gameObject.SetActive(false);
        }
    }
}
