using System;
using System.Reflection;

namespace CodeName.Serialization.Snapshotting
{
    public class SnapshotRequiredException : Exception
    {
        public SnapshotRequiredException(Type snapshottedType, MemberInfo serializedMember)
            : base($"{snapshottedType.Name} is marked with the [{typeof(SnapshottableAttribute).Name}] attribute, " +
                $"but the serialized member {serializedMember.DeclaringType}.{serializedMember.Name} does not have the [{typeof(SerializeSnapshotAttribute).Name}] attribute applied. " +
                $"The [{typeof(SerializeSnapshotAttribute).Name}] attribute is required to ensure the type is not accidentally serialized by value.") {}
    }
}
