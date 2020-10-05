using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Waddle.Dtos;
using Waddle.Supports;

namespace Waddle
{
    public class ProjectService : RestServiceBase
    {
        public ProjectService(string adoUrl, string pat)
            : base(adoUrl, pat)
        {

        }

        public async Task<ProjectCollection> GetProjectsAsync()
        {
            var path = "_apis/projects?stateFilter=All&api-version=1.0";
            var projects = await GetAzureDevOpsDefaultUri()
                .GetRestAsync<ProjectCollection>(path, await GetBearerTokenAsync());

            return projects;
        }
    }
}
