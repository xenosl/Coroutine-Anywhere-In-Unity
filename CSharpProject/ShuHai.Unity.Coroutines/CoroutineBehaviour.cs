using UnityEngine;

namespace ShuHai.Unity.Coroutines
{
    /// <summary>
    ///     Runtime runner for <see cref="Coroutine" />.
    /// </summary>
    internal class CoroutineBehaviour : MonoBehaviour
    {
        #region Instance

        /// <summary>
        ///     Single instance of <see cref="CoroutineBehaviour" />.
        /// </summary>
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

        #region Updates

        private void Update()
        {
            if (Application.isPlaying)
                Coroutine.UpdateAll(UpdateMethod.Update);
        }

        private void LateUpdate()
        {
            if (Application.isPlaying)
                Coroutine.UpdateAll(UpdateMethod.LateUpdate);
        }

        private void OnGUI()
        {
            if (Application.isPlaying)
                Coroutine.UpdateAll(UpdateMethod.OnGUI);
        }

        #endregion Updates
    }
}