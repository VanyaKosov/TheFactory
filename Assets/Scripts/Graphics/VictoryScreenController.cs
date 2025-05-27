using UnityEngine;

namespace Dev.Kosov.Factory.Graphics
{
    public class VictoryScreenController : MonoBehaviour
    {
        public GameObject VictoryScreen;
        public FadeController FadeController;
        public GameObject Player;

        public void Display(bool val)
        {
            VictoryScreen.SetActive(val);
        }

        public void OnReturnClick()
        {
            Player.transform.position = new(0, 0, Player.transform.position.z);
            FadeController.FadeOut();
            VictoryScreen.SetActive(false);
        }
    }
}
