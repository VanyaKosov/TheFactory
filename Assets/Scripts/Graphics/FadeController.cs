using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.Kosov.Factory.Graphics
{
    public class FadeController : MonoBehaviour
    {
        public const float fadeDuration = 3f;

        public event EventHandler<EventArgs> Faded;

        public Image FadePanel;

        public void SetFadePercent(float percent)
        {
            Color color = FadePanel.color;

            FadePanel.color = new(color.r, color.g, color.b, percent);
        }

        public void FadeIn()
        {
            StartCoroutine(Fade(0, 1));
        }

        public void FadeOut()
        {
            StartCoroutine(Fade(1, -1));
        }
        private IEnumerator Fade(int startingValue, int direction) // direction should be 1 or -1
        {
            float timeStarted = Time.time;

            float time = Time.time;
            Color color;
            while (time - timeStarted <= fadeDuration)
            {
                color = FadePanel.color;
                FadePanel.color = new(color.r, color.g, color.b, startingValue + direction * (time - timeStarted) / fadeDuration);

                if (color.a < 0f)
                {
                    color.a = 0f;
                    yield break;
                }

                if (color.a > 1f)
                {
                    color.a = 1f;
                    yield break;
                }

                yield return null;

                time = Time.time;
            }

            color = FadePanel.color;
            if (direction == 1)
            {
                FadePanel.color = new(color.r, color.g, color.b, 1f);
                Faded?.Invoke(this, new());
            }
            else
            {
                FadePanel.color = new(color.r, color.g, color.b, 0f);
            }
        }

    }
}
