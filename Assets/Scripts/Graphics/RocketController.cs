using Dev.Kosov.Factory.Core;
using System;
using UnityEngine;

namespace Dev.Kosov.Factory.Graphics
{
    public class RocketController : MonoBehaviour, IEntityInitializer
    {
        private const float rocketAccelerationTimeLimit = 11f;
        private const float playerVertOffset = 2f;
        private RocketSilo rocketSilo;
        private bool launched = false;
        private float timeLaunched = 0f;
        private float height;
        private GameObject player;
        private FadeController fadeController;
        private bool faded = false;
        private VictoryScreenController victoryScreen;

        public GameObject Rocket;

        void Start()
        {
            player = FindFirstObjectByType<PlayerController>().gameObject;
            fadeController = FindFirstObjectByType<FadeController>();
            victoryScreen = FindFirstObjectByType<VictoryScreenController>();

            height = Rocket.transform.position.y;
            rocketSilo.RocketLaunch += LaunchRocket;
            fadeController.Faded += OnFaded;
        }

        void Update()
        {
            VictoryScreen();
            UpdateRocketPos();
        }

        private void VictoryScreen()
        {
            if (!launched) return;
            if (faded) return;

            if (Time.time - timeLaunched >= rocketAccelerationTimeLimit)
            {
                faded = true;
                fadeController.FadeIn();
            }
        }

        private void UpdateRocketPos()
        {
            if (!launched) return;

            float time = Time.time - timeLaunched;

            Vector3 pos = Rocket.transform.position;
            float newHeight = -100f / (time - 15f) - 20f / 3f;
            if (time > rocketAccelerationTimeLimit) newHeight = -100f / (rocketAccelerationTimeLimit - 15f) - 20f / 3f;
            height += newHeight * Time.deltaTime;
            Rocket.transform.position = new(pos.x, height, pos.z);

            pos = Rocket.transform.position;
            player.transform.position = new(pos.x, pos.y + playerVertOffset, pos.z);
        }

        private void LaunchRocket(object sender, EventArgs args)
        {
            launched = true;
            timeLaunched = Time.time;
        }

        void IEntityInitializer.Init(Entity entity, Catalogs catalogs)
        {
            rocketSilo = (RocketSilo)entity;
        }

        private void OnFaded(object sender, EventArgs args)
        {
            launched = false;
            victoryScreen.Display(true);
            Rocket.SetActive(false);
        }
    }
}
