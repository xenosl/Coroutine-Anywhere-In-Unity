namespace ShuHai.Unity.CoroutineAnywhere
{
    /// <summary>
    ///     Represents yield object adapter that converts any object to an <see cref="IYield" /> object. In this way you can
    ///     treat any object as yield object for the coroutine.
    /// </summary>
    public interface IYieldAdapter
    {
        /// <summary>
        ///     Converts specified object to yield object.
        /// </summary>
        /// <param name="yieldObject">Object that needs to convert to <see cref="IYield" /> object.</param>
        IYield ToYield(object yieldObject);
    }
}