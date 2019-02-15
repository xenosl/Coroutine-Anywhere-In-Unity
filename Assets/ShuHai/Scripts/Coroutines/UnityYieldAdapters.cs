using System;
using UnityEngine;

namespace ShuHai.Unity.CoroutineAnywhere
{
    [YieldAdapterTarget(typeof(WaitForSeconds))]
    internal class WaitForSecondsAdapter : IYieldAdapter
    {
        public IYield ToYield(object yieldObject)
        {
            var obj = yieldObject as WaitForSeconds;
            if (obj == null)
                throw new ArgumentException("Invalid type of yield object.", "yieldObject");

            var seconds = ReflectionHelper.GetFieldFloat(typeof(WaitForSeconds), "m_Seconds", obj);
            return new WaitSeconds(seconds) { TimeSource = () => Time.time };
        }
    }

#if UNITY_2017_1_OR_NEWER
    [YieldAdapterTarget(typeof(WaitForSecondsRealtime))]
    internal class WaitForSecondsRealtimeAdapter : IYieldAdapter
    {
        public IYield ToYield(object yieldObject)
        {
            var obj = yieldObject as WaitForSecondsRealtime;
            if (obj == null)
                throw new ArgumentException("Invalid type of yield object.", "yieldObject");

            var stopTime = ReflectionHelper.GetFieldFloat(typeof(WaitForSecondsRealtime), "waitTime", obj);
            return new WaitSeconds(stopTime - Time.realtimeSinceStartup)
            {
                TimeSource = () => Time.realtimeSinceStartup
            };
        }
    }
#endif

#if UNITY_2018_3_OR_NEWER
// No code here since WWW is obsolete start from 2018.3
#else
    [YieldAdapterTarget(typeof(WWW))]
    internal class WWWAdapter : IYieldAdapter
    {
        public IYield ToYield(object yieldObject) { return new WaitWWW((WWW)yieldObject); }
    }
#endif

    [YieldAdapterTarget(typeof(AsyncOperation))]
    internal class AsyncOperationAdapter : IYieldAdapter
    {
        public IYield ToYield(object yieldObject) { return new WaitAsyncOperation((AsyncOperation)yieldObject); }
    }

    [YieldAdapterTarget(typeof(UnityEngine.WaitWhile))]
    internal class WaitWhileAdapter : IYieldAdapter
    {
        public IYield ToYield(object yieldObject)
        {
            return new WaitWhile(ReflectionHelper.GetFieldValue<Func<bool>>(
                typeof(UnityEngine.WaitWhile), "m_Predicate", yieldObject));
        }
    }

    [YieldAdapterTarget(typeof(UnityEngine.WaitUntil))]
    internal class WaitUntilAdapter : IYieldAdapter
    {
        public IYield ToYield(object yieldObject)
        {
            return new WaitUntil(ReflectionHelper.GetFieldValue<Func<bool>>(
                typeof(UnityEngine.WaitUntil), "m_Predicate", yieldObject));
        }
    }
}