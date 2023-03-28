using Newtonsoft.Json;

namespace Cielo.Azdo.Dtos
{

    public partial class VstsFolderCollection
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("value")]
        public VstsFolder[] Value { get; set; }
    }

    public partial class VstsFolder
    {
        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("createdOn")]
        public DateTimeOffset CreatedOn { get; set; }

        [JsonProperty("createdBy")]
        public VstsFolderCreatedBy CreatedBy { get; set; }

        [JsonProperty("lastChangedBy")]
        public string LastChangedBy { get; set; }

        public override string ToString()
        {
            return this.Path;
        }
    }

    public class VstsFolderCreatedBy
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

}
