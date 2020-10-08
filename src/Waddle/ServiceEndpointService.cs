
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Waddle.Dtos;
using Waddle.Supports;

namespace Waddle
{
    public class ServiceEndpointService : RestServiceBase
    {
        public ServiceEndpointService(string adoUrl, string pat)
            : base(adoUrl, pat)
        {

        }

        public async Task<VstsServiceEndpointCollection> ListServiceEndpointsAsync(Guid projectId)
        {
            var requestPath = $"{projectId}/_apis/serviceendpoint/endpoints?api-version=6.1-preview.4";
            var endpoints = await GetAzureDevOpsDefaultUri()
                .GetRestAsync<VstsServiceEndpointCollection>(requestPath, await GetBearerTokenAsync());

            return endpoints;
        }

        public enum ServiceEndpointPermissions
        {
            Administrator,
            Reader,
            User
        }

        public async Task<VstsServiceEndpointAccessControlCollection> SetPermissionProjectLevelAsync(
            Guid projectId, 
            Guid endpointId, 
            Guid localId, 
            ServiceEndpointPermissions permission)
        {
            var response = await GetAzureDevOpsDefaultUri()
                .PutRestAsync(
                $"_apis/securityroles/scopes/distributedtask.serviceendpointrole/roleassignments/resources/{projectId}_{endpointId}?api-version=5.0-preview.1",
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

        public async Task<VstsServiceEndpointAccessControlCollection> SetPermissionCollectionLevelAsync(
            Guid endpointId,
            Guid localId)
        {
            var response = await GetAzureDevOpsDefaultUri()
                .PutRestAsync(
                $"_apis/securityroles/scopes/distributedtask.serviceendpointrole/roleassignments/resources/collection_{endpointId}?api-version=5.0-preview.1",
                new List<object>
                {
                    new
                    {
                        roleName = ServiceEndpointPermissions.Administrator.ToString(), // only admin supported for this level
                        userId = localId
                    }
                },
                await GetBearerTokenAsync());
            return JsonConvert.DeserializeObject<VstsServiceEndpointAccessControlCollection>(response);
        }
    }
}