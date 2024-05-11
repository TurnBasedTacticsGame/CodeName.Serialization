using System;
using System.Reflection;

namespace CodeName.Serialization.Snapshotting
{
    public class SnapshotRequiredException : Exception
    {
        public SnapshotRequiredException(Type snapshottedType, MemberInfo serializedMember)
            : base($"{snapshottedType.Name} is marked with the [{typeof(SnapshottableAttribute).Name}] attribute," +
                $"but the serialized member {serializedMember.Name} does not have the [{typeof(SnapshotAttribute).Name}] attribute applied." +
                $"The [{typeof(SnapshotAttribute).Name}] attribute is required to ensure the type is not accidentally serialized by value.") {}
    }
}
