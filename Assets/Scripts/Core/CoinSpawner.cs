namespace Core
{
    using System;
    using UnityEngine;

    public class CoinSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private GameObject m_CoinPrefab;
        [SerializeField] private int m_AmountToSpawn = 10;

        [Header("Save Settings")]
        [Tooltip("ID único deste Spawner dentro da fase.")]
        [SerializeField] private string m_SpawnerId = "MainCoins";

        [SerializeField] private int m_SaveSlot = 0;

        [Header("Random Settings")]
        [Tooltip("Mantém as posições das moedas sempre iguais.")]
        [SerializeField] private int m_Seed = 12345;

        [Header("Area Bounds (Relative to Spawner)")]
        [SerializeField] private Vector3 m_FromPosition =
            new Vector3(-5, 0, -5);

        [SerializeField] private Vector3 m_ToPosition =
            new Vector3(5, 2, 5);

        private void Start()
        {
            SpawnCoins();
        }

        private void SpawnCoins()
        {
            if (m_CoinPrefab == null)
            {
                Debug.LogError(
                    "[CoinSpawner] Coin Prefab não foi configurado.",
                    this
                );

                return;
            }

            if (string.IsNullOrWhiteSpace(m_SpawnerId))
            {
                Debug.LogError(
                    "[CoinSpawner] O Spawner precisa possuir um ID único.",
                    this
                );

                return;
            }

            if (m_AmountToSpawn <= 0)
                return;

            SaveSystem saveSystem =
                SaveSystem.Singleton;

            System.Random random =
                new System.Random(m_Seed);

            for (int i = 0; i < m_AmountToSpawn; i++)
            {
                // ============================================================
                // GERA SEMPRE A MESMA POSIÇÃO PARA O MESMO ÍNDICE
                // ============================================================

                Vector3 localPosition = new Vector3(
                    Mathf.Lerp(
                        m_FromPosition.x,
                        m_ToPosition.x,
                        (float)random.NextDouble()
                    ),

                    Mathf.Lerp(
                        m_FromPosition.y,
                        m_ToPosition.y,
                        (float)random.NextDouble()
                    ),

                    Mathf.Lerp(
                        m_FromPosition.z,
                        m_ToPosition.z,
                        (float)random.NextDouble()
                    )
                );

                Vector3 spawnPosition =
                    transform.TransformPoint(localPosition);

                // ============================================================
                // ID ÚNICO E ESTÁVEL DA MOEDA
                // ============================================================

                string coinId =
                    $"{UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}" +
                    $"_{m_SpawnerId}_Coin_{i}";

                // ============================================================
                // SE A MOEDA JÁ FOI COLETADA, NÃO INSTANCIA
                // ============================================================

                if (saveSystem != null &&
                    saveSystem.IsCoinCollectedAtCheckpoint(
                        coinId,
                        m_SaveSlot))
                {
                    Debug.Log(
                        $"[CoinSpawner] Moeda já persistida no checkpoint: {coinId}"
                    );

                    continue;
                }

                // ============================================================
                // INSTANCIA A MOEDA
                // ============================================================

                GameObject coin =
                    Instantiate(
                        m_CoinPrefab,
                        spawnPosition,
                        Quaternion.identity,
                        transform
                    );

                // ============================================================
                // CONFIGURA O ID DA MOEDA
                // ============================================================

                PersistentCoin persistentCoin =
                    coin.GetComponent<PersistentCoin>();

                if (persistentCoin == null)
                {
                    Debug.LogError(
                        "[CoinSpawner] O CoinPrefab precisa possuir " +
                        "o componente PersistentCoin.",
                        coin
                    );

                    continue;
                }

                persistentCoin.Initialize(
                    coinId,
                    m_SaveSlot
                );
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;

            Vector3 center =
                (m_FromPosition + m_ToPosition) / 2f;

            Vector3 size = new Vector3(
                Mathf.Abs(m_ToPosition.x - m_FromPosition.x),
                Mathf.Abs(m_ToPosition.y - m_FromPosition.y),
                Mathf.Abs(m_ToPosition.z - m_FromPosition.z)
            );

            Gizmos.matrix =
                transform.localToWorldMatrix;

            Gizmos.DrawWireCube(
                center,
                size
            );
        }
    }
}
