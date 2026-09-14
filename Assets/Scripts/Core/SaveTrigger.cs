using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    [RequireComponent(typeof(Collider))]
    public class SaveTrigger : MonoBehaviour
    {
        [SerializeField]
        private int m_Slot = 0;

        private Collider m_Collider;
        private QuantityManager m_Quantity;

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

            // ============================================================
            // DADOS ATUAIS
            // ============================================================

            Vector3 playerPosition =
                other.transform.position;

            int coins = 0;

            if (m_Quantity != null)
            {
                coins = m_Quantity.Quantity;
            }

            string sceneName =
                SceneManager.GetActiveScene().name;

            // ============================================================
            // SALVA O CHECKPOINT
            // ============================================================

            bool checkpointSaved =
                SaveSystem.Singleton.SaveCheckpoint(
                    playerPosition,
                    coins,
                    m_Slot
                );

            if (!checkpointSaved)
            {
                Debug.LogError(
                    "[SaveTrigger] Não foi possível salvar o checkpoint."
                );

                return;
            }

            // ============================================================
            // SALVA A CENA EXPLICITAMENTE
            // ============================================================

            SaveSystem.Singleton.SaveSceneName(
                sceneName,
                m_Slot
            );

            // ============================================================
            // GRAVA NOVAMENTE O ARQUIVO
            //
            // SaveCheckpoint já grava o arquivo, mas como acabamos
            // de definir SceneName depois dele, precisamos gravar
            // novamente para garantir que a cena esteja no arquivo.
            // ============================================================

            SaveSystem.Singleton.SaveFile(
                m_Slot
            );

            Debug.Log(
                $"[SaveTrigger] CHECKPOINT SALVO COM SUCESSO\n" +
                $"Slot: {m_Slot}\n" +
                $"Cena: {sceneName}\n" +
                $"Posição: {playerPosition}\n" +
                $"Moedas: {coins}"
            );
        }
    }
}
