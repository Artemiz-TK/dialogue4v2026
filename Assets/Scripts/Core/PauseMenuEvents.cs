using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Core
{
    /// <summary>
    /// Responsável pelos eventos dos Buttons do Menu de Pause
    /// e pela abertura/fechamento do menu através do Input System.
    ///
    /// Não modifica:
    /// - posição
    /// - tamanho
    /// - fonte
    /// - cor
    /// - layout
    /// - navegação
    /// - hierarquia
    ///
    /// O script apenas controla eventos e estado do menu.
    /// </summary>
    public class PauseMenuEvents : MonoBehaviour
    {
        [Header("Pause Input")]
        [SerializeField]
        private InputActionReference m_PauseAction;

        [Header("Pause Menu")]
        [SerializeField]
        private GameObject m_PausePanel;

        [Header("Pause Buttons")]
        [SerializeField]
        private Button m_SaveGameButton;

        [SerializeField]
        private Button m_LoadGameButton;

        [SerializeField]
        private Button m_BackToMenuButton;

        [SerializeField]
        private Button m_ResumeButton;

        [Header("Save Slots")]
        [SerializeField]
        private GameObject m_SaveSlotsPanel;

        [SerializeField]
        private GameObject m_LoadSlotsPanel;

        [SerializeField]
        private Button m_SaveSlot1Button;

        [SerializeField]
        private Button m_SaveSlot2Button;

        [SerializeField]
        private Button m_SaveSlot3Button;

        [SerializeField]
        private Button m_LoadSlot1Button;

        [SerializeField]
        private Button m_LoadSlot2Button;

        [SerializeField]
        private Button m_LoadSlot3Button;

        [SerializeField]
        private Button m_SaveSlotsBackButton;

        [SerializeField]
        private Button m_LoadSlotsBackButton;

        private void Awake()
        {
            RegisterEvents();

            CloseAllPanels();
        }

        private void OnEnable()
        {
            if (m_PauseAction == null)
                return;

            m_PauseAction.action.performed += OnPausePerformed;
            m_PauseAction.action.Enable();
        }

        private void OnDisable()
        {
            if (m_PauseAction == null)
                return;

            m_PauseAction.action.performed -= OnPausePerformed;
            m_PauseAction.action.Disable();
        }

        // ============================================================
        // INPUT SYSTEM
        // ============================================================

        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            TogglePause();
        }

        // ============================================================
        // PAUSE
        // ============================================================

        private void TogglePause()
        {
            if (m_PausePanel == null)
                return;

            if (m_PausePanel.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                OpenPauseMenu();
            }
        }

        private void OpenPauseMenu()
        {
            CloseAllPanels();

            if (m_PausePanel != null)
                m_PausePanel.SetActive(true);

            Time.timeScale = 0f;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void ResumeGame()
        {
            CloseAllPanels();

            Time.timeScale = 1f;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        // ============================================================
        // MENU DE PAUSE
        // ============================================================

        private void OpenSaveMenu()
        {
            CloseAllPanels();

            if (m_SaveSlotsPanel != null)
                m_SaveSlotsPanel.SetActive(true);
        }

        private void OpenLoadMenu()
        {
            CloseAllPanels();

            if (m_LoadSlotsPanel != null)
                m_LoadSlotsPanel.SetActive(true);
        }

        private void BackToPauseMenu()
        {
            CloseAllPanels();

            if (m_PausePanel != null)
                m_PausePanel.SetActive(true);
        }

        // ============================================================
        // SAVE
        // ============================================================

        private void SaveSlot(int slot)
        {
            if (SaveSystem.Singleton == null)
            {
                Debug.LogError(
                    "[PauseMenuEvents] SaveSystem não encontrado."
                );

                return;
            }

            if (QuantityManager.Singleton == null)
            {
                Debug.LogError(
                    "[PauseMenuEvents] QuantityManager não encontrado."
                );

                return;
            }

            int coins =
                QuantityManager.Singleton.Quantity;

            Vector3 playerPosition =
                GetPlayerPosition();

            string sceneName =
                UnityEngine.SceneManagement
                    .SceneManager
                    .GetActiveScene()
                    .name;

            SaveSystem.Singleton.SavePosition(
                playerPosition,
                slot
            );

            SaveSystem.Singleton.SavePlayerCoins(
                coins,
                slot
            );

            SaveSystem.Singleton.SaveSceneName(
                sceneName,
                slot
            );

            SaveSystem.Singleton.SaveFile(slot);

            Debug.Log(
                $"[PauseMenuEvents] Jogo salvo no slot {slot}. " +
                $"Moedas: {coins} | " +
                $"Cena: {sceneName}"
            );

            BackToPauseMenu();
        }

        // ============================================================
        // LOAD
        // ============================================================

        private void LoadSlot(int slot)
        {
            if (SaveSystem.Singleton == null)
            {
                Debug.LogError(
                    "[PauseMenuEvents] SaveSystem não encontrado."
                );

                return;
            }

            if (!SaveSystem.Singleton.HasSave(slot))
            {
                Debug.LogWarning(
                    $"[PauseMenuEvents] O slot {slot} está vazio."
                );

                return;
            }

            if (!SaveSystem.Singleton.LoadFromFile(slot))
            {
                Debug.LogWarning(
                    $"[PauseMenuEvents] Falha ao carregar o slot {slot}."
                );

                return;
            }

            string sceneName =
                SaveSystem.Singleton.LoadSceneName(slot);

            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning(
                    $"[PauseMenuEvents] O slot {slot} não possui uma cena válida."
                );

                return;
            }

            Time.timeScale = 1f;

            if (GameManager.Singleton == null)
            {
                Debug.LogError(
                    "[PauseMenuEvents] GameManager não encontrado."
                );

                return;
            }

            GameManager.Singleton.LoadSavedScene(
                sceneName
            );
        }

        // ============================================================
        // VOLTAR AO MENU
        // ============================================================

        private void BackToMainMenu()
        {
            Time.timeScale = 1f;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (GameManager.Singleton == null)
            {
                Debug.LogError(
                    "[PauseMenuEvents] GameManager não encontrado."
                );

                return;
            }

            GameManager.Singleton.LoadScene(
                "MenuPrincipal"
            );
        }

        // ============================================================
        // PLAYER
        // ============================================================

        private Vector3 GetPlayerPosition()
        {
            GameObject player =
                GameObject.FindGameObjectWithTag("Player");

            if (player == null)
            {
                Debug.LogWarning(
                    "[PauseMenuEvents] Player não encontrado."
                );

                return Vector3.zero;
            }

            return player.transform.position;
        }

        // ============================================================
        // EVENTS
        // ============================================================

        private void RegisterEvents()
        {
            ClearEvents();

            // --------------------------------------------------------
            // Menu de Pause
            // --------------------------------------------------------

            if (m_SaveGameButton != null)
            {
                m_SaveGameButton.onClick.AddListener(
                    OpenSaveMenu
                );
            }

            if (m_LoadGameButton != null)
            {
                m_LoadGameButton.onClick.AddListener(
                    OpenLoadMenu
                );
            }

            if (m_BackToMenuButton != null)
            {
                m_BackToMenuButton.onClick.AddListener(
                    BackToMainMenu
                );
            }

            if (m_ResumeButton != null)
            {
                m_ResumeButton.onClick.AddListener(
                    ResumeGame
                );
            }

            // --------------------------------------------------------
            // Save Slots
            // --------------------------------------------------------

            if (m_SaveSlot1Button != null)
            {
                m_SaveSlot1Button.onClick.AddListener(
                    () => SaveSlot(1)
                );
            }

            if (m_SaveSlot2Button != null)
            {
                m_SaveSlot2Button.onClick.AddListener(
                    () => SaveSlot(2)
                );
            }

            if (m_SaveSlot3Button != null)
            {
                m_SaveSlot3Button.onClick.AddListener(
                    () => SaveSlot(3)
                );
            }

            // --------------------------------------------------------
            // Load Slots
            // --------------------------------------------------------

            if (m_LoadSlot1Button != null)
            {
                m_LoadSlot1Button.onClick.AddListener(
                    () => LoadSlot(1)
                );
            }

            if (m_LoadSlot2Button != null)
            {
                m_LoadSlot2Button.onClick.AddListener(
                    () => LoadSlot(2)
                );
            }

            if (m_LoadSlot3Button != null)
            {
                m_LoadSlot3Button.onClick.AddListener(
                    () => LoadSlot(3)
                );
            }

            // --------------------------------------------------------
            // Voltar
            // --------------------------------------------------------

            if (m_SaveSlotsBackButton != null)
            {
                m_SaveSlotsBackButton.onClick.AddListener(
                    BackToPauseMenu
                );
            }

            if (m_LoadSlotsBackButton != null)
            {
                m_LoadSlotsBackButton.onClick.AddListener(
                    BackToPauseMenu
                );
            }
        }

        private void ClearEvents()
        {
            if (m_SaveGameButton != null)
                m_SaveGameButton.onClick.RemoveAllListeners();

            if (m_LoadGameButton != null)
                m_LoadGameButton.onClick.RemoveAllListeners();

            if (m_BackToMenuButton != null)
                m_BackToMenuButton.onClick.RemoveAllListeners();

            if (m_ResumeButton != null)
                m_ResumeButton.onClick.RemoveAllListeners();

            if (m_SaveSlot1Button != null)
                m_SaveSlot1Button.onClick.RemoveAllListeners();

            if (m_SaveSlot2Button != null)
                m_SaveSlot2Button.onClick.RemoveAllListeners();

            if (m_SaveSlot3Button != null)
                m_SaveSlot3Button.onClick.RemoveAllListeners();

            if (m_LoadSlot1Button != null)
                m_LoadSlot1Button.onClick.RemoveAllListeners();

            if (m_LoadSlot2Button != null)
                m_LoadSlot2Button.onClick.RemoveAllListeners();

            if (m_LoadSlot3Button != null)
                m_LoadSlot3Button.onClick.RemoveAllListeners();

            if (m_SaveSlotsBackButton != null)
                m_SaveSlotsBackButton.onClick.RemoveAllListeners();

            if (m_LoadSlotsBackButton != null)
                m_LoadSlotsBackButton.onClick.RemoveAllListeners();
        }

        // ============================================================
        // PAINÉIS
        // ============================================================

        private void CloseAllPanels()
        {
            if (m_PausePanel != null)
                m_PausePanel.SetActive(false);

            if (m_SaveSlotsPanel != null)
                m_SaveSlotsPanel.SetActive(false);

            if (m_LoadSlotsPanel != null)
                m_LoadSlotsPanel.SetActive(false);
        }

        // ============================================================
        // DESTROY
        // ============================================================

        private void OnDestroy()
        {
            ClearEvents();
        }
    }
}
