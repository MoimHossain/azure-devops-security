
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Waddle.Dtos;
using Waddle.Supports;

namespace Waddle
{
    public class PipelineEnvironmentService : RestServiceBase
    {
        public PipelineEnvironmentService(string adoUrl, string pat)
            : base(adoUrl, pat)
        {

        }
        public enum PipelineEnvironmentPermissions
        {
            Administrator,
            Reader,
            User
        }

        public async Task<PipelineEnvironmentCollection> ListEnvironmentsAsync(Guid projectId)
        {
            var envs = await GetAzureDevOpsDefaultUri()
                  .GetRestAsync<PipelineEnvironmentCollection>(
                  $"{projectId}/_apis/distributedtask/environments",
                  await GetBearerTokenAsync());

            return envs;
        }

        public async Task<PipelineEnvironment> CreateEnvironmentAsync(
                Guid project, string envName, string envDesc)
        {
            var env = await GetAzureDevOpsDefaultUri()
                .PostRestAsync<PipelineEnvironment>(
                $"{project}/_apis/distributedtask/environments?api-version=5.1-preview.1",
                new
                {
                    name = envName,
                    description = envDesc
                },
                await GetBearerTokenAsync());

            return env;
        }

        public async Task<VstsServiceEndpointAccessControlCollection> SetPermissionAsync(
            Guid projectId, 
            long envId, 
            Guid localId,
            PipelineEnvironmentPermissions permission)
        {
            var response = await GetAzureDevOpsDefaultUri()
                .PutRestAsync(
                $"_apis/securityroles/scopes/distributedtask.environmentreferencerole/roleassignments/resources/{projectId}_{envId}?api-version=5.0-preview.1",
                new List<object>
                {
                    new 
                    {
                        roleName = permission.ToString(),
                        userId = localId
                    }
                },
                await GetBearerTokenAsync());
            return JsonConvert.DeserializeObject<VstsServiceEndpointAccessControlCollection>(response);
        }
    }
}