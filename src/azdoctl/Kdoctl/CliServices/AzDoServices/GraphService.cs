using Kdoctl.CliOptions;
using Kdoctl.CliServices.Abstract;
using Kdoctl.CliServices.AzDoServices.Dtos;
using Kdoctl.CliServices.Supports;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.Services.Aad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using static System.Formats.Asn1.AsnWriter;

namespace Kdoctl.CliServices.AzDoServices
{
    public class GraphService : RestServiceBase
    {
        private OptionBase baseOpts;

        public GraphService(OptionBase baseOpts, IHttpClientFactory clientFactory) : base(clientFactory)
        {
            this.baseOpts = baseOpts;
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
            if(string.IsNullOrWhiteSpace(description))
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







//public async Task<VstsItentitySearchResultResponse> GetIdentityWithPickerdAsync(string userAccount)
//{
//    var path = $"_apis/IdentityPicker/Identities?api-version=5.1-preview.1";
//    var response = await CoreApi()
//        .PostRestAsync<VstsItentitySearchResultResponse>(
//        path,
//        new
//        { 
//            query = userAccount, 
//            identityTypes = new List<string> { "user", "group" }, 
//            operationScopes = new List<object> { "ims" } 
//        });
//    return response;
//}
/*
 * 
// User ID must be a GUID given by AzDO system after adding an user entitlement
        public async Task<string> GetUserEntitlementAsync(string userId)
        {
            var path = $"_apis/userentitlements/{userId}?api-version=5.0-preview.2";

            var response = await VsaexApi().GetRestJsonAsync(path);
            return response;
        }
        public async Task<VstsIdentityCollection> GetLegacyIdentitiesByNameAsync(string name)
        {
            var path = $"_apis/identities?searchFilter=General&filterValue={RestUtils.UrlEncode(name)}&queryMembership=None&api-version=6.0";
            var users = await VsspsApi()
                .GetRestAsync<VstsIdentityCollection>(path);
            return users;
        }
        private async Task<GroupCollection> ListAllGroupsFromOrganizationCoreAsync()
        {
            var extractToken = new Func<HttpResponseMessage, string>((response) =>
            {
                foreach (var header in response.Headers)
                {
                    if (header.Key.Equals("X-MS-ContinuationToken", StringComparison.OrdinalIgnoreCase) && header.Value != null)
                    {
                        return header.Value.FirstOrDefault();
                    }
                }
                return string.Empty;
            });

            var nextPageToken = string.Empty;
            var allGroups = new List<VstsGroup>();

            var path = "_apis/graph/groups?api-version=6.1-preview.1";
            var groups = await VsspsApi()
                .GetRestAsync<GroupCollection>(path,                    
                    response =>
                    {
                        nextPageToken = extractToken(response);
                    });
            allGroups.AddRange(groups.Value);

            while (!string.IsNullOrWhiteSpace(nextPageToken))
            {
                var nextPath = $"{path}&continuationToken={nextPageToken}";
                groups = await VsspsApi()
                    .GetRestAsync<GroupCollection>(nextPath,                        
                        response =>
                        {
                            nextPageToken = extractToken(response);
                        });
                allGroups.AddRange(groups.Value);
            }
            return new GroupCollection { Value = allGroups.ToArray(), Count = allGroups.Count };
        }
        */
/*
/// <summary>
/// //////Delete below
/// </summary>
/// <returns></returns>
public async Task<GroupCollection> ListGroupsAsync()
{
    if(cachedCopy != null )
    {
        return cachedCopy;
    }
    cachedCopy = await ListAllGroupsFromOrganizationCoreAsync();
    return cachedCopy;
}

/// <summary>
/// //////Delete below
/// </summary>
/// <returns></returns>
public async Task<List<VstsGroup>> ListGroupsInProjectAsync(Guid projectId)
{
    var allGroups = await ListGroupsAsync();

    var projectGroups = allGroups.Value.Where(g => g.Domain.Contains($"{projectId}"));
    return projectGroups.ToList();
}*/
/*
public async Task<GroupCollection> ListUsersAsync()
{
    var continuationTokenHeader = "x-ms-continuationtoken";

    var cToken = string.Empty;
    var allItems = new List<VstsGroup>();
    do
    {
        var path = "_apis/graph/users?subjectTypes=aad&api-version=6.1-preview.1";

        if(!string.IsNullOrWhiteSpace(cToken))
        {
            path = $"{path}&continuationToken={cToken}";
        }

        var users = await VsspsApi()
        .GetRestAsync<GroupCollection>(path,
        response =>
        {
            if (response.Headers.TryGetValues(continuationTokenHeader, out var values))
            {
                if (values != null && values.Any())
                {
                    cToken = values.FirstOrDefault();
                }
            }
        });
        if(users != null && users.Value != null)
        {
            allItems.AddRange(users.Value);
        }

    } while (!string.IsNullOrEmpty(cToken));
    return new GroupCollection { Value = allItems.ToArray(), Count = allItems.Count };
}
*/