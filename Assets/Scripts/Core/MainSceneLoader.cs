using UnityEngine;

namespace Core
{
    public class MainSceneLoader : MonoBehaviour
    {
        public string nextScene = "SampleScene";

        public void LoadNextScene()
        {
            GameManager.Instance.LoadScene(nextScene);
            Debug.Log($"BootLoader: Loaded scene '{nextScene}'");
        }

        public void Quit()
        {
            GameManager.Instance.Quit();
        }
    }
}
