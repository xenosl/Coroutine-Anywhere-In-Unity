using System;
using System.Reflection;

namespace ShuHai.Unity.CoroutineAnywhere
{
    internal static class Extensions
    {
        #region Reflection

        public static T GetCustomAttribute<T>(this MemberInfo self) where T : Attribute
            { return (T)GetCustomAttribute(self, typeof(T)); }

        public static Attribute GetCustomAttribute(this MemberInfo self, Type type)
            { return Attribute.GetCustomAttribute(self, type); }

        #endregion Reflection
    }
}