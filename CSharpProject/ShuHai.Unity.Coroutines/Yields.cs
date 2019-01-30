using System;
using UnityEngine;

namespace ShuHai.Unity.Coroutines
{
    public abstract class EmptyYield : IYield
    {
        public abstract bool IsDone { get; }

        void IYield.Start() { }
        void IYield.Stop() { }
        void IYield.Update() { }
    }

    public sealed class WaitSeconds : IYield
    {
        #region Time

        public readonly float Duration;

        public float StartTime { get; private set; }
        public float StopTime { get; private set; }
        public float ElapsedTime { get { return CurrentTime - StartTime; } }

        public float CurrentTime { get { return TimeSource(); } }

        public Func<float> TimeSource = DefaultTimeSource;

        public static float DefaultTimeSource() { return Time.realtimeSinceStartup; }

        #endregion Time

        public bool IsDone { get; private set; }

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

    public sealed class WaitWhile : EmptyYield
    {
        public override bool IsDone { get { return !Predicate(); } }

        public readonly Func<bool> Predicate;

        public WaitWhile(Func<bool> predicate) { Predicate = predicate; }
    }

    public sealed class WaitUntil : EmptyYield
    {
        public override bool IsDone { get { return Predicate(); } }

        public readonly Func<bool> Predicate;

        public WaitUntil(Func<bool> predicate) { Predicate = predicate; }
    }
}