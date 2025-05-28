using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Dev.Kosov.Factory.Graphics
{
    public class MainMenuController : MonoBehaviour
    {
        private Scene loadingScene;
        private AsyncOperation loadingOperation = null;

        public GameObject EventSystem;
        public GameObject Loading;
        public Slider ProgressBar;

        void Start()
        {
        
        }

        void Update()
        {
            if (loadingOperation != null)
            {
                Loading.SetActive(true);
                ProgressBar.value = loadingOperation.progress;
            }

            CheckForSceneLoad();
        }

        private void CheckForSceneLoad() {
            if (!loadingScene.IsValid() || !loadingScene.isLoaded) return;

            SceneManager.UnloadSceneAsync("MainMenu");
            SceneManager.SetActiveScene(loadingScene);
        }

        public void StartGame()
        {
            Destroy(EventSystem);

            loadingOperation = SceneManager.LoadSceneAsync("World", LoadSceneMode.Additive);
            loadingScene = SceneManager.GetSceneByName("World");
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}
