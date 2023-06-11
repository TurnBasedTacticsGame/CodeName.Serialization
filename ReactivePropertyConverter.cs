#nullable enable
using System;
using System.Reflection;
using Newtonsoft.Json;
using UniRx;

namespace CodeName.Serialization
{
    public class ReactivePropertyConverter : JsonConverter
    {
        private const string InnerValuePropertyName = "Value";

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                serializer.Serialize(writer, null);

                return;
            }

            var innerValuePropertyInfo = GetInnerValuePropertyInfo(value.GetType());
            var innerValue = innerValuePropertyInfo.GetValue(value);

            serializer.Serialize(writer, innerValue);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (existingValue == null)
            {
                existingValue = Activator.CreateInstance(objectType);
            }

            var innerValuePropertyInfo = GetInnerValuePropertyInfo(existingValue.GetType());
            var innerValuePropertyType = innerValuePropertyInfo.PropertyType;
            innerValuePropertyInfo.SetValue(existingValue, serializer.Deserialize(reader, innerValuePropertyType));

            return existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            return IsGenericAssignableFrom(objectType, typeof(ReactiveProperty<>));
        }

        public PropertyInfo GetInnerValuePropertyInfo(Type type)
        {
            return type.GetProperty(InnerValuePropertyName) ?? throw new InvalidOperationException();
        }

        // From https://stackoverflow.com/questions/5461295/using-isassignablefrom-with-open-generic-types
        public bool IsGenericAssignableFrom(Type? extendType, Type baseType)
        {
            while (!baseType.IsAssignableFrom(extendType))
            {
                if (extendType == null || extendType.Equals(typeof(object)))
                {
                    return false;
                }

                if (extendType.IsGenericType && !extendType.IsGenericTypeDefinition)
                {
                    extendType = extendType.GetGenericTypeDefinition();
                }
                else
                {
                    extendType = extendType.BaseType;
                }
            }

            return true;
        }
    }
}