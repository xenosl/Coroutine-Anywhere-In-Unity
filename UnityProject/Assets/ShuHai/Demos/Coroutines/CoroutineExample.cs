using System.Collections;
using UnityEngine;

namespace ShuHai.Unity.Coroutines.Demos
{
    public class CoroutineExample : MonoBehaviour
    {
        public Material ColorGradientMaterial;

        public void StartColorGradient() { Coroutine.Start(ColorGradient()); }

        private IEnumerator ColorGradient()
        {
            if (!ColorGradientMaterial)
                yield break;

            var color = ColorGradientMaterial.color;
            var a = color.a;
            a -= 0.01f;
            a = Mathf.Abs(a % 1f);
            color.a = a;
            ColorGradientMaterial.color = color;

            yield return null;
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Color Gradient"))
                StartColorGradient();
        }
    }
}