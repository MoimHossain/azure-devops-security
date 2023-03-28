using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Cielo.Azdo.Dtos
{

    public partial class VstsTeamCollection
    {
        [JsonProperty("value")]
        public VstsTeam[] Value { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }
    }

    public partial class VstsTeam
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("identityUrl")]
        public Uri IdentityUrl { get; set; }

        [JsonProperty("projectName")]
        public string ProjectName { get; set; }

        [JsonProperty("projectId")]
        public Guid ProjectId { get; set; }
    }


    public partial class VstsTeamDescriptor
    {
        [JsonProperty("value")]
        public string ScopeDescriptor { get; set; }
    }

    public class MsVssTeamAdmin
    {
        [JsonProperty("displayName")]
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonProperty("uniqueName")]
        [JsonPropertyName("uniqueName")]
        public string UniqueName { get; set; }

        [JsonProperty("descriptor")]
        [JsonPropertyName("descriptor")]
        public string Descriptor { get; set; }
    }

    public class VstsTeamAdminDataProvider
    {
        [JsonProperty("ms.vss-admin-web.admin-teams-admin-list-data-provider")]
        [JsonPropertyName("ms.vss-admin-web.admin-teams-admin-list-data-provider")]
        public List<MsVssTeamAdmin> Admins { get; set; }
    }

    public class VstsTeamAdminResponseRoot
    {
        [JsonProperty("dataProviders")]
        [JsonPropertyName("dataProviders")]
        public VstsTeamAdminDataProvider DataProviders { get; set; }
    }




    public class VstsTeamConfig
    {
        [JsonProperty("defaultValue")]
        [JsonPropertyName("defaultValue")]
        public string DefaultPath { get; set; }

        [JsonProperty("values")]
        [JsonPropertyName("values")]
        public List<VstsTeamConfigAreaPath> AreaPaths { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string TrimmedDefaultPath
        {
            get {
                return ClassificationService.GetNormalizedPath(this.DefaultPath);
            }
        }
    }

    public class VstsTeamConfigAreaPath
    {
        [JsonProperty("value")]
        [JsonPropertyName("value")]
        public string Path { get; set; }

        [JsonProperty("includeChildren")]
        [JsonPropertyName("includeChildren")]
        public bool IncludeChildren { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string TrimmedPath
        {
            get
            {
                return ClassificationService.GetNormalizedPath(this.Path);
            }
        }
    }
}
