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

        public void StartColorGradient() { ColorGradientCoroutine = Coroutine.Start(ColorGradient()); }

        public void StopColorGradient() { ColorGradientCoroutine.Stop(); }

        private Coroutine ColorGradientCoroutine;

        private IEnumerator ColorGradient()
        {
            if (!MaterialToGradient)
                yield break;

            float value = MaterialToGradient.color.r, step = 0.01f;
            float l = step, h = 1 - step;
            while (true)
            {
                if (value < l || value > h)
                    step = -step;
                value = Mathf.Abs((value + step) % 1f);
                MaterialToGradient.color = new Color(value, value, 1);
                yield return null;
            }
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Start Color Gradient"))
                StartColorGradient();
            if (GUILayout.Button("Stop Color Gradient"))
                StopColorGradient();
        }
    }
}