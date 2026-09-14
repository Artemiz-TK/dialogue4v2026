using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    /// <summary>
    /// Responsável exclusivamente pelos eventos dos Buttons
    /// do Menu Principal.
    ///
    /// Não modifica:
    /// - posição
    /// - tamanho
    /// - fonte
    /// - cor
    /// - layout
    /// - navegação
    /// - hierarquia
    /// </summary>
    public class MainMenuEvents : MonoBehaviour
    {
        [Header("Main Menu")]
        [SerializeField]
        private Button m_ContinueButton;

        [SerializeField]
        private Button m_NewGameButton;

        [SerializeField]
        private Button m_LoadGameButton;

        [SerializeField]
        private Button m_QuitButton;

        [Header("Save Slots")]
        [SerializeField]
        private Button m_Slot1Button;

        [SerializeField]
        private Button m_Slot2Button;

        [SerializeField]
        private Button m_Slot3Button;

        [SerializeField]
        private Button m_BackButton;

        [Header("Panels")]
        [SerializeField]
        private GameObject m_MainPanel;

        [SerializeField]
        private GameObject m_LoadPanel;

        private MainSceneLoader m_Loader;

        private void Awake()
        {
            m_Loader =
                GetComponent<MainSceneLoader>();

            if (m_Loader == null)
            {
                m_Loader =
                    gameObject.AddComponent<MainSceneLoader>();
            }

            RegisterEvents();
        }

        private void Start()
        {
            RefreshContinueButton();

            ShowMainMenu();
        }

        // ============================================================
        // EVENTOS
        // ============================================================

        private void RegisterEvents()
        {
            ClearEvents();

            // --------------------------------------------------------
            // Menu Principal
            // --------------------------------------------------------

            if (m_ContinueButton != null)
            {
                m_ContinueButton.onClick.AddListener(
                    m_Loader.ContinueGame
                );
            }

            if (m_NewGameButton != null)
            {
                m_NewGameButton.onClick.AddListener(
                    m_Loader.NewGame
                );
            }

            if (m_LoadGameButton != null)
            {
                m_LoadGameButton.onClick.AddListener(
                    OpenLoadMenu
                );
            }

            if (m_QuitButton != null)
            {
                m_QuitButton.onClick.AddListener(
                    m_Loader.Quit
                );
            }

            // --------------------------------------------------------
            // Slots
            // --------------------------------------------------------

            if (m_Slot1Button != null)
            {
                m_Slot1Button.onClick.AddListener(
                    () => m_Loader.LoadSlot(1)
                );
            }

            if (m_Slot2Button != null)
            {
                m_Slot2Button.onClick.AddListener(
                    () => m_Loader.LoadSlot(2)
                );
            }

            if (m_Slot3Button != null)
            {
                m_Slot3Button.onClick.AddListener(
                    () => m_Loader.LoadSlot(3)
                );
            }

            if (m_BackButton != null)
            {
                m_BackButton.onClick.AddListener(
                    ShowMainMenu
                );
            }
        }

        private void ClearEvents()
        {
            if (m_ContinueButton != null)
                m_ContinueButton.onClick.RemoveAllListeners();

            if (m_NewGameButton != null)
                m_NewGameButton.onClick.RemoveAllListeners();

            if (m_LoadGameButton != null)
                m_LoadGameButton.onClick.RemoveAllListeners();

            if (m_QuitButton != null)
                m_QuitButton.onClick.RemoveAllListeners();

            if (m_Slot1Button != null)
                m_Slot1Button.onClick.RemoveAllListeners();

            if (m_Slot2Button != null)
                m_Slot2Button.onClick.RemoveAllListeners();

            if (m_Slot3Button != null)
                m_Slot3Button.onClick.RemoveAllListeners();

            if (m_BackButton != null)
                m_BackButton.onClick.RemoveAllListeners();
        }

        // ============================================================
        // MENU
        // ============================================================

        private void OpenLoadMenu()
        {
            if (m_MainPanel != null)
                m_MainPanel.SetActive(false);

            if (m_LoadPanel != null)
                m_LoadPanel.SetActive(true);
        }

        private void ShowMainMenu()
        {
            if (m_MainPanel != null)
                m_MainPanel.SetActive(true);

            if (m_LoadPanel != null)
                m_LoadPanel.SetActive(false);
        }

        // ============================================================
        // CONTINUE
        // ============================================================

        private void RefreshContinueButton()
        {
            if (m_ContinueButton == null)
                return;

            bool hasAutoSave =
                SaveSystem.Singleton != null &&
                SaveSystem.Singleton.HasSave(0);

            m_ContinueButton.gameObject.SetActive(
                hasAutoSave
            );
        }

        private void OnDestroy()
        {
            ClearEvents();
        }
    }
}