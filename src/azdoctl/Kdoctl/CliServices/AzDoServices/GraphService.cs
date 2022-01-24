using Kdoctl.CliServices.Abstract;
using Kdoctl.CliServices.AzDoServices.Dtos;
using Kdoctl.CliServices.Supports;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Linq;
using System.Net.Http;

namespace Kdoctl.CliServices.AzDoServices
{
    public class GraphService : RestServiceBase
    {
        private static GroupCollection cachedCopy;

        public GraphService(IHttpClientFactory clientFactory) : base(clientFactory) { }

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

        public async Task<GroupCollection> ListGroupsAsync()
        {
            if(cachedCopy != null )
            {
                return cachedCopy;
            }
            cachedCopy = await ListAllGroupsFromOrganizationCoreAsync();
            return cachedCopy;
        }

        public async Task<List<VstsGroup>> ListGroupsInProjectAsync(Guid projectId)
        {
            var allGroups = await ListGroupsAsync();

            var projectGroups = allGroups.Value.Where(g => g.Domain.Contains($"{projectId}"));
            return projectGroups.ToList();
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
            // invalidate cache 
            cachedCopy = null;
            return ep;
        }

        public async Task<VstsMembershipCollection> GetGroupMembersAsync(string securityDescriptor)
        {
            var path = $"_apis/graph/Memberships/{securityDescriptor}?direction=Down&api-version=6.1-preview.1";
            var memberCollection = await VsspsApi()
                .GetRestAsync<VstsMembershipCollection>(path);

            return memberCollection;
        }

        public async Task<GroupCollection> ListUsersAsync()
        {
            var path = "_apis/graph/users?subjectTypes=aad&api-version=6.1-preview.1";
            var users = await VsspsApi()
                .GetRestAsync<GroupCollection>(path);

            return users;
        }

        //public async Task<string> GetStorageKey(Uri storageKeyUrl)
        //{
        //    var path = string.Empty;
        //    var storageKeys = await storageKeyUrl
        //        .GetRestAsync<string>(path);

        //    return storageKeys;
        //}

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

        public async Task<VstsIdentityCollection> GetLegacyIdentitiesByNameAsync(string name)
        {            
            var path = $"_apis/identities?searchFilter=General&filterValue={HttpUtility.UrlEncode(name)}&queryMembership=None&api-version=6.0";
            var users = await VsspsApi()
                .GetRestAsync<VstsIdentityCollection>(path);
            return users;
        }

        public async Task<VstsItentitySearchResultResponse> GetIdentityWithPickerdAsync(string userAccount)
        {
            var path = $"_apis/IdentityPicker/Identities?api-version=5.1-preview.1";
            var response = await CoreApi()
                .PostRestAsync<VstsItentitySearchResultResponse>(
                path,
                new
                { 
                    query = userAccount, 
                    identityTypes = new List<string> { "user", "group" }, 
                    operationScopes = new List<object> { "ims" } 
                });
            return response;
        }


        public async Task<bool> RemoveMembershipAsync(Guid projectId, string parentDescriptor, string childDescriptor)
        {
            var path = $"_apis/graph/memberships/{childDescriptor}/{parentDescriptor}?api-version=6.1-preview.1";

            var response = await VsspsApi()
                .DeleteRestAsync(path);
            return response;
        }

        public async Task<string> AddMemberAsync(Guid projectId, string parentDescriptor, string childDescriptor)
        {
            var path = $"_apis/graph/memberships/{childDescriptor}/{parentDescriptor}?api-version=6.0-preview.1";

            var response = await VsspsApi()
                .PutRestAsync(
                path,
                string.Empty);
            return response;
        }

        // User ID must be a GUID given by AzDO system after adding an user entitlement
        public async Task<string> GetUserEntitlementAsync(string userId)
        {
            var path = $"_apis/userentitlements/{userId}?api-version=5.0-preview.2";

            var response = await VsaexApi().GetRestJsonAsync(path);
            return response;
        }
    }
}
