using UnityEngine;

namespace Core
{
    public class BootLoader : MonoBehaviour
    {
        public string nextScene = "SampleScene";

        public void LoadNextScene()
        {
            GameManager.Instance.ChangeState(GameState.Gameplay);
            Debug.Log($"BootLoader: Loaded scene '{nextScene}'");
            Debug.Log($"BootLoader -> GameManager: State changed to {GameManager.Instance.State}");
        }

        public void Quit()
        {
            GameManager.Instance.Quit();
        }
    }
}
