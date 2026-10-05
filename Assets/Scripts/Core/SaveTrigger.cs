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

        private void Start()
        {
            if (TryGetComponent<Collider>(out var col))
            {
                m_Collider = col;
                m_Collider.isTrigger = true;
            }

            m_Quantity =
                QuantityManager.Singleton;

            if (m_Quantity != null)
            {
                Debug.Log(
                    $"[SaveTrigger] QuantityManager encontrado. " +
                    $"Checkpoint: {m_CheckpointId}"
                );
            }

            // ============================================================
            // VERIFICA SE ESTE CHECKPOINT JÁ FOI ALCANÇADO
            // ============================================================

            if (SaveSystem.Singleton != null &&
                m_Collider != null)
            {
                string sceneName =
                    SceneManager.GetActiveScene().name;

                if (SaveSystem.Singleton.HasReachedCheckpoint(
                        m_CheckpointId,
                        sceneName,
                        m_Slot))
                {
                    m_Collider.enabled = false;

                    Debug.Log(
                        $"[SaveTrigger] Checkpoint {m_CheckpointId} " +
                        "já foi alcançado. Trigger desativado."
                    );
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            if (SaveSystem.Singleton == null)
            {
                Debug.LogError(
                    "[SaveTrigger] SaveSystem não encontrado."
                );

                return;
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
                Debug.LogWarning(
                    $"[SaveTrigger] Checkpoint {m_CheckpointId} " +
                    "não foi salvo."
                );

                return;
            }

            // ============================================================
            // O CHECKPOINT FOI PROCESSADO.
            //
            // Desabilita o collider para impedir novas ativações
            // durante esta execução da fase.
            // ============================================================

            if (m_Collider != null)
            {
                m_Collider.enabled = false;
            }

            Debug.Log(
                $"[SaveTrigger] Checkpoint {m_CheckpointId} ativado.\n" +
                $"Posição: {checkpointPosition}\n" +
                $"Moedas: {coins}\n" +
                $"Trigger desativado."
            );
        }
    }
}
