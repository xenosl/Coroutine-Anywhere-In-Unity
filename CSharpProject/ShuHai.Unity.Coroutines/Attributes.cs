using System;

namespace ShuHai.Unity.Coroutines
{
    public class TargetTypeAttribute : Attribute
    {
        public Type Value;

        public TargetTypeAttribute(Type value) { Value = value; }
    }

    public class TargetTypesAttribute : Attribute
    {
        public Type[] Values;

        public TargetTypesAttribute(params Type[] values) { Values = values; }
    }
}