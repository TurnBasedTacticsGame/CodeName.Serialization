using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeName.Serialization.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.UnityConverters;

namespace CodeName.Serialization
{
    public class CodeNameJsonContractResolver : UnityTypeContractResolver
    {
        private Dictionary<Type, bool> isTypeValidateSerializeByValue = new();

        protected override JsonProperty CreateProperty(MemberInfo memberInfo, MemberSerialization memberSerialization)
        {
            var jsonProperty = base.CreateProperty(memberInfo, memberSerialization);

            // From https://stackoverflow.com/questions/70608545/json-net-ignore-serialized-private-fields-in-unity
            // This makes it so that members with [JsonIgnore] applied are properly ignored, even with [SerializeField] applied
            if (!jsonProperty.Ignored && memberInfo.GetCustomAttribute<JsonIgnoreAttribute>() != null)
            {
                jsonProperty.Ignored = true;
            }

            if (!jsonProperty.Ignored && IsMemberTypeValidateSerializeByValue(memberInfo, out var memberType))
            {
                var isValidate = memberInfo.GetCustomAttribute<SerializeByValueAttribute>() != null;
                if (!isValidate)
                {
                    throw new SerializeByValueRequiredException(memberType, memberInfo);
                }
            }

            return jsonProperty;
        }

        private bool IsMemberTypeValidateSerializeByValue(MemberInfo memberInfo, out Type memberType)
        {
            memberType = null;
            return (memberInfo is PropertyInfo propertyInfo && IsTypeValidateSerializeByValue(propertyInfo.PropertyType, out memberType))
                || (memberInfo is FieldInfo fieldInfo && IsTypeValidateSerializeByValue(fieldInfo.FieldType, out memberType));
        }

        private bool IsTypeValidateSerializeByValue(Type type, out Type narrowedType)
        {
            var originalType = type;
            narrowedType = type;

            if (isTypeValidateSerializeByValue.TryGetValue(narrowedType, out var isValidate))
            {
                return isValidate;
            }

            var enumerableInterface = narrowedType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            if (enumerableInterface != null)
            {
                narrowedType = enumerableInterface.GetGenericArguments()[0];
            }

            isValidate = narrowedType.GetCustomAttribute<ValidateSerializeByValueAttribute>() != null;
            isTypeValidateSerializeByValue[originalType] = isValidate;

            return isValidate;
        }
    }
}
