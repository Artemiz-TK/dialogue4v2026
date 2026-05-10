using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
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

            public HashMap(string firstState, string secondState, string thirdState)
            {
                getSceneDictionary.Add(firstState, GameState.Splash);
                getSceneDictionary.Add(secondState, GameState.MenuPrincipal);
                getSceneDictionary.Add(thirdState, GameState.Gameplay);
            }
        }

        private HashMap _hashTable = new("Splash", "MenuPrincipal", "SampleScene");

        private static GameManager _instance;

        private GameState _currentState;

        public static GameManager Instance => _instance;

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

        public void StartGame()
        {
            LoadScene("SampleScene");
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
