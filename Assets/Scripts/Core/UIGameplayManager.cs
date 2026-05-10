using TMPro;
using UnityEngine;

namespace Core
{
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

        void OnEnable()
        {
            DoorController.OnOpenedOrLocked += HandleOpenOrLockDoor;
        }

        void OnDisable()
        {
            DoorController.OnOpenedOrLocked -= HandleOpenOrLockDoor;
        }

        public void ShowButton()
        {
            buttonGameObject.SetActive(true);
        }

        void HandleOpenOrLockDoor(bool ctx)
        {
            if (buttonGameObject.GetComponentInChildren<TextMeshProUGUI>().TryGetComponent<TextMeshProUGUI>(out var txtMesh))
            {
                if (ctx)
                {
                    txtMesh.text = "Close";
                    return;
                }

                txtMesh.text = "Open";
            }else
            {
                Debug.Log("Não conseguiu pegar o component");
            }
        }
    }
}
