using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Dev.Kosov.Factory.Graphics
{
    public class MainMenuController : MonoBehaviour
    {
        private const float splashTime = 0.5f;
        private const int minSplashSize = 23;
        private const int maxSplashSize = 26;
        private readonly string[] splashTexts =
        {
            "Longest conveyor by Kut0lui_",
            "How do I craft rocket fuel?",
            "Where have I seen this before?",
            "No bugs whatsoever...",
            "Factory wars: Trees strike back",
            "Here we go again",
            "I forgot...",
            "Also try Factorio"
        };
        private bool controlsOpen = false;
        private bool tutorialOpen = false;
        private int splashDirection = 1;
        private Scene loadingScene;
        private AsyncOperation loadingOperation = null;

        public GameObject EventSystem;
        public GameObject Loading;
        public Slider ProgressBar;
        public TMP_Text SplashText;
        public GameObject Controls;
        public GameObject Tutorial;

        void Start()
        {
            SplashText.text = splashTexts[Random.Range(0, splashTexts.Length)];
        }

        void Update()
        {
            if (loadingOperation != null)
            {
                Loading.SetActive(true);
                ProgressBar.value = loadingOperation.progress;
            }

            CheckForSceneLoad();
            UpdateSplashSize();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (controlsOpen)
                {
                    ToggleControls();
                }

                if (tutorialOpen)
                {
                    ToggleTutorial();
                }
            }
        }

        public void StartGame()
        {
            Destroy(EventSystem);

            loadingOperation = SceneManager.LoadSceneAsync("World", LoadSceneMode.Additive);
            loadingScene = SceneManager.GetSceneByName("World");
        }

        public void Exit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        public void ToggleTutorial()
        {
            tutorialOpen = !tutorialOpen;
            if (tutorialOpen)
            {
                controlsOpen = false;
                Controls.SetActive(false);

                Tutorial.SetActive(true);
            }
            else
            {
                Tutorial.SetActive(false);
            }
        }

        public void ToggleControls()
        {
            controlsOpen = !controlsOpen;
            if (controlsOpen)
            {
                tutorialOpen = false;
                Tutorial.SetActive(false);

                Controls.SetActive(true);
            }
            else
            {
                Controls.SetActive(false);
            }
        }

        private void UpdateSplashSize()
        {
            SplashText.fontSize += splashDirection * (maxSplashSize - minSplashSize) / splashTime * Time.deltaTime;
            if (SplashText.fontSize > maxSplashSize)
            {
                splashDirection = -1;
                SplashText.fontSize = maxSplashSize;
            }
            else if (SplashText.fontSize < minSplashSize)
            {
                splashDirection = 1;
                SplashText.fontSize = minSplashSize;
            }
        }

        private void CheckForSceneLoad()
        {
            if (!loadingScene.IsValid() || !loadingScene.isLoaded) return;

            SceneManager.UnloadSceneAsync("MainMenu");
            SceneManager.SetActiveScene(loadingScene);
        }
    }
}
