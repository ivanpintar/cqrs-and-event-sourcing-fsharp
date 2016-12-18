using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace PinetreeCQRS.Persistence
{
    public class PrivateSetterResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            if (!prop.Writable)
            {
                var property = member as PropertyInfo;
                if (property != null)
                {
                    var hasPrivateSetter = property.GetSetMethod(true) != null;
                    prop.Writable = hasPrivateSetter;
                }
            }

            return prop;
        }
    }

    public class JsonConversionSettings
    {
        private static JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            ContractResolver = new PrivateSetterResolver()
        };

        public static JsonSerializerSettings SerializerSettings { get { return _settings; } }
    }
}
