using Cielo.Azdo.Abstract;
using Cielo.Azdo.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cielo.Azdo
{
    public class GraphService : RestServiceBase
    {
        public GraphService(IHttpClientFactory clientFactory) : base(clientFactory)
        {
            
        }

        public async Task<VstsIdentity> GetIdentityObjectAsync(Guid id)
        {
            var path = $"_apis/identities?identityIds={id}&api-version=7.0";
            var identityCOll = await VsspsApi().GetRestAsync<VstsIdentityCollection>(path);
            if (identityCOll != null && identityCOll.Value != null)
            {
                return identityCOll.Value.FirstOrDefault();
            }
            return null;
        }

        public async Task<VstsGroup> GetAadGroupById(Guid aadObjectId)
        {
            // it seems until you use a AAD group the materialzation doesn't persist and you won't
            // find it later by searching by names
            return await this.CreateAadGroupByObjectId(aadObjectId);
        }

        public async Task<VstsGroup> GetGroupByNameFromCollectionAsync(string groupName)
        {
            var path = "_apis/graph/subjectquery?api-version=7.0-preview.1";
            var groups = await VsspsApi().PostRestAsync<GroupCollection>(path,
            new
            {
                query = groupName,
                subjectKind = new List<string> { "Group" }
            });
            if (groups != null && groups.Value != null && groups.Value.Length > 0)
            {
                return groups.Value.FirstOrDefault();
            }
            return null;
        }

        public async Task<VstsGroup> GetGroupByNameFromProjectAsync(string projectName, string groupName)
        {
            var fqgn = $"[{projectName}]\\{groupName}";

            var path = $"_apis/identities?api-version=6.0&searchFilter=General&filterValue={RestUtils.UriEncode(fqgn)}";
            var identities = await VsspsApi().GetRestAsync<IdentityInternalCollectionDto>(path);
            if (identities != null && identities.Value != null && identities.Value.Count > 0)
            {
                var groupIdentity = identities.Value.FirstOrDefault();
                if (groupIdentity != null && fqgn.Equals(groupIdentity.ProviderDisplayName, StringComparison.OrdinalIgnoreCase))
                {
                    var group = await this.GetGroupByDescriptorAsync(groupIdentity.SubjectDescriptor);
                    return group;
                }
            }
            return null;
        }

        public async Task<VstsGroup> CreateOrgScopedVstsGroupAsync(string displayName, string description = "")
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                description = displayName;
            }
            var ep = await VsspsApi()
                .PostRestAsync<VstsGroup>(
                $"_apis/graph/groups?api-version=7.0-preview.1",
                new
                {
                    displayName,
                    description
                });
            return ep;

        }

        public async Task<VstsGroup> CreateProjectScopedVstsGroupAsync(Guid projectId, string displayName, string description = "")
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                description = displayName;
            }

            var path = $"_apis/graph/descriptors/{projectId}";
            var projectDescriptor = await VsspsApi().GetRestAsync<VstsProjectDescriptor>(path);

            if (projectDescriptor != null && !string.IsNullOrWhiteSpace(projectDescriptor.ScopeDescriptor))
            {
                var ep = await VsspsApi()
                    .PostRestAsync<VstsGroup>(
                    $"_apis/graph/groups?scopeDescriptor={projectDescriptor.ScopeDescriptor}&api-version=7.1-preview.1",
                    new
                    {
                        displayName = displayName,
                        description
                    });
                return ep;
            }
            return null;
        }

        public async Task<VstsGroup> CreateAadGroupByObjectId(Guid aadObjectId)
        {
            var ep = await VsspsApi()
                .PostRestAsync<VstsGroup>(
                $"_apis/graph/groups?api-version=6.0-preview.1",
                new
                {
                    originId = aadObjectId
                });
            return ep;
        }

        public async Task<VstsMembershipCollection> GetGroupMembersAsync(string securityDescriptor)
        {
            var path = $"_apis/graph/Memberships/{securityDescriptor}?direction=Down&api-version=6.1-preview.1";
            var memberCollection = await VsspsApi()
                .GetRestAsync<VstsMembershipCollection>(path);

            return memberCollection;
        }
        public bool IsGroupDescriptor(string descriptor)
        {
            return !string.IsNullOrWhiteSpace(descriptor) &&
                (descriptor.StartsWith("aadgp.", StringComparison.OrdinalIgnoreCase)
                    || descriptor.StartsWith("vssgp.", StringComparison.OrdinalIgnoreCase));
        }
        public async Task<VstsGroup> GetGroupByDescriptorAsync(string descriptor)
        {
            var path = $"_apis/graph/groups/{descriptor}?api-version=6.1-preview.1";
            var group = await VsspsApi()
                .GetRestAsync<VstsGroup>(path);
            return group;
        }
        public async Task<VstsUser> GetUserByDescriptorAsync(string descriptor)
        {
            var path = $"_apis/graph/users/{descriptor}?api-version=6.1-preview.1";
            var user = await VsspsApi()
                .GetRestAsync<VstsUser>(path);
            return user;
        }
        public async Task<VstsIdentityCollection> GetLegacyIdentitiesBySidAsync(string descriptors)
        {
            var path = $"_apis/identities?descriptors={descriptors}&queryMembership=None&api-version=6.0";
            var users = await VsspsApi()
                .GetRestAsync<VstsIdentityCollection>(path);

            return users;
        }
        public async Task<bool> RemoveMembershipAsync(Guid projectId, string parentDescriptor, string childDescriptor)
        {
            var path = $"_apis/graph/memberships/{childDescriptor}/{parentDescriptor}?api-version=6.1-preview.1";

            var response = await VsspsApi()
                .DeleteRestAsync(path);
            return response;
        }
        public async Task<bool> AddMemberAsync(Guid projectId, string parentDescriptor, string childDescriptor)
        {
            var path = $"_apis/graph/memberships/{childDescriptor}/{parentDescriptor}?api-version=6.0-preview.1";

            var response = await VsspsApi()
                .PutRestAsync(
                path,
                string.Empty);
            return !string.IsNullOrWhiteSpace(response);
        }
        public async Task<VstsUserInfo> GetUserByPrincipalNameAsync(string principalName)
        {
            var path = $"_apis/UserEntitlements?$filter=name eq '{principalName}'&$orderBy=name Ascending";
            var response = await VsaexApi().GetRestAsync<VstsUserEntitlementCollection>(path);
            if (response != null && response.Items != null && response.Items.Count > 0)
            {
                return response.Items.First().User;
            }
            return null;
        }
        public string GetSecurityDescriptorForUser(VstsUserInfo user)
        {
            return $"Microsoft.IdentityModel.Claims.ClaimsIdentity;{user.Domain}\\{user.PrincipalName}";
        }
    }
}
