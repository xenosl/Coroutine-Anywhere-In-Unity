using System;

namespace ShuHai.Unity.Coroutines
{
    /// <summary>
    ///     Specifies from where the <see cref="Coroutine.Update" /> method of <see cref="Coroutine" /> is invoked.
    /// </summary>
    public enum UpdateMethod
    {
        /// <summary>
        ///     The <see cref="Coroutine.Update" /> method is not called automantically, user should call it manually.
        /// </summary>
        None,

        /// <summary>
        ///     The <see cref="Coroutine.Update" /> method is called from MonoBehaviour.Update.
        /// </summary>
        Update,

        /// <summary>
        ///     The <see cref="Coroutine.Update" /> method is called from MonoBehaviour.LateUpdate.
        /// </summary>
        LateUpdate,

        /// <summary>
        ///     The <see cref="Coroutine.Update" /> method is called from MonoBehaviour.OnGUI.
        /// </summary>
        OnGUI,

        /// <summary>
        ///     The <see cref="Coroutine.Update" /> method is called from UnityEditor.EditorApplication.update, and only available
        ///     in Unity Editor.
        /// </summary>
        EditorUpdate
    }

    /// <summary>
    ///     Traits of enum type <see cref="UpdateMethod" />
    /// </summary>
    public static class UpdateMethodTraits
    {
        /// <summary>
        ///     Number of members defined in <see cref="UpdateMethod" />.
        /// </summary>
        public static int Count { get { return values.Length; } }

        /// <summary>
        ///     Get value of <see cref="UpdateMethod" /> at specified index.
        /// </summary>
        public static UpdateMethod GetValue(int index) { return values[index]; }

        private static readonly UpdateMethod[] values;

        static UpdateMethodTraits() { values = (UpdateMethod[])Enum.GetValues(typeof(UpdateMethod)); }
    }
}