using UnityEngine;

namespace ShuHai.Unity.Coroutines
{
    internal class CoroutineBehaviour : MonoBehaviour
    {
        #region Instance

        public static CoroutineBehaviour Instance => GetOrCreate();

        private static CoroutineBehaviour instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static CoroutineBehaviour GetOrCreate()
        {
            if (instance)
                return instance;

            var gameObject = new GameObject
            {
                name = typeof(CoroutineBehaviour).Name,
                hideFlags = HideFlags.HideAndDontSave
            };
            DontDestroyOnLoad(gameObject);

            instance = gameObject.AddComponent<CoroutineBehaviour>();

            return instance;
        }

        #endregion Instance

        private void Update()
        {
            if (!Application.isEditor)
                Coroutine.UpdateAll();
        }
    }
}