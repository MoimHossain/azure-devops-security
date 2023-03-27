using Newtonsoft.Json;

namespace Cielo.Azdo.Dtos
{
    public class VstsUserEntitlementInfo
    {
        [JsonProperty("user", NullValueHandling = NullValueHandling.Ignore)]
        public VstsUserInfo User { get; set; }
    }

    public class VstsUserEntitlementCollection
    {
        [JsonProperty("items", NullValueHandling = NullValueHandling.Ignore)]
        public List<VstsUserEntitlementInfo> Items { get; set; }
    }

    public class VstsUserInfo
    {
        [JsonProperty("subjectKind", NullValueHandling = NullValueHandling.Ignore)]
        public string SubjectKind { get; set; }

        [JsonProperty("metaType", NullValueHandling = NullValueHandling.Ignore)]
        public string MetaType { get; set; }

        [JsonProperty("directoryAlias", NullValueHandling = NullValueHandling.Ignore)]
        public string DirectoryAlias { get; set; }

        [JsonProperty("domain", NullValueHandling = NullValueHandling.Ignore)]
        public string Domain { get; set; }

        [JsonProperty("principalName", NullValueHandling = NullValueHandling.Ignore)]
        public string PrincipalName { get; set; }

        [JsonProperty("mailAddress", NullValueHandling = NullValueHandling.Ignore)]
        public string MailAddress { get; set; }

        [JsonProperty("origin", NullValueHandling = NullValueHandling.Ignore)]
        public string Origin { get; set; }

        [JsonProperty("originId", NullValueHandling = NullValueHandling.Ignore)]
        public string OriginId { get; set; }

        [JsonProperty("displayName", NullValueHandling = NullValueHandling.Ignore)]
        public string DisplayName { get; set; }

        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }

        [JsonProperty("descriptor", NullValueHandling = NullValueHandling.Ignore)]
        public string Descriptor { get; set; }

        [JsonIgnore]
        public string Sid
        {
            get
            {
                return $"Microsoft.IdentityModel.Claims.ClaimsIdentity;{this.Domain}\\{this.PrincipalName}";
            }
        }
    }


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

        [JsonIgnore]
        public string Sid
        {
            get
            {
                return IdentityBase64Supports.GetSid(this.Descriptor);
            }
        }
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
