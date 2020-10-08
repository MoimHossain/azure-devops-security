

using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Waddle.Dtos
{
    
    public partial class VstsIdentityCollection
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("value")]
        public VstsIdentity[] Value { get; set; }
    }

    public partial class VstsIdentity
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("descriptor")]
        public string Descriptor { get; set; }

        [JsonProperty("subjectDescriptor")]
        public string SubjectDescriptor { get; set; }

        [JsonProperty("providerDisplayName")]
        public string ProviderDisplayName { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("isContainer")]
        public bool IsContainer { get; set; }

        [JsonProperty("members")]
        public object[] Members { get; set; }

        [JsonProperty("memberOf")]
        public object[] MemberOf { get; set; }

        [JsonProperty("memberIds")]
        public object[] MemberIds { get; set; }

        [JsonProperty("properties")]
        public Dictionary<string, VstsIdentityProperty> Properties { get; set; }

        [JsonProperty("resourceVersion")]
        public long ResourceVersion { get; set; }

        [JsonProperty("metaTypeId")]
        public long MetaTypeId { get; set; }
    }

    public partial class VstsIdentityProperty
    {
        [JsonProperty("$type")]
        public TypeEnum Type { get; set; }

        [JsonProperty("$value")]
        public string Value { get; set; }
    }

    public enum TypeEnum { SystemGuid, SystemString };


    internal class TypeEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TypeEnum) || t == typeof(TypeEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "System.Guid":
                    return TypeEnum.SystemGuid;
                case "System.String":
                    return TypeEnum.SystemString;
            }
            throw new Exception("Cannot unmarshal type TypeEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (TypeEnum)untypedValue;
            switch (value)
            {
                case TypeEnum.SystemGuid:
                    serializer.Serialize(writer, "System.Guid");
                    return;
                case TypeEnum.SystemString:
                    serializer.Serialize(writer, "System.String");
                    return;
            }
            throw new Exception("Cannot marshal type TypeEnum");
        }

        public static readonly TypeEnumConverter Singleton = new TypeEnumConverter();
    }
}
