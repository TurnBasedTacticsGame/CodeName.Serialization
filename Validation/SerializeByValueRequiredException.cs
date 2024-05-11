using System;
using System.Reflection;

namespace CodeName.Serialization.Validation
{
    public class SerializeByValueRequiredException : Exception
    {
        public SerializeByValueRequiredException(Type serializedType, MemberInfo serializedMember)
            : base($"{serializedType.Name} is marked with the [{typeof(ValidateSerializeByValueAttribute).Name}] attribute, " +
                $"but the serialized member {serializedMember.DeclaringType}.{serializedMember.Name} does not have the [{typeof(SerializeByValueAttribute).Name}] attribute applied. " +
                $"The [{typeof(SerializeByValueAttribute).Name}] attribute is required to ensure the type is not accidentally serialized by value.") {}
    }
}
