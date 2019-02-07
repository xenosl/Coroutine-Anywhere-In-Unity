using UnityEditor;

namespace ShuHai.Unity.Coroutines.Editor
{
    internal static class CoroutineRunner
    {
        [InitializeOnLoadMethod]
        private static void Initialize() { EditorApplication.update += Coroutine.UpdateAll; }
    }
}