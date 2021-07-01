

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kdoctl.CliServices.AzDoServices.Dtos
{
    public partial class VstsServiceEndpointCollection
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("value")]
        public VstsServiceEndpoint[] Value { get; set; }
    }

    public partial class VstsServiceEndpoint
    {
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
        public VstsServiceEndpointCreatedBy CreatedBy { get; set; }

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
    }



    public partial class VstsServiceEndpointCreatedBy
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

    public partial class VstsServiceEndpointAccessControlCollection
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("value")]
        public VstsServiceEndpointAccessControl[] Value { get; set; }
    }

    public partial class VstsServiceEndpointAccessControl
    {
        [JsonProperty("identity")]
        public VstsServiceEndpointAccessControlIdentity Identity { get; set; }

        [JsonProperty("role")]
        public VstsServiceEndpointAccessControlRole Role { get; set; }

        [JsonProperty("access")]
        public string Access { get; set; }

        [JsonProperty("accessDisplayName")]
        public string AccessDisplayName { get; set; }
    }

    public partial class VstsServiceEndpointAccessControlIdentity
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("uniqueName")]
        public string UniqueName { get; set; }
    }

    public partial class VstsServiceEndpointAccessControlRole
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("allowPermissions")]
        public long AllowPermissions { get; set; }

        [JsonProperty("denyPermissions")]
        public long DenyPermissions { get; set; }

        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }
    }
}
