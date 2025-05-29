using System;
using UnityEngine;

namespace Dev.Kosov.Factory.Graphics
{
    public class EscapeMenuController : MonoBehaviour
    {
        private bool open = false;
        private bool controlsOpen = false;

        public PlayerController PlayerController;
        public FadeController FadeController;
        public GameObject EscapeMenu;
        public GameObject Controls;

        void Start()
        {
            PlayerController.OpenEscapeMenu += ToggleEscapeMenu;
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

        public void ToggleControls()
        {
            controlsOpen = !controlsOpen;
            if (controlsOpen)
            {
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
            if (controlsOpen)
            {
                ToggleControls();
                return;
            }

            open = false;
            EscapeMenu.SetActive(false);
            FadeController.SetFadePercent(0f);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
