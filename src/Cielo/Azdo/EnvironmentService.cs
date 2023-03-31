using Cielo.Azdo.Abstract;
using Cielo.Azdo.Dtos;
using static Cielo.Azdo.PermissionEnums;

namespace Cielo.Azdo
{
    public class EnvironmentService : RestServiceBase
    {
        public EnvironmentService(IHttpClientFactory clientFactory) : base(clientFactory)
        {

        }

        public async Task<bool> AddRootPermissionsAsync(
            Guid projectId, 
            List<VstsEnvRoleAssignment> roleAssignments)
        {
            if (roleAssignments != null)
            {
                var path = $"_apis/securityroles/scopes/distributedtask.globalenvironmentreferencerole/roleassignments/resources/{projectId}?api-version=5.1-preview.1";
                var result = await CoreApi().PutRestAsync(path, roleAssignments);
                return result != null;
            }
            return false;
        }

        public async Task<bool> AddPermissionsAsync(
            Guid projectId, int envId, 
            List<VstsEnvRoleAssignment> roleAssignments)
        {
            if(roleAssignments != null)
            {
                var path = $"_apis/securityroles/scopes/distributedtask.environmentreferencerole/roleassignments/resources/{projectId}_{envId}?api-version=5.1-preview.1";
                var result = await CoreApi().PutRestAsync(path, roleAssignments);
                return result != null;
            }
            return false;
        }

        public async Task<VstsEnvironment?> GetEnvironmentAsync(Guid projectId, string envName)
        {
            var envNameEncoded = RestUtils.UriEncode(envName);
            var path = $"{projectId}/_apis/distributedtask/environments?name={envNameEncoded}&api-version=5.1-preview.1";
            var envs = await CoreApi().GetRestAsync<VstsEnvironmentCollection>(path);

            if (envs != null && envs.Value != null)
            {
                return envs.Value.FirstOrDefault();
            }
            return null;
        }

        public async Task<List<VstsEnvPermission>> GetRootPermissionsAsync(Guid projectId)
        {
            var path = $"_apis/securityroles/scopes/distributedtask.globalenvironmentreferencerole/roleassignments/resources/{projectId}?api-version=5.1-preview.1";
            var permissionCollection = await CoreApi().GetRestAsync<VstsEnvPermissionCollection>(path);

            if (permissionCollection != null && permissionCollection.Value != null)
            {
                return permissionCollection.Value;
            }
            return null;
        }
        

        public async Task<VstsEnvironment?> CreateEnvironmentAsync(Guid projectId, string envName)
        {
            var env = await CoreApi()
                            .PostRestAsync<VstsEnvironment>(
                               $"{projectId}/_apis/distributedtask/environments?api-version=5.1-preview.1",
                               new
                               {
                                   name = envName,
                                   description = $"Created by automation"
                               });
            return env;
        }

        public async Task<List<VstsEnvPermission>?> GetEnvPermissionsAsync(Guid projectId, int envId)
        {
            var path = $"_apis/securityroles/scopes/distributedtask.environmentreferencerole/roleassignments/resources/{projectId}_{envId}";
            var permissions = await CoreApi().GetRestAsync<VstsEnvPermissionCollection>(path);
            if(permissions != null && permissions.Value != null)
            {
                return permissions.Value;
            }
            return null;
        }
    }
}
