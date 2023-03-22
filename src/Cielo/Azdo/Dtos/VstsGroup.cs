using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cielo.Azdo.Dtos
{
    #region User and Groups
    public class VstsMembershipCollection
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("value")]
        public VstsMembershipElement[] Members { get; set; }
    }

    public class VstsMembershipElement
    {
        [JsonProperty("containerDescriptor")]
        public string ContainerDescriptor { get; set; }

        [JsonProperty("memberDescriptor")]
        public string MemberDescriptor { get; set; }
    }

    public class IdentityInternalCollectionDto
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("value")]
        public List<IdentityInternalDto> Value { get; set; }
    }
    public class IdentityInternalDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

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
    }
    public partial class VstsProjectDescriptor
    {
        [JsonProperty("value")]
        public string ScopeDescriptor { get; set; }
    }

    public partial class VstsUser
    {
        [JsonProperty("subjectKind")]
        public SubjectKind SubjectKind { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }

        [JsonProperty("principalName")]
        public string PrincipalName { get; set; }

        [JsonProperty("mailAddress")]
        public object MailAddress { get; set; }

        [JsonProperty("origin")]
        public IdentityOrigin Origin { get; set; }

        [JsonProperty("originId")]
        public Guid OriginId { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }


        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("uniqueName")]
        public string UniqueName { get; set; }

        [JsonProperty("imageUrl")]
        public Uri ImageUrl { get; set; }

        [JsonProperty("descriptor")]
        public string Descriptor { get; set; }
    }
    public partial class GroupCollection
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("value")]
        public VstsGroup[] Value { get; set; }
    }

    public partial class VstsGroup
    {
        [JsonProperty("subjectKind")]
        public SubjectKind SubjectKind { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }

        [JsonProperty("principalName")]
        public string PrincipalName { get; set; }

        [JsonProperty("mailAddress")]
        public object MailAddress { get; set; }

        [JsonProperty("origin")]
        public IdentityOrigin Origin { get; set; }

        [JsonProperty("originId")]
        public Guid OriginId { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("_links")]
        public GroupLinks Links { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("descriptor")]
        public string Descriptor { get; set; }


        public bool CrossProject
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Domain) && Domain.StartsWith("vstfs:///Framework/Generic/");
            }
        }

        [JsonIgnore]
        public string Sid
        {
            get
            {
                return IdentityBase64Supports.GetSid(this.Descriptor);
            }
        }

        public override string ToString()
        {
            return $"{(CrossProject ? "ORG" : "PROJ")}:: {Origin}:: {PrincipalName}, [{SubjectKind}]";
        }
    }

    public partial class GroupLinks
    {
        [JsonProperty("self")]
        public MembershipState Self { get; set; }

        [JsonProperty("memberships")]
        public MembershipState Memberships { get; set; }

        [JsonProperty("membershipState")]
        public MembershipState MembershipState { get; set; }

        [JsonProperty("storageKey")]
        public MembershipState StorageKey { get; set; }
    }

    public partial class MembershipState
    {
        [JsonProperty("href")]
        public Uri Href { get; set; }
    }

    public enum IdentityOrigin { Aad, Vsts };

    public enum SubjectKind { Group, User };

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                OriginConverter.Singleton,
                SubjectKindConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class OriginConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(IdentityOrigin) || t == typeof(IdentityOrigin?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "aad":
                    return IdentityOrigin.Aad;
                case "vsts":
                    return IdentityOrigin.Vsts;
            }
            throw new Exception("Cannot unmarshal type Origin");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (IdentityOrigin)untypedValue;
            switch (value)
            {
                case IdentityOrigin.Aad:
                    serializer.Serialize(writer, "aad");
                    return;
                case IdentityOrigin.Vsts:
                    serializer.Serialize(writer, "vsts");
                    return;
            }
            throw new Exception("Cannot marshal type Origin");
        }

        public static readonly OriginConverter Singleton = new OriginConverter();
    }

    internal class SubjectKindConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(SubjectKind) || t == typeof(SubjectKind?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "group")
            {
                return SubjectKind.Group;
            }
            throw new Exception("Cannot unmarshal type SubjectKind");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (SubjectKind)untypedValue;
            if (value == SubjectKind.Group)
            {
                serializer.Serialize(writer, "group");
                return;
            }
            throw new Exception("Cannot marshal type SubjectKind");
        }

        public static readonly SubjectKindConverter Singleton = new SubjectKindConverter();
    }

    public class IdentityBase64Supports
    {
        public static string GetSid(string descriptor)
        {
            if (!string.IsNullOrWhiteSpace(descriptor))
            {
                var b64s = descriptor.Substring(descriptor.IndexOf(".") + 1);
                var raw = DecodeUrlBase64(b64s);
                return $"Microsoft.TeamFoundation.Identity;{raw}";
            }
            return string.Empty;
        }

        public static string DecodeUrlBase64(string s)
        {
            s = s.Replace('-', '+').Replace('_', '/').PadRight(4 * ((s.Length + 3) / 4), '=');
            return ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(s));
        }
    }
    #endregion
}
