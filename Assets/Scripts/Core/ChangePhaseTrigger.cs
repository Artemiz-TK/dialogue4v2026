using UnityEngine;

namespace Core
{
    [RequireComponent(typeof(BoxCollider))]
    public class ChangePhaseTrigger : MonoBehaviour
    {
        public string NextScene;

        private async Awaitable OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            await other.GetComponent<PlayerController>().MoveTowards(Vector3.zero, 1f);
            await Awaitable.WaitForSecondsAsync(0.67f);
            EventTriggers.SecondFaseSavedTrigger(NextScene);
            await GameManager.Singleton.LoadLastPhase();
        }
    }
}
