using System;

namespace CodeName.Serialization.Snapshotting
{
    /// <summary>
    /// Used to prevent accidentally serializing objects by value.
    /// If a property or field containing the object is serialized, the property or field must have the <see cref="SnapshotAttribute"/> applied.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SnapshottableAttribute : Attribute {}
}
