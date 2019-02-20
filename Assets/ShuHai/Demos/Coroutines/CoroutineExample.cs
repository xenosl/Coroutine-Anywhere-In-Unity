using System;
using System.Collections;
using UnityEngine;

namespace ShuHai.Unity.CoroutineAnywhere.Demos
{
    // The using statements below are recommended since the name "Coroutine" existed in UnityEngine namespace.
    //using ACoroutine = CoroutineAnywhere.Coroutine;
    //using Coroutine = CoroutineAnywhere.Coroutine;

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

        /// <summary>
        ///     Two ways to start a coroutine:
        ///     1. Call static method <see cref="Coroutine.Start(IEnumerator,Action)" />.
        ///     2. Create new instance of <see cref="Coroutine" />, and call instance method <see cref="Coroutine.Start()" />.
        ///     Here is the first way.
        /// </summary>
        public void StartRedGradient()
        {
            StopRedGradient();
            redGradientCoroutine = Coroutine.Start(ColorGradient(), OnRedGradientCoroutineDone);
        }

        /// <summary>
        ///     Two ways to stop a coroutine outside of the coroutine method:
        ///     1. Call static method <see cref="Coroutine.Stop(IEnumerator)" />.
        ///     2. Call instance method with previouly returned instance from <see cref="Coroutine.Start(IEnumerator,Action)" />
        ///     or previously created instance from a constructor.
        ///     Here is the second way.
        /// </summary>
        public void StopRedGradient()
        {
            if (redGradientCoroutine != null)
                redGradientCoroutine.Stop();
        }

        /// <summary>
        ///     You can stop a coroutine inside the coroutine methed by yield break.
        /// </summary>
        public void StopRedGradientByBreak() { redGradientCoroutineBreak = true; }

        private bool redGradientCoroutineBreak;
        private Coroutine redGradientCoroutine;

        private void OnRedGradientCoroutineDone() { redGradientCoroutine = null; }

        #endregion Start & Stop

        #region WaitForSeconds

        /// <summary>
        ///     Unity built-in yield instructions are supported, use it as what you do with Unity built-in coroutine.
        ///     The <see cref="UnityEngine.WaitForSeconds" /> yield is used here to pause the color gradient.
        /// </summary>
        public void PauseGradientForSeconds(float secnods)
        {
            if (redGradientCoroutine != null)
                pauseDuration = secnods;
        }

        private float? pauseDuration;

        #endregion WaitForSeconds

        #region Nested

        /// <summary>
        ///     Nested coroutine is supported by using the <see cref="Coroutine" /> as a yield instruction, that is
        ///     "yield return new Coroutine(YourNestedMethod())". Note that the host coroutine is suspended while the nested
        ///     coroutine running.
        /// </summary>
        public void StartNestedGreenGradient() { nestedGreenGradient = true; }

        /// <summary>
        ///     Just skip the yield instruction to avoid execution the nested coroutine. It's same as other yield instructions.
        /// </summary>
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

        /// <summary>
        ///     Coroutine execution method.
        /// </summary>
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