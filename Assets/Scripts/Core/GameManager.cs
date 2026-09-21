using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Linq;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public enum GameState
        {
            Iniciando,
            Splash,
            MenuPrincipal,
            Fase1,
            Fase2,
        }

        private class HashMapper
        {
            public readonly Dictionary<string, GameState> Mapper = new();

            public HashMapper()
            {
                foreach (var state in Enum.GetValues(typeof(GameState)).Cast<GameState>())
                {
                    Mapper[state.ToString()] = state;
                }
            }
        }

        private HashMapper m_HashTable = new();

        private static GameManager s_Instance;

        private GameState m_CurrentState;
        public GameState State
        {
            set
            {
                m_CurrentState = value;
                if (m_CurrentState == GameState.Fase1)
                    OnGameplayEntered?.Invoke();
            }
        }

        private Vector3 m_CurrentPhaseStartPosition;

        public Vector3 CurrentPhaseStartPosition =>
            m_CurrentPhaseStartPosition;

        public static GameManager Singleton => s_Instance;
        public static event Action OnGameplayEntered;

        private void Awake()
        {
            if (s_Instance != null && s_Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            s_Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            OnGameplayEntered += HandleGameplayEnter;
        }
        private void OnDisable()
        {
            OnGameplayEntered -= HandleGameplayEnter;
        }

        private void Start()
        {
            m_CurrentState = GameState.Iniciando;
            Debug.Log($"GameManager: State changed to {m_CurrentState}");
        }

        public bool IsInGameplay()
        {
            return m_CurrentState is GameState.Fase1 or GameState.Fase2;
        }

        private bool CanTransitionTo(GameState newState)
        {
            switch (m_CurrentState)
            {
                case GameState.Iniciando:
                    return newState == GameState.Splash;
                case GameState.Splash:
                    return newState == GameState.MenuPrincipal;
                case GameState.MenuPrincipal:
                    return newState is GameState.Fase1 or GameState.Fase2;
                case GameState.Fase1:
                    return newState is GameState.Fase2 or GameState.MenuPrincipal;
                case GameState.Fase2:
                    return newState == GameState.MenuPrincipal;
                default:
                    return false;
            }
        }

        public async Awaitable LoadLastPhase()
        {
            // ============================================================
            // PREPARA A NOVA FASE
            // ============================================================

            if (SaveSystem.Singleton != null)
            {
                SaveSystem.Singleton.PrepareNextPhase(
                    "Fase2"
                );
            }

            if (QuantityManager.Singleton != null)
            {
                QuantityManager.Singleton.ResetQuantity();
            }

            // ============================================================
            // CARREGA FASE 2
            // ============================================================

            await SceneManager.LoadSceneAsync(
                "Fase2",
                LoadSceneMode.Single
            );

            await SceneManager.LoadSceneAsync(
                "GUI",
                LoadSceneMode.Additive
            );

            State = GameState.Fase2;

            Debug.Log(
                "[GameManager] Fase 2 carregada. " +
                "Quantidade de moedas resetada."
            );
        }
        
        public void StartNewGame()
        {
            // if (SaveSystem.Singleton != null)
            // {
            //     SaveSystem.Singleton.NewGame();
            // }

            StartGame();
        }

        // =====================================================================
        // LOAD DE SLOT
        // =====================================================================

        public void LoadSaveSlot(int slot)
        {
            if (SaveSystem.Singleton == null)
            {
                Debug.LogError(
                    "[GameManager] SaveSystem não encontrado."
                );

                return;
            }

            if (!SaveSystem.Singleton.LoadFromFile(slot))
            {
                Debug.LogWarning(
                    $"[GameManager] Não foi possível carregar o slot {slot}."
                );

                return;
            }

            string sceneName =
                SaveSystem.Singleton.LoadSceneName(slot);

            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning(
                    $"[GameManager] Slot {slot} não possui uma cena salva."
                );

                return;
            }

            StartCoroutine(
                LoadSavedSceneRoutine(sceneName)
            );
        }

        // =====================================================================
        // LOAD DA CENA SALVA
        // =====================================================================

        private IEnumerator LoadSavedSceneRoutine(string sceneName)
        {
            if (sceneName != "Fase1" &&
                sceneName != "Fase2")
            {
                Debug.LogWarning(
                    $"GameManager: Cena de save inválida: {sceneName}"
                );

                yield break;
            }

            // ============================================================
            // CARREGA A FASE
            // ============================================================

            yield return SceneManager.LoadSceneAsync(
                sceneName,
                LoadSceneMode.Single
            );

            // Registra a posição original da fase.
            CapturePhaseStartPosition();

            // ============================================================
            // CARREGA A GUI
            // ============================================================

            yield return SceneManager.LoadSceneAsync(
                "GUI",
                LoadSceneMode.Additive
            );

            yield return null;

            // ============================================================
            // RESTAURA CHECKPOINT
            // ============================================================

            if (SaveSystem.Singleton != null &&
                SaveSystem.Singleton.HasCheckpoint())
            {
                if (SaveSystem.Singleton.LoadCheckpointPosition(
                        out Vector3 checkpointPosition))
                {
                    if (PlayerController.Singleton != null)
                    {
                        PlayerController.Singleton.SetPosition(
                            checkpointPosition
                        );

                        Debug.Log(
                            $"[GameManager] Player restaurado no checkpoint: " +
                            $"{checkpointPosition}"
                        );
                    }
                }

                if (SaveSystem.Singleton.LoadCheckpointCoins(
                        out int checkpointCoins))
                {
                    if (QuantityManager.Singleton != null)
                    {
                        QuantityManager.Singleton.SetQuantity(
                            checkpointCoins
                        );
                    }
                }
            }
            else
            {
                if (QuantityManager.Singleton != null)
                {
                    QuantityManager.Singleton.ResetQuantity();
                }
            }

            // ============================================================
            // ESTADO
            // ============================================================

            State =
                sceneName == "Fase1"
                    ? GameState.Fase1
                    : GameState.Fase2;

            Debug.Log(
                $"GameManager: Save carregado na cena '{sceneName}'."
            );
        }

        public void LoadSceneWithGUI(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
            SceneManager.LoadScene("GUI", LoadSceneMode.Additive);
            State = m_HashTable.Mapper[sceneName];
        }
        
        public void LoadScene(string scene)
        {
            var state = m_HashTable.Mapper[scene];
            if (!CanTransitionTo(state))
                return;

            switch (state)
            {
                case GameState.Splash:
                    ChangeScene("Splash");
                    break;
                case GameState.MenuPrincipal:
                    ChangeScene("MenuPrincipal");
                    break;
                case GameState.Fase1:
                    ChangeScene("Fase1");
                    break;
                case GameState.Fase2:
                    ChangeScene("Fase2");
                    break;
                case GameState.Iniciando:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            m_CurrentState = state;
            Debug.Log($"GameManager: State changed to {m_CurrentState}");
        }

        private void CapturePhaseStartPosition()
        {
            var player =
                GameObject.FindGameObjectWithTag("Player");

            if (player == null)
            {
                Debug.LogWarning(
                    "[GameManager] Não foi possível encontrar o Player " +
                    "para registrar a posição inicial da fase."
                );

                m_CurrentPhaseStartPosition =
                    new Vector3(0f, 1.49f, 20.58f);

                return;
            }

            m_CurrentPhaseStartPosition =
                player.transform.position;

            Debug.Log(
                $"[GameManager] Posição inicial da fase registrada: " +
                $"{m_CurrentPhaseStartPosition}"
            );
        }

        private void HandleGameplayEnter()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void StartGame()
        {
            StartCoroutine(StartGameRoutine());
        }

        private IEnumerator StartGameRoutine()
        {
            yield return SceneManager.LoadSceneAsync(
                "Fase1",
                LoadSceneMode.Single
            );

            // A posição original da Fase 1 é registrada
            // antes de qualquer restauração de save.
            CapturePhaseStartPosition();

            yield return SceneManager.LoadSceneAsync(
                "GUI",
                LoadSceneMode.Additive
            );

            State = GameState.Fase1;

            Debug.Log(
                $"GameManager: State changed to {m_CurrentState}"
            );
        }

        public void Quit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void ChangeScene(string sceneName)
        {
            var state = m_HashTable.Mapper[sceneName];
            if (!CanTransitionTo(state))
            {
                Debug.LogWarning("Scene switch not allowed right now.");
                return;
            }

            SceneManager.LoadScene(sceneName);
        }
        
        // ============================================================
// CARREGAMENTO DE SAVE
// ============================================================

        public void LoadSavedScene(string sceneName)
        {
            StartCoroutine(
                LoadSavedSceneRoutine(sceneName)
            );
        }

        public void LoadSceneWithoutState(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            SceneManager.LoadScene(sceneName, mode);
        }
    }
}
