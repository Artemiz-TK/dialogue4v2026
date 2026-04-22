using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class SceneManagerCore : MonoBehaviour
    {
        private static SceneManagerCore _instance;

        public static SceneManagerCore Instance => _instance;

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

        /// <summary>
        /// Requests to transition to the main menu scene if everything is okay.
        /// </summary>
        public void RequestTransitionToMainMenu()
        {
            if (GameManager.Instance.CanTransitionTo(GameState.MenuPrincipal))
            {
                GameManager.SetState(GameState.MenuPrincipal);
            }
            else
            {
                Debug.LogWarning("SceneManager: Cannot transition to Main Menu at this time.");
            }
        }

        /// <summary>
        /// Requests to transition to gameplay scene if everything is okay.
        /// </summary>
        public void RequestTransitionToGameplay()
        {
            if (GameManager.Instance.CanTransitionTo(GameState.Gameplay))
            {
                GameManager.SetState(GameState.Gameplay);
            }
            else
            {
                Debug.LogWarning("SceneManager: Cannot transition to Gameplay at this time.");
            }
        }
    }
}
