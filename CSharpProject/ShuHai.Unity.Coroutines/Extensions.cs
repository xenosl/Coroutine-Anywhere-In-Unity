using System;
using System.Reflection;

namespace ShuHai.Unity.Coroutines
{
    internal static class Extensions
    {
        #region Reflection

        public static T GetCustomAttribute<T>(this MemberInfo self) where T : Attribute
            => (T)GetCustomAttribute(self, typeof(T));

        public static Attribute GetCustomAttribute(this MemberInfo self, Type type)
            => Attribute.GetCustomAttribute(self, type);

        #endregion Reflection
    }
}