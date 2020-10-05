

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Waddle;
using Waddle.Dtos;
using Waddle.Supports;

namespace Waddle
{
    public class ClassificationService : RestServiceBase
    {
        public ClassificationService(string adoUrl, string pat)
            : base(adoUrl, pat)
        {

        }

        public async Task<VstsClassification> GetAllAreaPathsAsync(Guid projectId, int depth = 1)
        {
            var path = $"{projectId}/_apis/wit/classificationnodes/Areas?$depth={depth}&api-version=6.0";
            var paths = await GetAzureDevOpsDefaultUri()
                .GetRestAsync<VstsClassification>(path, await GetBearerTokenAsync());

            return paths;
        }

        public async Task<VstsClassification> GetAllIterationsPathsAsync(Guid projectId, int depth = 1)
        {
            var path = $"{projectId}/_apis/wit/classificationnodes/Iterations?$depth={depth}&api-version=6.0";
            var paths = await GetAzureDevOpsDefaultUri()
                .GetRestAsync<VstsClassification>(path, await GetBearerTokenAsync());

            return paths;
        }        
    }
}
