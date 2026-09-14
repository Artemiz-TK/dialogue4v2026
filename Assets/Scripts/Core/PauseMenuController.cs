using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    /// <summary>
    /// Controla as operações realizadas pelo Menu de Pausa.
    ///
    /// Não controla visual dos Panels nem eventos dos Buttons.
    /// </summary>
    public class PauseMenuController : MonoBehaviour
    {
        public bool IsPaused { get; private set; }

        // ============================================================
        // PAUSA
        // ============================================================

        public void PauseGame()
        {
            if (IsPaused)
                return;

            IsPaused = true;
            Time.timeScale = 0f;

            Debug.Log(
                "[PauseMenuController] Jogo pausado."
            );
        }

        public void ResumeGame()
        {
            if (!IsPaused)
                return;

            IsPaused = false;
            Time.timeScale = 1f;

            Debug.Log(
                "[PauseMenuController] Jogo retomado."
            );
        }

        // ============================================================
        // SAVE
        // ============================================================

        public void SaveSlot(int slot)
        {
            if (slot < 1 || slot > 3)
            {
                Debug.LogWarning(
                    $"[PauseMenuController] Slot inválido: {slot}"
                );

                return;
            }

            if (SaveSystem.Singleton == null)
            {
                Debug.LogError(
                    "[PauseMenuController] SaveSystem não encontrado."
                );

                return;
            }

            GameObject player =
                GameObject.FindGameObjectWithTag("Player");

            if (player == null)
            {
                Debug.LogWarning(
                    "[PauseMenuController] Player não encontrado."
                );

                return;
            }

            // --------------------------------------------------------
            // Posição
            // --------------------------------------------------------

            SaveSystem.Singleton.SavePosition(
                player.transform.position,
                slot
            );

            // --------------------------------------------------------
            // Moedas
            // --------------------------------------------------------

            int coins = 0;

            if (QuantityManager.Singleton != null)
            {
                coins =
                    QuantityManager.Singleton.Quantity;
            }

            SaveSystem.Singleton.SavePlayerCoins(
                coins,
                slot
            );

            // --------------------------------------------------------
            // Cena
            // --------------------------------------------------------

            string sceneName =
                SceneManager.GetActiveScene().name;

            SaveSystem.Singleton.SaveSceneName(
                sceneName,
                slot
            );

            // --------------------------------------------------------
            // Grava arquivo
            // --------------------------------------------------------

            SaveSystem.Singleton.SaveFile(slot);

            Debug.Log(
                $"[PauseMenuController] " +
                $"Jogo salvo no Slot {slot}."
            );
        }

        // ============================================================
        // LOAD
        // ============================================================

        public void LoadSlot(int slot)
        {
            if (slot < 1 || slot > 3)
            {
                Debug.LogWarning(
                    $"[PauseMenuController] Slot inválido: {slot}"
                );

                return;
            }

            if (SaveSystem.Singleton == null)
            {
                Debug.LogError(
                    "[PauseMenuController] SaveSystem não encontrado."
                );

                return;
            }

            if (!SaveSystem.Singleton.HasSave(slot))
            {
                Debug.LogWarning(
                    $"[PauseMenuController] " +
                    $"O Slot {slot} está vazio."
                );

                return;
            }

            if (!SaveSystem.Singleton.LoadFromFile(slot))
            {
                Debug.LogWarning(
                    $"[PauseMenuController] " +
                    $"Não foi possível carregar o Slot {slot}."
                );

                return;
            }

            string sceneName =
                SaveSystem.Singleton.LoadSceneName(slot);

            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning(
                    $"[PauseMenuController] " +
                    $"O Slot {slot} não possui uma cena válida."
                );

                return;
            }

            // Retira a pausa antes de iniciar o carregamento.
            Time.timeScale = 1f;
            IsPaused = false;

            GameManager.Singleton.LoadSavedScene(
                sceneName
            );
        }

        // ============================================================
        // MENU PRINCIPAL
        // ============================================================

        public void ReturnToMainMenu()
        {
            Time.timeScale = 1f;
            IsPaused = false;

            SceneManager.LoadScene(
                "MenuPrincipal"
            );

            Debug.Log(
                "[PauseMenuController] " +
                "Retornando ao Menu Principal."
            );
        }
    }
}
