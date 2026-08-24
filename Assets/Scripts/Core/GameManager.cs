using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public enum GameState
        {
            Iniciando,
            Splash,
            MenuPrincipal,
            Gameplay
        }

        private class HashMap
        {
            public readonly Dictionary<string, GameState> getSceneDictionary = new();

            public HashMap(params string[] sceneNames)
            {
                foreach (var sceneName in sceneNames)
                {
                    getSceneDictionary.Add(sceneNames[0], GameState.Splash);
                }
                // getSceneDictionary.Add(sceneNames[0], GameState.Splash);
                // getSceneDictionary.Add(secondState, GameState.MenuPrincipal);
                // getSceneDictionary.Add(thirdState, GameState.Gameplay);
            }
        }

        private HashMap _hashTable = new("Splash", "MenuPrincipal", "SampleScene");

        private static GameManager _instance;

        private GameState _currentState;
        public GameState State
        {
            set
            {
                _currentState = value;
                if (_currentState == GameState.Gameplay)
                    OnGameplayEntered?.Invoke();
            }
        }

        public static GameManager Instance => _instance;
        public static event Action OnGameplayEntered;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
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
            _currentState = GameState.Iniciando;
            Debug.Log($"GameManager: State changed to {_currentState}");
        }

        public bool IsInGameplay()
        {
            return _currentState == GameState.Gameplay;
        }

        private bool CanTransitionTo(GameState newState)
        {
            switch (_currentState)
            {
                case GameState.Iniciando:
                    return newState == GameState.Splash;
                case GameState.Splash:
                    return newState == GameState.MenuPrincipal;
                case GameState.MenuPrincipal:
                    return newState == GameState.Gameplay;
                case GameState.Gameplay:
                    return newState == GameState.MenuPrincipal;
                default:
                    return false;
            }
        }

        public void LoadScene(string scene)
        {
            var state = _hashTable.getSceneDictionary[scene];
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
                case GameState.Gameplay:
                    ChangeScene("SampleScene");
                    break;
            }

            _currentState = state;
            Debug.Log($"GameManager: State changed to {_currentState}");
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
            yield return SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Single);

            // 2. Carrega a cena de interface por cima (Aditiva)
            yield return SceneManager.LoadSceneAsync("GUI", LoadSceneMode.Additive);

            State = GameState.Gameplay;
            Debug.Log($"GameManager: State changed to {_currentState}");
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
            var state = _hashTable.getSceneDictionary[sceneName];
            if (!CanTransitionTo(state))
            {
                Debug.LogWarning("Scene switch not allowed right now.");
                return;
            }

            SceneManager.LoadScene(sceneName);
        }
    }
}
