using System;

namespace CodeName.Serialization.Validation
{
    /// <summary>
    /// Used to prevent accidentally serializing objects by value.
    /// If a property or field containing the object is serialized, the property or field must have the <see cref="SerializeByValueAttribute"/> applied.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ValidateSerializeByValueAttribute : Attribute {}
}
