namespace Core
{
    using UnityEngine;

    public class PersistentCoin : MonoBehaviour
    {
        private string m_CoinId;
        private int m_SaveSlot;

        private bool m_Collected;

        public string CoinId =>
            m_CoinId;

        public void Initialize(
            string coinId,
            int saveSlot)
        {
            m_CoinId = coinId;
            m_SaveSlot = saveSlot;
            m_Collected = false;
        }

        public void Collect()
        {
            if (m_Collected)
                return;

            m_Collected = true;

            SaveSystem saveSystem =
                SaveSystem.Singleton;

            // ============================================================
            // VERIFICA SE EXISTE CHECKPOINT VÁLIDO NESTE MOMENTO
            // ============================================================

            bool hasValidCheckpoint =
                saveSystem != null &&
                saveSystem.HasValidCheckpointForCurrentScene(
                    m_SaveSlot
                );

            // ============================================================
            // SÓ TORNA A MOEDA PERSISTENTE SE HAVIA CHECKPOINT VÁLIDO
            // ============================================================

            if (hasValidCheckpoint)
            {
                saveSystem.RegisterCollectedCoin(
                    m_CoinId,
                    m_SaveSlot
                );

                Debug.Log(
                    $"[PersistentCoin] Moeda persistida: {m_CoinId}"
                );
            }
            else
            {
                Debug.Log(
                    $"[PersistentCoin] Moeda coletada sem checkpoint válido: " +
                    $"{m_CoinId}. Ela reaparecerá ao recarregar a fase."
                );
            }

            // A moeda desaparece normalmente durante a execução.
            gameObject.SetActive(false);
        }
    }
}
