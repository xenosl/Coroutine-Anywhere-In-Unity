using System.Collections;
using UnityEngine;

namespace ShuHai.Unity.CoroutineAnywhere.Demos
{
    public class CoroutineExample : MonoBehaviour
    {
        public Renderer RendererToGradient;

        public Material MaterialToGradient
        {
            get
            {
                if (!RendererToGradient)
                    return null;
#if UNITY_EDITOR
                return UnityEditor.EditorApplication.isPlaying || UnityEditor.EditorApplication.isPaused
                    ? RendererToGradient.material : null;
#else
                return RendererToGradient.material;
#endif
            }
        }

        #region Start & Stop

        public void StartRedGradient()
        {
            StopRedGradient();
            redGradientCoroutine = Coroutine.Start(ColorGradient(), OnRedGradientCoroutineDone);
        }

        public void StopRedGradient()
        {
            if (redGradientCoroutine != null)
                redGradientCoroutine.Stop();
        }

        public void StopRedGradientByBreak() { redGradientCoroutineBreak = true; }

        private bool redGradientCoroutineBreak;
        private Coroutine redGradientCoroutine;

        private void OnRedGradientCoroutineDone() { redGradientCoroutine = null; }

        #endregion Start & Stop

        #region WaitForSeconds

        public void PauseGradientForSeconds(float secnods)
        {
            if (redGradientCoroutine != null)
                pauseDuration = secnods;
        }

        private float? pauseDuration;

        #endregion WaitForSeconds

        #region Nested

        public void StartNestedGreenGradient() { nestedGreenGradient = true; }
        public void StopNestedGreenGradient() { nestedGreenGradient = false; }

        private bool nestedGreenGradient;

        private IEnumerator GreenGradient()
        {
            while (true)
            {
                MaterialToGradient.color = colorGradient.NextGreen();
                if (!nestedGreenGradient)
                    yield break;
                yield return null;
            }
        }

        #endregion Nested

        private readonly ColorGradient colorGradient = new ColorGradient();

        private IEnumerator ColorGradient()
        {
            if (!MaterialToGradient)
                yield break;

            while (true)
            {
                MaterialToGradient.color = colorGradient.NextRed();

                // Support for UnityEngine.WaitForSeconds
                if (pauseDuration != null)
                {
                    yield return new WaitForSeconds(pauseDuration.Value);
                    pauseDuration = null;
                }

                // Nested coroutine
                if (nestedGreenGradient)
                    yield return new Coroutine(GreenGradient());

                // Stop current coroutine by break
                if (redGradientCoroutineBreak)
                {
                    redGradientCoroutineBreak = false;
                    yield break;
                }

                yield return null;
            }
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Start Red Gradient"))
                StartRedGradient();
            if (GUILayout.Button("Stop Red Gradient"))
                StopRedGradient();
            if (GUILayout.Button("Stop Red Gradient By Break"))
                StopRedGradientByBreak();

            if (GUILayout.Button("Pause Gradient For 2 seconds"))
                PauseGradientForSeconds(2);

            if (GUILayout.Button("Start Nested Green Gradient"))
                StartNestedGreenGradient();
            if (GUILayout.Button("Stop Nested Green Gradient"))
                StopNestedGreenGradient();
        }
    }
}