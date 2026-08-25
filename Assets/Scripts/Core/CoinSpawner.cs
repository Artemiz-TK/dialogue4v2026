namespace Core
{
    using UnityEngine;
    using Extensions;

    public class CoinSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private GameObject m_CoinPrefab;
        [SerializeField] private int m_AmountToSpawn = 10;

        [Header("Area Bounds (Relative to Spawner)")]
        [SerializeField] private Vector3 m_FromPosition = new Vector3(-5, 0, -5);
        [SerializeField] private Vector3 m_ToPosition = new Vector3(5, 2, 5);

        void Start()
        {
            SpawnCoins();
        }

        public void SpawnCoins()
        {
            if (m_CoinPrefab == null) return;

            for (int i = 0; i < m_AmountToSpawn; i++)
            {
                // Calcula a posição aleatória usando seu método de extensão
                Vector3 randomPosition = new Vector3().Range(m_FromPosition, m_ToPosition);
            
                // Aplica a posição transformando os pontos locais para pontos no mundo global
                Vector3 spawnPosition = transform.TransformPoint(randomPosition);

                // Instancia a moeda na posição correta
                Instantiate(m_CoinPrefab, spawnPosition, Quaternion.identity, transform);
            }
        }

        private void OnDrawGizmos()
        {
            // Desenha o cubo no editor considerando a posição do Spawner
            Gizmos.color = Color.yellow;
        
            Vector3 center = (m_FromPosition + m_ToPosition) / 2f;
            Vector3 size = new Vector3(
                Mathf.Abs(m_ToPosition.x - m_FromPosition.x),
                Mathf.Abs(m_ToPosition.y - m_FromPosition.y),
                Mathf.Abs(m_ToPosition.z - m_FromPosition.z)
            );

            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(center, size);
        }
    }

}