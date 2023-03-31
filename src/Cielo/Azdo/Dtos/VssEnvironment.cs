using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core.Tokens;

namespace Cielo.Azdo.Dtos
{
    public record VstsEnvironmentCollection([property: JsonProperty("count")] int Count, [property: JsonProperty("value")] List<VstsEnvironment> Value);

    public class VstsEnvironment
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("lastModifiedOn")]
        public DateTime LastModifiedOn { get; set; }
    }

    public class VstsEnvRoleAssignment
    {
        [JsonProperty("userId")]
        public Guid userId { get; set; }

        [JsonProperty("roleName")]
        public string roleName { get; set; }
    }

    public class VstsEnvIdentityRef
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("uniqueName")]
        public string UniqueName { get; set; }
    }

    public class VstsEnvRole
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("allowPermissions")]
        public int AllowPermissions { get; set; }

        [JsonProperty("denyPermissions")]
        public int DenyPermissions { get; set; }

        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

    }

    public class VstsEnvPermission
    {
        [JsonProperty("identity")]
        public VstsEnvIdentityRef Identity { get; set; }

        [JsonProperty("role")]
        public VstsEnvRole Role { get; set; }

        [JsonProperty("access")]
        public string Access { get; set; }

        [JsonProperty("accessDisplayName")]
        public string AccessDisplayName { get; set; }
    }

    public class VstsEnvPermissionCollection
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("value")]
        public List<VstsEnvPermission> Value { get; set; }
    }
}
