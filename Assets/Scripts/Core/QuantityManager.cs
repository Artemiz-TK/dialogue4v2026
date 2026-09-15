using UnityEngine;

namespace Core
{
    public class QuantityManager : MonoBehaviour
    {
        private static QuantityManager s_Instance;
        public static QuantityManager Singleton => s_Instance;

        private int m_TotalQuantity;

        public int Quantity => m_TotalQuantity;

        private void Awake()
        {
            if (s_Instance != null && s_Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            s_Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            EventTriggers.OnAddCoin += AddCoin;
        }

        private void OnDisable()
        {
            EventTriggers.OnAddCoin -= AddCoin;
        }

        private void Start()
        {
            if (SaveSystem.Singleton == null)
            {
                Debug.LogWarning(
                    "[QuantityManager] SaveSystem não encontrado."
                );

                return;
            }

            if (!SaveSystem.Singleton.LoadProgressCoins(
                    out int coins))
            {
                Debug.Log(
                    "[QuantityManager] Nenhuma moeda salva encontrada."
                );

                return;
            }

            m_TotalQuantity = coins;

            Debug.Log(
                $"[QuantityManager] Moedas carregadas: {m_TotalQuantity}"
            );

            EventTriggers.LoadTrigger(
                m_TotalQuantity
            );
        }

        public void AddCoin()
        {
            m_TotalQuantity++;

            EventTriggers.LoadTrigger(
                m_TotalQuantity
            );
        }

        public void SetQuantity(int quantity)
        {
            m_TotalQuantity = quantity;

            EventTriggers.LoadTrigger(
                m_TotalQuantity
            );
        }

        public int GetQuantity()
        {
            return m_TotalQuantity;
        }

        public void ResetQuantity()
        {
            SetQuantity(0);

            EventTriggers.LoadTrigger(
                m_TotalQuantity
            );

            Debug.Log(
                "[QuantityManager] Quantidade resetada para 0."
            );
        }
    }
}