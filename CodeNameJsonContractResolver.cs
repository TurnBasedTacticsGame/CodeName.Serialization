using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeName.Serialization.Snapshotting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.UnityConverters;

namespace CodeName.Serialization
{
    public class CodeNameJsonContractResolver : UnityTypeContractResolver
    {
        private Dictionary<Type, bool> isTypeSnapshottableCache = new();

        protected override JsonProperty CreateProperty(MemberInfo memberInfo, MemberSerialization memberSerialization)
        {
            var jsonProperty = base.CreateProperty(memberInfo, memberSerialization);

            // From https://stackoverflow.com/questions/70608545/json-net-ignore-serialized-private-fields-in-unity
            // This makes it so that members with [JsonIgnore] applied are properly ignored, even with [SerializeField] applied
            if (!jsonProperty.Ignored && memberInfo.GetCustomAttribute<JsonIgnoreAttribute>() != null)
            {
                jsonProperty.Ignored = true;
            }

            if (IsMemberTypeSnapshottable(memberInfo, out var memberType))
            {
                var isSnapshot = memberInfo.GetCustomAttribute<SerializeSnapshotAttribute>() != null;
                if (!isSnapshot)
                {
                    throw new SnapshotRequiredException(memberType, memberInfo);
                }
            }

            return jsonProperty;
        }

        private bool IsMemberTypeSnapshottable(MemberInfo memberInfo, out Type memberType)
        {
            memberType = null;
            return (memberInfo is PropertyInfo propertyInfo && IsTypeSnapshottable(propertyInfo.PropertyType, out memberType))
                || (memberInfo is FieldInfo fieldInfo && IsTypeSnapshottable(fieldInfo.FieldType, out memberType));
        }

        private bool IsTypeSnapshottable(Type type, out Type narrowedType)
        {
            var originalType = type;
            narrowedType = type;

            if (isTypeSnapshottableCache.TryGetValue(narrowedType, out var isSnapshottable))
            {
                return isSnapshottable;
            }

            var enumerableInterface = narrowedType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            if (enumerableInterface != null)
            {
                narrowedType = enumerableInterface.GetGenericArguments()[0];
            }

            isSnapshottable = narrowedType.GetCustomAttribute<SnapshottableAttribute>() != null;
            isTypeSnapshottableCache[originalType] = isSnapshottable;

            return isSnapshottable;
        }
    }
}
