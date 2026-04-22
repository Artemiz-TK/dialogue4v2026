using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        
        public static GameManager Instance => _instance;
        private GameState _currentState;
        

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            _currentState = GameState.Iniciando;
        }

        public bool IsInitialized()
        {
            return _currentState == GameState.Iniciando;
        }
        
        public bool IsInMainMenu()
        {
            return _currentState == GameState.MenuPrincipal;
        }
        
        public bool IsInGameplay()
        {
            return _currentState == GameState.Gameplay;
        }
        
        public bool CanTransitionTo(GameState newState)
        {
            switch (_currentState)
            {
                case GameState.Iniciando:
                    return newState == GameState.MenuPrincipal;
                case GameState.MenuPrincipal:
                    return newState == GameState.Gameplay;
                case GameState.Gameplay:
                    return newState == GameState.MenuPrincipal; // Allow returning to menu, or add logic for next levels if needed
                default:
                    return false;
            }
        }

        public static void SetState(GameState newState)
        {
            if (_instance == null)
            {
                Debug.LogError("GameManager instance is null");
                return;
            }
            if (_instance.CanTransitionTo(newState))
            {
                _instance._currentState = newState;
                Debug.Log($"GameManager: State changed to {newState}");

                // Load the appropriate scene based on the new state
                if (newState == GameState.Gameplay)
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
                }
                else if (newState == GameState.MenuPrincipal)
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("MenuPrincipal");
                }
            }
            else
            {
                Debug.LogWarning($"GameManager: Cannot transition from {_instance._currentState} to {newState}");
                UnityEngine.SceneManagement.SceneManager.LoadScene("MenuPrincipal");
            }
        }

        public static void SetStateToGameplay()
        {
            SetState(GameState.Gameplay);
        }
    }
}
