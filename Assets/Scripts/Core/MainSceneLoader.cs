using System;
using UnityEngine;

namespace Core
{
    public class MainSceneLoader : MonoBehaviour
    {
        public string NextScene;

        private void OnEnable()
        {
            EventTriggers.OnSecondFaseSaved += SaveScene;
        }

        private void OnDisable()
        {
            EventTriggers.OnSecondFaseSaved -= SaveScene;
        }

        // ============================================================
        // MENU PRINCIPAL
        // ============================================================

        /// <summary>
        /// Novo Jogo.
        /// </summary>
        /// <remarks>
        /// O StartGame() já inicia a Fase 1. O código:
        /// <code>
        /// SaveSystem.Singleton.NewGame();
        /// </code>
        /// Remove o autosave antigo da mémoria e deleta o antigo arquivo.
        /// </remarks>
        public void NewGame()
        {
            // ============================================================
            // 1. Remove o autosave anterior
            // ============================================================

            if (SaveSystem.Singleton != null)
            {
                SaveSystem.Singleton.NewGame();
            }

            // ============================================================
            // 2. Zera a quantidade atual de moedas
            // ============================================================

            if (QuantityManager.Singleton != null)
            {
                QuantityManager.Singleton.ResetQuantity();
            }

            // ============================================================
            // 3. Começa novamente pela Fase 1
            // ============================================================

            GameManager.Singleton.StartGame();
        }

        /// <summary>
        /// Continuar Jogo.
        /// Carrega o autosave (slot 0).
        /// </summary>
        public void ContinueGame()
        {
            if (SaveSystem.Singleton == null)
            {
                Debug.LogError(
                    "[MainSceneLoader] SaveSystem não encontrado."
                );

                return;
            }

            if (!SaveSystem.Singleton.HasSave(0))
            {
                Debug.LogWarning(
                    "[MainSceneLoader] Não existe autosave no slot 0."
                );

                return;
            }

            if (!SaveSystem.Singleton.LoadFromFile(0))
            {
                Debug.LogWarning(
                    "[MainSceneLoader] Falha ao carregar o autosave."
                );

                return;
            }

            string sceneName =
                SaveSystem.Singleton.LoadSceneName(0);

            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning(
                    "[MainSceneLoader] O autosave não possui uma cena válida."
                );
                
                Debug.Log(sceneName);

                return;
            }

            LoadSavedScene(sceneName);
        }

        /// <summary>
        /// Abre o carregamento de um slot específico.
        /// Será usado pelos botões de Slot 1, 2 e 3.
        /// </summary>
        public void LoadSlot(int slot)
        {
            if (SaveSystem.Singleton == null)
            {
                Debug.LogError(
                    "[MainSceneLoader] SaveSystem não encontrado."
                );

                return;
            }

            if (!SaveSystem.Singleton.HasSave(slot))
            {
                Debug.LogWarning(
                    $"[MainSceneLoader] O slot {slot} está vazio."
                );

                return;
            }

            if (!SaveSystem.Singleton.LoadFromFile(slot))
            {
                Debug.LogWarning(
                    $"[MainSceneLoader] Não foi possível carregar o slot {slot}."
                );

                return;
            }

            string sceneName =
                SaveSystem.Singleton.LoadSceneName(slot);

            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning(
                    $"[MainSceneLoader] O slot {slot} não possui uma cena válida."
                );

                return;
            }

            LoadSavedScene(sceneName);
        }
        
        /// <summary>
        /// Salva o estado atual do jogo no slot manual escolhido.
        /// O SaveSystem também replica esse save para o slot 0.
        /// </summary>
        public void SaveSlot(int slot)
        {
            if (SaveSystem.Singleton == null)
            {
                Debug.LogError(
                    "[MainSceneLoader] SaveSystem não encontrado."
                );

                return;
            }

            if (slot is < 1 or > 3)
            {
                Debug.LogWarning(
                    $"[MainSceneLoader] Slot inválido: {slot}"
                );

                return;
            }

            // ------------------------------------------------------------
            // Pega a posição atual do jogador
            // ------------------------------------------------------------

            GameObject player =
                GameObject.FindGameObjectWithTag("Player");

            if (player == null)
            {
                Debug.LogWarning(
                    "[MainSceneLoader] Player não encontrado."
                );

                return;
            }

            // ------------------------------------------------------------
            // Pega a quantidade atual de moedas
            // ------------------------------------------------------------

            int coins = 0;

            if (QuantityManager.Singleton != null)
            {
                coins =
                    QuantityManager.Singleton.Quantity;
            }

            // ------------------------------------------------------------
            // Salva posição + moedas
            // ------------------------------------------------------------

            SaveSystem.Singleton.SavePosition(
                player.transform.position,
                slot
            );

            SaveSystem.Singleton.SavePlayerCoins(
                coins,
                slot
            );

            // ------------------------------------------------------------
            // Salva o nome da cena atual
            // ------------------------------------------------------------

            string sceneName =
                UnityEngine.SceneManagement.SceneManager
                    .GetActiveScene()
                    .name;

            SaveSystem.Singleton.SaveSceneName(
                sceneName,
                slot
            );

            // ------------------------------------------------------------
            // Finalmente grava o arquivo.
            //
            // SaveFile() também replica o slot manual para o slot 0.
            // ------------------------------------------------------------

            SaveSystem.Singleton.SaveFile(slot);

            Debug.Log(
                $"[MainSceneLoader] Jogo salvo no Slot {slot}. " +
                $"Cena: {sceneName} | " +
                $"Moedas: {coins} | " +
                $"Posição: {player.transform.position}"
            );
        }

        /// <summary>
        /// Carrega a cena armazenada no save.
        /// </summary>
        private void LoadSavedScene(string sceneName)
        {
            if (sceneName != "Fase1" &&
                sceneName != "Fase2")
            {
                Debug.LogWarning(
                    $"[MainSceneLoader] Cena de save desconhecida: {sceneName}"
                );

                return;
            }
            
            GameManager.Singleton.LoadSavedScene(
                sceneName
            );
        }

        // ============================================================
        // MÉTODOS QUE JÁ EXISTIAM
        // ============================================================

        public void LoadNextScene()
        {
            GameManager.Singleton.StartGame();

            Debug.Log(
                $"MainSceneLoader: Loaded scene '{NextScene}'"
            );
        }

        public void Quit()
        {
            GameManager.Singleton.Quit();
        }

        // ============================================================
        // EVENTO
        // ============================================================

        private void SaveScene(string sceneName)
        {
            NextScene = sceneName;
        }
    }
}
