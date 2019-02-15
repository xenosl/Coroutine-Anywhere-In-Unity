using UnityEditor;

namespace ShuHai.Unity.CoroutineAnywhere.Editor
{
    internal static class CoroutineRunner
    {
        private static void Update() { Coroutine.UpdateAll(UpdateMethod.EditorUpdate); }

        [InitializeOnLoadMethod]
        private static void Initialize() { EditorApplication.update += Update; }
    }
}