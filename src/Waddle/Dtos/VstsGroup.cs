using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Waddle.Dtos
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
        public User CreatedBy { get; set; }

        [JsonProperty("createdOn")]
        public DateTimeOffset CreatedOn { get; set; }

        [JsonProperty("lastModifiedBy")]
        public User LastModifiedBy { get; set; }

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
        public CreatedBy CreatedBy { get; set; }

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

    public partial class CreatedBy
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("uniqueName")]
        public string UniqueName { get; set; }

        [JsonProperty("imageUrl")]
        public Uri ImageUrl { get; set; }

        [JsonProperty("descriptor")]
        public string Descriptor { get; set; }
    }

    public partial class Links
    {
        [JsonProperty("avatar")]
        public Avatar Avatar { get; set; }
    }

    public partial class Avatar
    {
        [JsonProperty("href")]
        public Uri Href { get; set; }
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

    public partial class User
    {
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
        public Group[] Value { get; set; }
    }

    public partial class Group
    {
        [JsonProperty("subjectKind")]
        public string SubjectKind { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }

        [JsonProperty("principalName")]
        public string PrincipalName { get; set; }

        [JsonProperty("mailAddress")]
        public object MailAddress { get; set; }


        [JsonProperty("originId")]
        public Guid OriginId { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("descriptor")]
        public string Descriptor { get; set; }

        [JsonProperty("isCrossProject", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsCrossProject { get; set; }
    }
    #endregion
}
