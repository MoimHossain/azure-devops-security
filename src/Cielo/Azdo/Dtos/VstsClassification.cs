using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cielo.Azdo.Dtos
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

        [JsonIgnore]
        public string TrimmedPath
        {
            get
            {
                return ClassificationService.GetNormalizedPath(this.Path.Replace("\\Area", string.Empty));
            }
        }


        private List<Guid> parentNodeIds = new List<Guid>();
        [JsonIgnore]
        public List<Guid> ParentNodeIds
        {
            get
            {
                return this.parentNodeIds;
            }
        }

        public override string ToString()
        {
            return $"{this.Name} = {this.Path}";
        }
    }

    public class WorkItemClassificationNode
    {
        [JsonProperty("NodeId")]
        public Guid NodeId { get; set; }

        [JsonProperty("NodeName")]
        public string NodeName { get; set; }

        [JsonProperty("ParentId")]
        public Guid ParentId { get; set; }

        [JsonProperty("IterationStartDate")]
        public DateTime? IterationStartDate { get; set; }

        [JsonProperty("IterationEndDate")]
        public DateTime? IterationEndDate { get; set; }

        public object GetCreateOrUpdatePayload()
        {
            return new
            {
                syncWorkItemTracking = false,
                operationData = JsonConvert.SerializeObject(this)
            };
        }
    }

    public class WorkItemClassificationCreateOrUpdateResponse
    {
        [JsonProperty("node")]
        public WorkItemClassificationNodeInfo Node { get; set; }
    }
    public class WorkItemClassificationNodeInfo
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("parentId")]
        public Guid ParentId { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
