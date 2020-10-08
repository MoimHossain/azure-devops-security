using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Waddle.Dtos;
using Waddle.Supports;

namespace Waddle
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
            var groups = await GetAzureDevOpsVsspUri()
                .GetRestAsync<GroupCollection>(path, await GetBearerTokenAsync());

            return groups;
        }

        public async Task<VstsGroup> CreateAadGroupByObjectId(Guid aadObjectId)
        {
            var ep = await GetAzureDevOpsVsspUri()
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
            var users = await GetAzureDevOpsVsspUri()
                .GetRestAsync<GroupCollection>(path, await GetBearerTokenAsync());

            return users;
        }


        public async Task<VstsIdentityCollection> GetLegacyIdentitiesBySidAsync(string descriptors)
        {
            var path = $"_apis/identities?descriptors={descriptors}&queryMembership=None&api-version=6.0";

            //var path = $"_apis/identities?searchFilter=General&filterValue={HttpUtility.UrlEncode("[TEAM FOUNDATION]\\IoT-Developers")}&queryMembership=None&api-version=6.0";
            var users = await GetAzureDevOpsVsspUri()
                .GetRestAsync<VstsIdentityCollection>(path, await GetBearerTokenAsync());

            return users;
        }
        public async Task<VstsIdentityCollection> GetLegacyIdentitiesByNameAsync(string name)
        {
            //var path = $"_apis/identities?searchFilter=General&filterValue={HttpUtility.UrlEncode("[TEAM FOUNDATION]\\IoT-Developers")}&queryMembership=None&api-version=6.0";
            var path = $"_apis/identities?searchFilter=General&filterValue={HttpUtility.UrlEncode(name)}&queryMembership=None&api-version=6.0";
            var users = await GetAzureDevOpsVsspUri()
                .GetRestAsync<VstsIdentityCollection>(path, await GetBearerTokenAsync());

            return users;
        }
    }
}
