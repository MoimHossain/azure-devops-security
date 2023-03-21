using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
}
