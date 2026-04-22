using UnityEngine;
using UnityEngine.SceneManagement;

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
    }
}
