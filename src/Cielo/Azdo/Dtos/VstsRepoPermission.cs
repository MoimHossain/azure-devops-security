

using Newtonsoft.Json;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json.Serialization;
using static Cielo.Azdo.PermissionEnums;

namespace Cielo.Azdo.Dtos
{
    public class VstsRepoPermissionDataProviders
    {
        [JsonProperty("ms.vss-admin-web.security-view-permissions-data-provider")]
        [JsonPropertyName("ms.vss-admin-web.security-view-permissions-data-provider")]
        public MsVssAdminWebSecurityViewPermissionsDataProvider MsVssAdminWebSecurityViewPermissionsDataProvider { get; set; }
    }

    public class MsVssAdminWebSecurityViewPermissionsDataProvider
    {
        [JsonProperty("subjectPermissions")]
        [JsonPropertyName("subjectPermissions")]
        public List<VstsSubjectPermission> SubjectPermissions { get; set; }

        [JsonProperty("identityDescriptor")]
        [JsonPropertyName("identityDescriptor")]
        public string IdentityDescriptor { get; set; }

        [JsonProperty("canEditPermissions")]
        [JsonPropertyName("canEditPermissions")]
        public bool CanEditPermissions { get; set; }

        [JsonProperty("userHasReadAccess")]
        [JsonPropertyName("userHasReadAccess")]
        public bool UserHasReadAccess { get; set; }
    }


    public class VstsRepoPermissionRoot
    {
        [JsonProperty("dataProviders")]
        [JsonPropertyName("dataProviders")]
        public VstsRepoPermissionDataProviders DataProviders { get; set; }
    }

    public class VstsSubjectPermission
    {
        [JsonProperty("displayName")]
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("namespaceId")]
        [JsonPropertyName("namespaceId")]
        public string NamespaceId { get; set; }

        [JsonProperty("token")]
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonProperty("bit")]
        [JsonPropertyName("bit")]
        public int Bit { get; set; }

        [JsonProperty("canEdit")]
        [JsonPropertyName("canEdit")]
        public bool CanEdit { get; set; }

        [JsonProperty("permissionDisplayString")]
        [JsonPropertyName("permissionDisplayString")]
        public string PermissionDisplayString { get; set; }

        [JsonProperty("effectivePermissionValue")]
        [JsonPropertyName("effectivePermissionValue")]
        public int? EffectivePermissionValue { get; set; }

        [JsonProperty("isPermissionInherited")]
        [JsonPropertyName("isPermissionInherited")]
        public bool? IsPermissionInherited { get; set; }        


    }
}
