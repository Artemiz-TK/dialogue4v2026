using UnityEngine;

public class UIGameplayManager : MonoBehaviour
{
    public static  UIGameplayManager Instance;
    public GameObject buttonGameObject;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        Instance = this;
        buttonGameObject.SetActive(false);
    }

    public void ShowButton()
    {
        buttonGameObject.SetActive(true);
    }
}
