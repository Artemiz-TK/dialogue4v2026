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
            if (slot is < 1 or > 3)
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

            if (GameManager.Singleton == null)
            {
                Debug.LogError(
                    "[PauseMenuController] GameManager não encontrado."
                );

                return;
            }

            string sceneName =
                SceneManager.GetActiveScene().name;

            bool saved =
                SaveSystem.Singleton.SaveManualSlot(
                    slot,
                    GameManager.Singleton.CurrentPhaseStartPosition,
                    sceneName
                );

            if (!saved)
            {
                Debug.LogWarning(
                    $"[PauseMenuController] " +
                    $"Não foi possível salvar o Slot {slot}."
                );

                return;
            }

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
