using System;
using UnityEngine;

namespace ShuHai.Unity.CoroutineAnywhere
{
    /// <summary>
    ///     Base class for common yield type. Saves time for many derived classes that don't require
    ///     <see cref="IYield.Start" />, <see cref="IYield.Start" /> or <see cref="IYield.Update" />.
    /// </summary>
    public abstract class EmptyYield : IYield
    {
        /// <inheritdoc />
        public abstract bool IsDone { get; }

        void IYield.Start() { }
        void IYield.Stop() { }
        void IYield.Update() { }
    }

    /// <summary>
    ///     Suspends the coroutine execution for the given amount of seconds.
    /// </summary>
    public sealed class WaitSeconds : IYield
    {
        #region Time

        /// <summary>
        ///     How long the coroutine is suspended.
        /// </summary>
        public readonly float Duration;

        /// <summary>
        ///     When the current instance starts suspending the coroutine.
        /// </summary>
        public float StartTime { get; private set; }

        /// <summary>
        ///     When the current instance stops suspending the coroutine.
        /// </summary>
        public float StopTime { get; private set; }

        /// <summary>
        ///     Time elasped since the current instance starts suspending the coroutine.
        /// </summary>
        public float ElapsedTime { get { return CurrentTime - StartTime; } }

        /// <summary>
        ///     Current time.
        /// </summary>
        public float CurrentTime { get { return (TimeSource ?? DefaultTimeSource)(); } }

        /// <summary>
        ///     From where <see cref="CurrentTime" /> is fetched.
        /// </summary>
        public Func<float> TimeSource = DefaultTimeSource;

        /// <summary>
        ///     Time source used when <see cref="TimeSource" /> is not set.
        /// </summary>
        public static float DefaultTimeSource() { return Time.realtimeSinceStartup; }

        #endregion Time

        /// <summary>
        ///     Indicates whether the waiting is ended.
        /// </summary>
        public bool IsDone { get; private set; }

        /// <summary>
        ///     Suspends the coroutine execution for the given amount of seconds.
        /// </summary>
        /// <param name="duration">How long the suspending lasts.</param>
        public WaitSeconds(float duration) { Duration = duration; }

        void IYield.Start()
        {
            StartTime = CurrentTime;
            StopTime = StartTime + Duration;
        }

        void IYield.Stop() { IsDone = true; }

        void IYield.Update()
        {
            if (ElapsedTime > Duration)
                IsDone = true;
        }
    }

    /// <summary>
    ///     Suspends the coroutine execution until the supplied delegate evaluates to <see langword="false" />.
    /// </summary>
    public sealed class WaitWhile : EmptyYield
    {
        /// <inheritdoc />
        public override bool IsDone { get { return !Predicate(); } }

        /// <summary>
        ///     The delegate evaluated per frame.
        /// </summary>
        public readonly Func<bool> Predicate;

        /// <summary>
        ///     Suspends the coroutine execution until the supplied delegate evaluates to <see langword="false" />.
        /// </summary>
        public WaitWhile(Func<bool> predicate) { Predicate = predicate; }
    }

    /// <summary>
    ///     Suspends the coroutine execution until the supplied delegate evaluates to <see langword="true" />.
    /// </summary>
    public sealed class WaitUntil : EmptyYield
    {
        /// <inheritdoc />
        public override bool IsDone { get { return Predicate(); } }

        /// <summary>
        ///     The delegate evaluated per frame.
        /// </summary>
        public readonly Func<bool> Predicate;

        /// <summary>
        ///     Suspends the coroutine execution until the supplied delegate evaluates to <see langword="true" />.
        /// </summary>
        public WaitUntil(Func<bool> predicate) { Predicate = predicate; }
    }
}