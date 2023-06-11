using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.UnityConverters.Math;
using Newtonsoft.Json.UnityConverters.Scripting;
using UniDi;

namespace Project.Source.Serialization
{
    public class JsonNetInstaller : ScriptableObjectInstaller
    {
        private const string ApplySerializerSettingsMethodName = "ApplySerializerSettings";

        public override void InstallBindings()
        {
            BindConverters();

            Container.Bind<IContractResolver>().To<ProjectJsonContractResolver>()
                .AsSingle()
                .CopyIntoAllSubContainers();

            Container.Bind<JsonSerializerSettings>().FromMethod(CreateJsonSerializerSettings).AsSingle()
                .CopyIntoAllSubContainers();

            Container.BindInterfacesAndSelfTo<ProjectJsonSerializer>()
                .AsSingle()
                .OnInstantiated<JsonSerializer>(ApplySerializerSettings)
                .CopyIntoAllSubContainers();
        }

        private void BindConverters()
        {
            // Default
            BindConverter<VersionConverter>();
            BindConverter<StringEnumConverter>();

            // Unity - Math
            BindConverter<Color32Converter>();
            BindConverter<ColorConverter>();

            BindConverter<Matrix4x4Converter>();
            BindConverter<QuaternionConverter>();

            BindConverter<Vector2Converter>();
            BindConverter<Vector2IntConverter>();

            BindConverter<Vector3Converter>();
            BindConverter<Vector3IntConverter>();

            BindConverter<Vector4Converter>();

            // Unity - Scripting
            BindConverter<RangeIntConverter>();
            BindConverter<LayerMaskConverter>();

            // Custom
            BindConverter<ReactivePropertyConverter>();
        }

        private void BindConverter<T>() where T : JsonConverter
        {
            Container.Bind<JsonConverter>().To<T>().AsSingle();
        }

        private JsonSerializerSettings CreateJsonSerializerSettings(InjectContext context)
        {
            return new JsonSerializerSettings
            {
                Converters = context.Container.Resolve<IList<JsonConverter>>(),
                ContractResolver = context.Container.Resolve<IContractResolver>(),
            };
        }

        private void ApplySerializerSettings(InjectContext context, JsonSerializer serializer)
        {
            var settings = context.Container.Resolve<JsonSerializerSettings>();
            var flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            var applySettingsMethodInfo = typeof(JsonSerializer).GetMethod(ApplySerializerSettingsMethodName, flags)!;

            applySettingsMethodInfo.Invoke(null, new object[] { serializer, settings });
        }
    }
}
