using System;
using UnityEngine;

namespace Dev.Kosov.Factory.Graphics
{
    public class UserInput : MonoBehaviour
    {
        public EventHandler<EventArgs> PrimaryInput;
        public EventHandler<EventArgs> SecondaryInput;
        public EventHandler<EventArgs> OpenInventory;
        public EventHandler<EventArgs> RotateEntity;
        public EventHandler<EventArgs> OpenPlayerCrafter;
        public EventHandler<EventArgs> Escape;

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                PrimaryInput?.Invoke(this, new());
            }

            if (Input.GetMouseButtonDown(1))
            {
                SecondaryInput?.Invoke(this, new());
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                OpenInventory?.Invoke(this, new());
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                RotateEntity?.Invoke(this, new());
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                OpenPlayerCrafter?.Invoke(this, new());
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Escape?.Invoke(this, new());
            }
        }
    }
}
