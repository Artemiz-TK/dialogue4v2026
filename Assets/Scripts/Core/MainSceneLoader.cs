using UnityEngine;

namespace Core
{
    public class MainSceneLoader : MonoBehaviour
    {
        public string nextScene = "SampleScene";

        public void LoadNextScene()
        {
            GameManager.Singleton.StartGame();
            Debug.Log($"MainSceneLoader: Loaded scene '{nextScene}'");
        }

        public void Quit()
        {
            GameManager.Singleton.Quit();
        }
    }
}
