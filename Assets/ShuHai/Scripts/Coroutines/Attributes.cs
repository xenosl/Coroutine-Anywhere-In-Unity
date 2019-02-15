using System;

namespace ShuHai.Unity.CoroutineAnywhere
{
    /// <summary>
    ///     Specifies the adapt target type of <see cref="IYieldAdapter" />.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class YieldAdapterTargetAttribute : Attribute
    {
        /// <summary>
        ///     Type of the adapt targets.
        /// </summary>
        public Type Type;

        /// <summary>
        ///     Initialize a new instance of <see cref="YieldAdapterTargetAttribute" /> with <see cref="Type" /> of adapt targets.
        /// </summary>
        /// <param name="type">Type of adapt targets.</param>
        public YieldAdapterTargetAttribute(Type type) { Type = type; }
    }

    /// <summary>
    ///     Specifies the adapt target types of <see cref="IYieldAdapter" />.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class YieldAdapterTargetsAttribute : Attribute
    {
        /// <summary>
        ///     Types of the adapt targets.
        /// </summary>
        public Type[] Types;

        /// <summary>
        ///     Initialize a new instance of <see cref="YieldAdapterTargetsAttribute" /> with <see cref="Type" />s of adapt
        ///     targets.
        /// </summary>
        /// <param name="types">Types of adapt targets.</param>
        public YieldAdapterTargetsAttribute(params Type[] types) { Types = types; }
    }
}