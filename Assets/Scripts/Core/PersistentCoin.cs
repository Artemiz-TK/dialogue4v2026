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

            if (saveSystem != null)
            {
                // --------------------------------------------------------
                // REGISTRA A MOEDA NA EXECUÇÃO ATUAL.
                //
                // Isso NÃO significa que ela já foi salva no disco.
                // Ela só se torna persistente quando um checkpoint
                // copiar esse estado para CheckpointCollectedCoins.
                // --------------------------------------------------------

                saveSystem.RegisterCollectedCoin(
                    m_CoinId,
                    m_SaveSlot
                );

                Debug.Log(
                    $"[PersistentCoin] Moeda coletada na execução atual: " +
                    $"{m_CoinId}"
                );
            }

            // Desaparece normalmente nesta execução.
            gameObject.SetActive(false);
        }
    }
}
