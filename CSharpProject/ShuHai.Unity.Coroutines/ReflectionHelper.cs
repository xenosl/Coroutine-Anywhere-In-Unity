using System;

namespace ShuHai.Unity.Coroutines
{
    internal static class ReflectionHelper
    {
        public static float GetFieldFloat(Type type, string name, object obj) => GetFieldValue<float>(type, name, obj);

        public static T GetFieldValue<T>(Type type, string name, object obj)
            => (T)type.GetField(name, false).GetValue(obj);
    }
}