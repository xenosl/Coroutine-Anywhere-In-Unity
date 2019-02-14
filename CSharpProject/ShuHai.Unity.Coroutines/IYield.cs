namespace ShuHai.Unity.Coroutines
{
    /// <summary>
    ///     Represents any yield object for <see cref="Coroutine" />.
    /// </summary>
    public interface IYield
    {
        /// <summary>
        ///     Indicates whether current instance is done. The coroutine is suspended while the value is <see langword="false" />
        ///     and <see cref="Update"/> is executed per frame.
        /// </summary>
        bool IsDone { get; }

        /// <summary>
        ///     Executes when the coroutine start current instance.
        /// </summary>
        void Start();

        /// <summary>
        ///     Executes when the coroutine stop current instance.
        /// </summary>
        void Stop();

        /// <summary>
        ///     Executes when the coroutine update current instance.
        /// </summary>
        void Update();
    }
}