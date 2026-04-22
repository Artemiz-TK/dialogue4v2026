using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace Core
{
    public class BootLoader : MonoBehaviour
    {
        public string nextScene = "SimpleScene";
        public float delay = 2f;

        public void LoadNextScene()
        {
            SceneManager.LoadScene(nextScene);
            Debug.Log($"BootLoader: Loaded scene '{nextScene}'");
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif

        }
    }
}
