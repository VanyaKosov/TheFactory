using System;
using UnityEngine;
using UnityEditor;

namespace Dev.Kosov.Factory.Graphics
{
    public class EscapeMenuController : MonoBehaviour
    {
        private bool open = false;
        private bool controlsOpen = false;
        private bool tutorialOpen = false;

        public PlayerController PlayerController;
        public FadeController FadeController;
        public GameObject EscapeMenu;
        public GameObject Controls;
        public GameObject Tutorial;

        void Start()
        {
            PlayerController.OpenEscapeMenu += ToggleEscapeMenu;
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

        public void OpenEscapeMenu()
        {
            open = true;
            FadeController.SetFadePercent(0.5f);
            EscapeMenu.SetActive(true);
        }

        public void CloseEscapeMenu()
        {
            open = false;

            if (controlsOpen)
            {
                ToggleControls();
            }

            if (tutorialOpen)
            {
                ToggleTutorial();
            }

            EscapeMenu.SetActive(false);
            FadeController.SetFadePercent(0f);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        public void ToggleEscapeMenu(object sender, EventArgs args)
        {
            if (open)
            {
                CloseEscapeMenu();
            }
            else
            {
                OpenEscapeMenu();
            }
        }
    }
}
