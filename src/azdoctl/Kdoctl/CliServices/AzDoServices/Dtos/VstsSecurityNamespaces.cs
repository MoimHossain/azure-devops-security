using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kdoctl.CliServices.AzDoServices.Dtos
{
    public partial class VstsSecurityNamespaceCollection
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("value")]
        public VstsSecurityNamespace[] Value { get; set; }
    }

    public partial class VstsSecurityNamespace
    {
        [JsonProperty("namespaceId")]
        public Guid NamespaceId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("separatorValue")]
        public string SeparatorValue { get; set; }

        [JsonProperty("elementLength")]
        public long ElementLength { get; set; }

        [JsonProperty("writePermission")]
        public long WritePermission { get; set; }

        [JsonProperty("readPermission")]
        public long ReadPermission { get; set; }

        [JsonProperty("dataspaceCategory")]
        public string DataspaceCategory { get; set; }

        [JsonProperty("actions")]
        public VstsAction[] Actions { get; set; }

        [JsonProperty("structureValue")]
        public long StructureValue { get; set; }

        [JsonProperty("extensionType")]
        public string ExtensionType { get; set; }

        [JsonProperty("isRemotable")]
        public bool IsRemotable { get; set; }

        [JsonProperty("useTokenTranslator")]
        public bool UseTokenTranslator { get; set; }
    }

    public partial class VstsAction
    {
        [JsonProperty("bit")]
        public long Bit { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("namespaceId")]
        public Guid NamespaceId { get; set; }
    }
}
