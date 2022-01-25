

using Kdoctl.CliServices.Abstract;
using Kdoctl.CliServices.AzDoServices.Dtos;
using Kdoctl.CliServices.Supports;
using System;
using System.Net.Http;
using System.Threading.Tasks;


namespace Kdoctl.CliServices.AzDoServices
{
    public class ClassificationService : RestServiceBase
    {
        public ClassificationService(IHttpClientFactory clientFactory) : base(clientFactory) { }

        public async Task<VstsClassification> GetAllAreaPathsAsync(Guid projectId, int depth = 1)
        {
            var path = $"{projectId}/_apis/wit/classificationnodes/Areas?$depth={depth}&api-version=6.0";
            var paths = await CoreApi()
                .GetRestAsync<VstsClassification>(path);

            return paths;
        }

        public async Task<VstsClassification> GetAllIterationsPathsAsync(Guid projectId, int depth = 1)
        {
            var path = $"{projectId}/_apis/wit/classificationnodes/Iterations?$depth={depth}&api-version=6.0";
            var paths = await CoreApi()
                .GetRestAsync<VstsClassification>(path);

            return paths;
        }        
    }
}
