﻿

using Kdoctl.CliServices.Supports;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;


namespace Kdoctl.CliServices.AzDoServices.Dtos
{
    #region Environments
    public partial class PipelineEnvironmentCollection
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("value")]
        public PipelineEnvironment[] Value { get; set; }
    }

    public partial class PipelineEnvironment
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("createdBy")]
        public VstsUser CreatedBy { get; set; }

        [JsonProperty("createdOn")]
        public DateTimeOffset CreatedOn { get; set; }

        [JsonProperty("lastModifiedBy")]
        public VstsUser LastModifiedBy { get; set; }

        [JsonProperty("lastModifiedOn")]
        public DateTimeOffset LastModifiedOn { get; set; }

        [JsonProperty("project")]
        public Project Project { get; set; }
    }


    public partial class Project
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class ProjectCollection
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("value")]
        public Project[] Value { get; set; }
    }
    #endregion

    #region Endpoint
    public partial class EndpointCollection
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("value")]
        public Endpoint[] Value { get; set; }
    }

    public partial class Endpoint
    {
        public override string ToString()
        {
            return $"{Type} :: {Name} [{Id}]";
        }
        [JsonProperty("data")]
        public Data Data { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("createdBy")]
        public VstsFolderCreatedBy CreatedBy { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("authorization")]
        public Authorization Authorization { get; set; }

        [JsonProperty("isShared")]
        public bool IsShared { get; set; }

        [JsonProperty("isReady")]
        public bool IsReady { get; set; }

        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("serviceEndpointProjectReferences")]
        public ServiceEndpointProjectReference[] ServiceEndpointProjectReferences { get; set; }

        [JsonProperty("operationStatus", NullValueHandling = NullValueHandling.Ignore)]
        public OperationStatus OperationStatus { get; set; }
    }

    public partial class Authorization
    {
        [JsonProperty("scheme")]
        public string Scheme { get; set; }

        [JsonProperty("parameters", NullValueHandling = NullValueHandling.Ignore)]
        public Parameters Parameters { get; set; }
    }

    public partial class Parameters
    {
        [JsonProperty("registry", NullValueHandling = NullValueHandling.Ignore)]
        public Uri Registry { get; set; }

        [JsonProperty("username", NullValueHandling = NullValueHandling.Ignore)]
        public string Username { get; set; }

        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; set; }

        [JsonProperty("azureEnvironment", NullValueHandling = NullValueHandling.Ignore)]
        public string AzureEnvironment { get; set; }

        [JsonProperty("azureTenantId", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? AzureTenantId { get; set; }

        [JsonProperty("serviceAccountName", NullValueHandling = NullValueHandling.Ignore)]
        public string ServiceAccountName { get; set; }

        [JsonProperty("roleBindingName", NullValueHandling = NullValueHandling.Ignore)]
        public string RoleBindingName { get; set; }

        [JsonProperty("secretName", NullValueHandling = NullValueHandling.Ignore)]
        public string SecretName { get; set; }
    }
  

    public partial class Data
    {
        [JsonProperty("pipelinesSourceProvider", NullValueHandling = NullValueHandling.Ignore)]
        public string PipelinesSourceProvider { get; set; }

        [JsonProperty("registrytype", NullValueHandling = NullValueHandling.Ignore)]
        public string Registrytype { get; set; }

        [JsonProperty("subscriptionId", NullValueHandling = NullValueHandling.Ignore)]
        public string SubscriptionId { get; set; }

        [JsonProperty("subscriptionName", NullValueHandling = NullValueHandling.Ignore)]
        public string SubscriptionName { get; set; }

        [JsonProperty("registryId", NullValueHandling = NullValueHandling.Ignore)]
        public string RegistryId { get; set; }

        [JsonProperty("spnObjectId", NullValueHandling = NullValueHandling.Ignore)]
        public string SpnObjectId { get; set; }

        [JsonProperty("appObjectId", NullValueHandling = NullValueHandling.Ignore)]
        public string AppObjectId { get; set; }

        [JsonProperty("azureSpnRoleAssignmentId", NullValueHandling = NullValueHandling.Ignore)]
        public string AzureSpnRoleAssignmentId { get; set; }

        [JsonProperty("azureSpnPermissions", NullValueHandling = NullValueHandling.Ignore)]
        public string AzureSpnPermissions { get; set; }

        [JsonProperty("authorizationType", NullValueHandling = NullValueHandling.Ignore)]
        public string AuthorizationType { get; set; }

        [JsonProperty("azureSubscriptionId", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? AzureSubscriptionId { get; set; }

        [JsonProperty("azureSubscriptionName", NullValueHandling = NullValueHandling.Ignore)]
        public string AzureSubscriptionName { get; set; }

        [JsonProperty("clusterId", NullValueHandling = NullValueHandling.Ignore)]
        public string ClusterId { get; set; }

        [JsonProperty("namespace", NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }
    }

    public partial class OperationStatus
    {
        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("statusMessage")]
        public string StatusMessage { get; set; }
    }

    public partial class ServiceEndpointProjectReference
    {
        [JsonProperty("projectReference")]
        public ProjectReference ProjectReference { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
    }

    public partial class ProjectReference
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
    #endregion

    #region User and Groups

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
    #endregion

    public partial class ProcessTemplateCollection
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("value")]
        public ProcessTemplate[] Value { get; set; }
    }

    public partial class ProcessTemplate
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("isDefault")]
        public bool IsDefault { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }


    public partial class VstsItentitySearchResultResponse
    {
        [JsonProperty("results")]
        public VstsIdentitySearchResult[] Results { get; set; }
    }

    public partial class VstsIdentitySearchResult
    {
        [JsonProperty("queryToken")]
        public string QueryToken { get; set; }

        [JsonProperty("identities")]
        public VstsIdentitySearchedItem[] Identities { get; set; }

        [JsonProperty("pagingToken")]
        public string PagingToken { get; set; }
    }

    public partial class VstsIdentitySearchedItem
    {
        [JsonProperty("entityId")]
        public string EntityId { get; set; }

        [JsonProperty("entityType")]
        public string EntityType { get; set; }

        [JsonProperty("originDirectory")]
        public string OriginDirectory { get; set; }

        [JsonProperty("originId")]
        public Guid OriginId { get; set; }

        [JsonProperty("localDirectory")]
        public string LocalDirectory { get; set; }

        [JsonProperty("localId")]
        public Guid LocalId { get; set; }

        [JsonProperty("displayName")]
        public object DisplayName { get; set; }

        [JsonProperty("scopeName")]
        public object ScopeName { get; set; }

        [JsonProperty("samAccountName")]
        public object SamAccountName { get; set; }

        [JsonProperty("active")]
        public object Active { get; set; }

        [JsonProperty("subjectDescriptor")]
        public object SubjectDescriptor { get; set; }

        [JsonProperty("department")]
        public object Department { get; set; }

        [JsonProperty("jobTitle")]
        public object JobTitle { get; set; }

        [JsonProperty("mail")]
        public object Mail { get; set; }

        [JsonProperty("mailNickname")]
        public object MailNickname { get; set; }

        [JsonProperty("physicalDeliveryOfficeName")]
        public object PhysicalDeliveryOfficeName { get; set; }

        [JsonProperty("signInAddress")]
        public object SignInAddress { get; set; }

        [JsonProperty("surname")]
        public object Surname { get; set; }

        [JsonProperty("guest")]
        public bool Guest { get; set; }

        [JsonProperty("telephoneNumber")]
        public object TelephoneNumber { get; set; }

        [JsonProperty("description")]
        public object Description { get; set; }

        [JsonProperty("isMru")]
        public bool IsMru { get; set; }
    }

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
}
