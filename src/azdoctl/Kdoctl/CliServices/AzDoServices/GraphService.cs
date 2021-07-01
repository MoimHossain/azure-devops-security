using Kdoctl.CliServices.Abstract;
using Kdoctl.CliServices.AzDoServices.Dtos;
using Kdoctl.CliServices.Supports;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Kdoctl.CliServices.AzDoServices
{
    public class GraphService : RestServiceBase
    {
        public GraphService(string adoUrl, string pat)
            : base(adoUrl, pat)
        {

        }

        public async Task<GroupCollection> ListGroupsAsync()
        {
            var path = "_apis/graph/groups?api-version=6.0-preview.1";
            var groups = await VsspsApi()
                .GetRestAsync<GroupCollection>(path, await GetBearerTokenAsync());

            return groups;
        }

        public async Task<VstsGroup> CreateAadGroupByObjectId(Guid aadObjectId)
        {
            var ep = await VsspsApi()
                .PostRestAsync<VstsGroup>(
                $"_apis/graph/groups?api-version=6.0-preview.1",
                new
                {
                    originId = aadObjectId
                },
                await GetBearerTokenAsync());
            return ep;
        }

        public async Task<GroupCollection> ListUsersAsync()
        {
            var path = "_apis/graph/users?subjectTypes=aad&api-version=6.1-preview.1";
            var users = await VsspsApi()
                .GetRestAsync<GroupCollection>(path, await GetBearerTokenAsync());

            return users;
        }

        public async Task<string> GetStorageKey(Uri storageKeyUrl)
        {
            var path = string.Empty;
            var storageKeys = await storageKeyUrl
                .GetRestAsync<string>(path, await GetBearerTokenAsync());

            return storageKeys;
        }

        public async Task<VstsIdentityCollection> GetLegacyIdentitiesBySidAsync(string descriptors)
        {
            var path = $"_apis/identities?descriptors={descriptors}&queryMembership=None&api-version=6.0";

            //var path = $"_apis/identities?searchFilter=General&filterValue={HttpUtility.UrlEncode("[TEAM FOUNDATION]\\IoT-Developers")}&queryMembership=None&api-version=6.0";
            var users = await VsspsApi()
                .GetRestAsync<VstsIdentityCollection>(path, await GetBearerTokenAsync());

            return users;
        }

        public async Task<VstsIdentityCollection> GetLegacyIdentitiesByNameAsync(string name)
        {
            //var path = $"_apis/identities?searchFilter=General&filterValue={HttpUtility.UrlEncode("[TEAM FOUNDATION]\\IoT-Developers")}&queryMembership=None&api-version=6.0";
            var path = $"_apis/identities?searchFilter=General&filterValue={HttpUtility.UrlEncode(name)}&queryMembership=None&api-version=6.0";
            var users = await VsspsApi()
                .GetRestAsync<VstsIdentityCollection>(path, await GetBearerTokenAsync());

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
                },
                await GetBearerTokenAsync());
            return response;
        }

        public async Task<string> AddMemberAsync(Guid projectId, string parentDescriptor, string childDescriptor)
        {
            var path = $"_apis/graph/memberships/{childDescriptor}/{parentDescriptor}?api-version=6.0-preview.1";

            var response = await VsspsApi()
                .PutRestAsync(
                path,
                string.Empty,
                await GetBearerTokenAsync());
            return response;
        }
    }
}
