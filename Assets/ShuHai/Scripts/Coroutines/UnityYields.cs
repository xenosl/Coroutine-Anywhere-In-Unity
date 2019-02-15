using UnityEngine;

namespace ShuHai.Unity.CoroutineAnywhere
{
    /// <summary>
    ///     Base adaptee class for yield objects that inherit from <see cref="CustomYieldInstruction" />.
    /// </summary>
    /// <typeparam name="T">Type of target yield object to adapt.</typeparam>
    public abstract class CustomYieldInstructionYield<T> : EmptyYield
        where T : CustomYieldInstruction
    {
        /// <summary>
        ///     See <see cref="CustomYieldInstruction.keepWaiting" />.
        /// </summary>
        public override bool IsDone { get { return !YieldObject.keepWaiting; } }

        /// <summary>
        ///     Adapt target yield object.
        /// </summary>
        public readonly T YieldObject;

        /// <summary>
        ///     Initialize a new instance of adaptee class for <typeparamref name="T" />.
        /// </summary>
        protected CustomYieldInstructionYield(T yieldObject) { YieldObject = yieldObject; }
    }

#if UNITY_2018_3_OR_NEWER
// No code here since WWW is obsolete start from 2018.3
#elif UNITY_2017_1_OR_NEWER
    /// <summary>
    ///     Adaptee for <see cref="WWW" />.
    /// </summary>
    public sealed class WaitWWW : CustomYieldInstructionYield<WWW>
    {
        /// <summary>
        ///     See <see cref="WWW" />.
        /// </summary>
        /// <param name="yieldObject">Actual <see cref="WWW" /> object.</param>
        public WaitWWW(WWW yieldObject) : base(yieldObject) { }
    }
#else
    /// <summary>
    ///     Adaptee for <see cref="WWW" />.
    /// </summary>
    public sealed class WaitWWW : EmptyYield
    {
        /// <summary>
        ///     See <see cref="UnityEngine.WWW.isDone" />.
        /// </summary>
        public override bool IsDone { return WWW.isDone; }

        /// <summary>
        ///     Adapt target yield object.
        /// </summary>
        public readonly WWW WWW;

        /// <summary>
        ///     See <see cref="WWW" />.
        /// </summary>
        /// <param name="www">Actual <see cref="WWW" /> object.</param>
        public WaitWWW(WWW www) { WWW = www; }
    }
#endif

    /// <summary>
    ///     Adaptee for <see cref="UnityEngine.AsyncOperation" />.
    /// </summary>
    public sealed class WaitAsyncOperation : EmptyYield
    {
        /// <summary>
        ///     See <see cref="UnityEngine.AsyncOperation.isDone" />.
        /// </summary>
        public override bool IsDone { get { return AsyncOperation.isDone; } }

        /// <summary>
        ///     Adapt target yield object.
        /// </summary>
        public readonly AsyncOperation AsyncOperation;

        /// <summary>
        ///     See <see cref="UnityEngine.AsyncOperation" />
        /// </summary>
        /// <param name="asyncOperation">Actual <see cref="UnityEngine.AsyncOperation" /> object.</param>
        public WaitAsyncOperation(AsyncOperation asyncOperation) { AsyncOperation = asyncOperation; }
    }
}