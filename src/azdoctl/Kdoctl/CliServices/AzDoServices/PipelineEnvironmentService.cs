
using Kdoctl.CliServices.Abstract;
using Kdoctl.CliServices.AzDoServices.Dtos;
using Kdoctl.CliServices.Supports;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kdoctl.CliServices.AzDoServices
{
    public class PipelineEnvironmentService : RestServiceBase
    {
        public PipelineEnvironmentService(IHttpClientFactory clientFactory) : base(clientFactory) { }

        public enum PipelineEnvironmentPermissions
        {
            Administrator,
            Reader,
            User
        }

        public async Task<PipelineEnvironmentCollection> ListEnvironmentsAsync(Guid projectId)
        {
            var envs = await CoreApi()
                  .GetRestAsync<PipelineEnvironmentCollection>(
                  $"{projectId}/_apis/distributedtask/environments");

            return envs;
        }

        public async Task<PipelineEnvironment> CreateEnvironmentAsync(
                Guid project, string envName, string envDesc)
        {
            var env = await CoreApi()
                .PostRestAsync<PipelineEnvironment>(
                $"{project}/_apis/distributedtask/environments?api-version=5.1-preview.1",
                new
                {
                    name = envName,
                    description = envDesc
                });

            return env;
        }

        public async Task<VstsServiceEndpointAccessControlCollection> SetPermissionAsync(
            Guid projectId, 
            long envId, 
            Guid localId,
            PipelineEnvironmentPermissions permission)
        {
            var response = await CoreApi()
                .PutRestAsync(
                $"_apis/securityroles/scopes/distributedtask.environmentreferencerole/roleassignments/resources/{projectId}_{envId}?api-version=5.0-preview.1",
                new List<object>
                {
                    new 
                    {
                        roleName = permission.ToString(),
                        userId = localId
                    }
                });
            return JsonConvert.DeserializeObject<VstsServiceEndpointAccessControlCollection>(response);
        }
    }
}