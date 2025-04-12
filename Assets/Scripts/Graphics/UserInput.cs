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

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                OpenInventory?.Invoke(this, new());
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                RotateEntity?.Invoke(this, new());
            }
        }
    }
}
