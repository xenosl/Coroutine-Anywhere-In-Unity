using System;
using UnityEngine;

namespace ShuHai.Unity.Coroutines
{
    [TargetTypes(typeof(WaitForSeconds))]
    internal class WaitForSecondsAdapter : IYieldAdapter
    {
        public IYield ToYield(object yieldObject)
        {
            if (!(yieldObject is WaitForSeconds obj))
                throw new ArgumentException("Invalid type of yield object.", nameof(yieldObject));

            var seconds = ReflectionHelper.GetFieldFloat(typeof(WaitForSeconds), "m_Seconds", obj);
            return new WaitSeconds(seconds) { TimeSource = () => Time.time };
        }
    }

#if UNITY_2017_1_OR_NEWER
    [TargetTypes(typeof(WaitForSecondsRealtime))]
    internal class WaitForSecondsRealtimeAdapter : IYieldAdapter
    {
        public IYield ToYield(object yieldObject)
        {
            if (!(yieldObject is WaitForSecondsRealtime obj))
                throw new ArgumentException("Invalid type of yield object.", nameof(yieldObject));

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
    [TargetType(typeof(WWW))]
    internal class WWWAdapter : IYieldAdapter
    {
        public IYield ToYield(object yieldObject) { return new WaitWWW((WWW)yieldObject); }
    }
#endif

    [TargetType(typeof(AsyncOperation))]
    internal class AsyncOperationAdapter : IYieldAdapter
    {
        public IYield ToYield(object yieldObject) { return new WaitAsyncOperation((AsyncOperation)yieldObject); }
    }

    [TargetType(typeof(UnityEngine.WaitWhile))]
    internal class WaitWhileAdapter : IYieldAdapter
    {
        public IYield ToYield(object yieldObject)
        {
            return new WaitWhile(ReflectionHelper.GetFieldValue<Func<bool>>(
                typeof(UnityEngine.WaitWhile), "m_Predicate", yieldObject));
        }
    }

    [TargetType(typeof(UnityEngine.WaitUntil))]
    internal class WaitUntilAdapter : IYieldAdapter
    {
        public IYield ToYield(object yieldObject)
        {
            return new WaitUntil(ReflectionHelper.GetFieldValue<Func<bool>>(
                typeof(UnityEngine.WaitUntil), "m_Predicate", yieldObject));
        }
    }
}