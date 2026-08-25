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
            private readonly Dictionary<string, GameState> m_Mapper = new();
            
            public Dictionary<string, GameState> Mapper => m_Mapper;

            public HashMapper()
            {
                foreach (var state in Enum.GetValues(typeof(GameState)).Cast<GameState>())
                {
                    m_Mapper[state.ToString()] = state;
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
            return m_CurrentState == GameState.Fase1 || m_CurrentState == GameState.Fase2;
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
                    return newState == GameState.Fase1;
                case GameState.Fase1:
                    return newState == GameState.MenuPrincipal;
                default:
                    return false;
            }
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
            yield return SceneManager.LoadSceneAsync("Fase1", LoadSceneMode.Single);

            // 2. Carrega a cena de interface por cima (Aditiva)
            yield return SceneManager.LoadSceneAsync("GUI", LoadSceneMode.Additive);

            State = GameState.Fase1;
            Debug.Log($"GameManager: State changed to {m_CurrentState}");
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
    }
}
