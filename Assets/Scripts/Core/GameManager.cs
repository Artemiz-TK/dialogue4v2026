using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        
        private GameState _currentState;

        private bool m_CanSwitchScene;
        
        public static GameManager Instance => _instance;
        public GameState State => _currentState;

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            _currentState = GameState.Iniciando;
            Debug.Log($"GameManager: State changed to {_currentState}");
            StartCoroutine(StartRoutine());
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
        
        public void ChangeState(GameState newState)
        {
            if (!CanTransitionTo(newState))
                return;

            _currentState = newState;

            switch (newState)
            {
                case GameState.Splash:
                    LoadScene("Splash");
                    break;
                case GameState.MenuPrincipal:
                    LoadScene("MenuPrincipal");
                    break;
                case GameState.Gameplay:
                    LoadScene("SampleScene");
                    break;
            }

            Debug.Log($"GameManager: State changed to {newState}");
        }

        public void StartGame()
        {
            ChangeState(GameState.Gameplay);
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private IEnumerator StartRoutine()
        {
            yield return null;

            m_CanSwitchScene = true;
            ChangeState(GameState.Splash);

            yield return null;
            m_CanSwitchScene = false;

            yield return new WaitForSeconds(2f);

            m_CanSwitchScene = true;
            ChangeState(GameState.MenuPrincipal);
        }

        public void LoadScene(string sceneName)
        {
            if (!m_CanSwitchScene)
            {
                Debug.LogWarning("Scene switch not allowed right now.");
                return;
            }

            SceneManager.LoadScene(sceneName);
        }
    }
}
