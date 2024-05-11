using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.UnityConverters;

namespace CodeName.Serialization
{
    public class CodeNameJsonContractResolver : UnityTypeContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var jsonProperty = base.CreateProperty(member, memberSerialization);

            // From https://stackoverflow.com/questions/70608545/json-net-ignore-serialized-private-fields-in-unity
            // This makes it so that members with [JsonIgnore] applied are properly ignored, even with [SerializeField] applied
            if (!jsonProperty.Ignored && member.GetCustomAttribute<JsonIgnoreAttribute>() != null)
            {
                jsonProperty.Ignored = true;
            }

            return jsonProperty;
        }
    }
}
