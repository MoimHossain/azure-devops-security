using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Waddle.Dtos;
using Waddle.Supports;

namespace Waddle
{
    public class GroupService : RestServiceBase
    {
        public GroupService(string adoUrl, string pat)
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
    }
}
