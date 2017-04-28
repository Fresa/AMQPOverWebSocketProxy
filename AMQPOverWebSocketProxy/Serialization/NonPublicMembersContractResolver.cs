using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AMQPOverWebSocketProxy.Serialization
{
    public class NonPublicMembersContractResolver : DefaultContractResolver
    {
        private readonly Dictionary<Type, JsonConverter> _converters = new Dictionary<Type, JsonConverter>();
        private readonly List<Type> _excluded = new List<Type>();

        public NonPublicMembersContractResolver RegisterConverter<T>(JsonConverter converter)
        {
            _converters.Add(typeof(T), converter);
            return this;
        }

        public NonPublicMembersContractResolver Exclude(params Type[] types)
        {
            _excluded.AddRange(types);
            return this;
        }

        public bool CamelCase { get; set; }

        protected override JsonContract CreateContract(Type objectType)
        {
            var contract = base.CreateContract(objectType);
            if (_converters.ContainsKey(objectType))
            {
                contract.Converter = _converters[objectType];
            }
            return contract;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            if (_excluded.Contains(type))
            {
                return base.CreateProperties(type, memberSerialization);
            }

            var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Select(propertyInfo =>
                {
                    var property = CreateProperty(propertyInfo, memberSerialization);
                    property.PropertyName = ResolveName(property.PropertyName);
                    property.Readable = propertyInfo.CanRead;
                    property.Writable = propertyInfo.CanWrite;
                    return property;
                }).ToList();

            return props;
        }

        private string ResolveName(string propertyName)
        {
            if (CamelCase)
            {
                return char.ToLower(propertyName[0]) + string.Join("", propertyName.Skip(1));
            }
            return propertyName;
        }
    }
}