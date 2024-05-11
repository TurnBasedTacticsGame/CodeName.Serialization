using System;
using System.Reflection;
using CodeName.Serialization.Snapshotting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.UnityConverters;

namespace CodeName.Serialization
{
    public class CodeNameJsonContractResolver : UnityTypeContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo memberInfo, MemberSerialization memberSerialization)
        {
            var jsonProperty = base.CreateProperty(memberInfo, memberSerialization);

            // From https://stackoverflow.com/questions/70608545/json-net-ignore-serialized-private-fields-in-unity
            // This makes it so that members with [JsonIgnore] applied are properly ignored, even with [SerializeField] applied
            if (!jsonProperty.Ignored && memberInfo.GetCustomAttribute<JsonIgnoreAttribute>() != null)
            {
                jsonProperty.Ignored = true;
            }

            if (IsSnapshottable(memberInfo, out var memberType))
            {
                var isSnapshot = memberInfo.GetCustomAttribute<SerializeSnapshotAttribute>() != null;
                if (!isSnapshot)
                {
                    throw new SnapshotRequiredException(memberType, memberInfo);
                }
            }

            return jsonProperty;
        }

        private bool IsSnapshottable(MemberInfo memberInfo, out Type memberType)
        {
            if (memberInfo is PropertyInfo propertyInfo && propertyInfo.PropertyType.GetCustomAttribute<SnapshottableAttribute>() != null)
            {
                memberType = propertyInfo.PropertyType;
                return true;
            }

            if (memberInfo is FieldInfo fieldInfo && fieldInfo.FieldType.GetCustomAttribute<SnapshottableAttribute>() != null)
            {
                memberType = fieldInfo.FieldType;
                return true;
            }

            memberType = null;
            return false;
        }
    }
}
