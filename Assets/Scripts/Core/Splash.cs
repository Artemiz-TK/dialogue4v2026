using UnityEngine;
using System.Collections;

namespace Core
{

    public class Splash : MonoBehaviour
    {
        private static Splash _instance;
        public static Splash Instance => _instance;


        void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(this.gameObject);
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        void Start()
        {
            StartCoroutine(StartRoutine());
        }

        private IEnumerator StartRoutine()
        {
            yield return null;

            GameManager.Instance.LoadScene("Splash");

            yield return new WaitForSeconds(2f);

            GameManager.Instance.LoadScene("MenuPrincipal");
        }
    }
}
