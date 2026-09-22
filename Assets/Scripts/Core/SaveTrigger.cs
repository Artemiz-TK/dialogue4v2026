using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    [RequireComponent(typeof(Collider))]
    public class SaveTrigger : MonoBehaviour
    {
        [Header("Save Settings")]
        [SerializeField]
        private int m_Slot = 0;

        [Header("Checkpoint")]
        [SerializeField]
        private int m_CheckpointId;

        private Collider m_Collider;
        private QuantityManager m_Quantity;

        private bool m_Loaded = false;

        private void Start()
        {
            if (TryGetComponent<Collider>(out var col))
            {
                m_Collider = col;
                m_Collider.isTrigger = true;
            }

            m_Quantity = QuantityManager.Singleton;

            if (m_Quantity != null)
            {
                Debug.Log(
                    "[SaveTrigger] QuantityManager encontrado."
                );
            }
            else
            {
                Debug.LogWarning(
                    "[SaveTrigger] QuantityManager não encontrado."
                );
            }
            
            if (SaveSystem.Singleton.HasCheckpoint(m_Slot))
                return;
            
            _ = SaveCheckpoint(m_Slot);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            if (SaveSystem.Singleton is null)
                return;

            if (SaveSystem.Singleton.HasCheckpoint(m_Slot) && m_Loaded)
                return;

            _ = SaveCheckpoint(m_Slot);
            m_Loaded = true;
        }

        private bool SaveCheckpoint(int slot)
        {
            if (SaveSystem.Singleton == null)
            {
                Debug.LogError(
                    "[SaveTrigger] SaveSystem não encontrado."
                );

                return false;
            }

            int coins = 0;

            if (m_Quantity != null)
            {
                coins =
                    m_Quantity.Quantity;
            }

            Vector3 checkpointPosition =
                transform.position;

            bool checkpointSaved =
                SaveSystem.Singleton.SaveCheckpoint(
                    checkpointPosition,
                    coins,
                    m_CheckpointId,
                    m_Slot
                );

            if (!checkpointSaved)
            {
                Debug.LogError(
                    "[SaveTrigger] Não foi possível salvar o checkpoint."
                );

                return false;
            }

            Debug.Log(
                $"[SaveTrigger] Checkpoint processado.\n" +
                $"Posição: {checkpointPosition}\n" +
                $"Moedas: {coins}"
            );

            return true;
        }
    }
}
