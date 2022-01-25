

using Newtonsoft.Json;
using System;

namespace Kdoctl.CliServices.AzDoServices.Dtos
{
    public partial class VstsClassification
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("identifier")]
        public Guid Identifier { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("structureType")]
        public string StructureType { get; set; }

        [JsonProperty("hasChildren")]
        public bool HasChildren { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

   

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("children", NullValueHandling = NullValueHandling.Ignore)]
        public VstsClassification[] Children { get; set; }
    }
}
