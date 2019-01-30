using UnityEngine;

namespace ShuHai.Unity.Coroutines
{
    public abstract class CustomYieldInstructionYield<T> : EmptyYield
        where T : CustomYieldInstruction
    {
        public override bool IsDone => !YieldObject.keepWaiting;

        public readonly T YieldObject;

        protected CustomYieldInstructionYield(T yieldObject) { YieldObject = yieldObject; }
    }

#if UNITY_2018_3_OR_NEWER
// No code here since WWW is obsolete start from 2018.3
#elif UNITY_2017_1_OR_NEWER
    public sealed class WaitWWW : CustomYieldInstructionYield<WWW>
    {
        public WaitWWW(WWW yieldObject) : base(yieldObject) { }
    }
#else
    public sealed class WaitWWW : EmptyYield
    {
        public override bool IsDone => WWW.isDone;

        public readonly WWW WWW;

        public WaitWWW(WWW www) { WWW = www; }
    }
#endif

    public sealed class WaitAsyncOperation : EmptyYield
    {
        public override bool IsDone => AsyncOperation.isDone;

        public readonly AsyncOperation AsyncOperation;

        public WaitAsyncOperation(AsyncOperation asyncOperation) { AsyncOperation = asyncOperation; }
    }
}