

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Waddle.Constants;
using Waddle.Dtos;
using Waddle.Supports;

namespace Waddle
{
    public class SecurityNamespaceService : RestServiceBase
    {
        public SecurityNamespaceService(string adoUrl, string pat) 
            : base(adoUrl, pat)
        {

        }

        public async Task<VstsSecurityNamespace> GetNamespaceAsync(SecurityNamespaceConstants securityNamespace)
        {
            var sv = securityNamespace.GetStringValue();
            var all = await GetAllNamespacesAsync();
            var ns = all.Value.FirstOrDefault(sn => sn.Name.Equals(sv, StringComparison.OrdinalIgnoreCase));

            return ns;
        }

        
        public async Task<VstsSecurityNamespace> GetNamespaceAsync(
            SecurityNamespaceConstants securityNamespace, string action)
        {
            var sv = securityNamespace.GetStringValue();
            var all = await GetAllNamespacesAsync();
            var ns = all.Value.FirstOrDefault(sn => sn.Name.Equals(sv, StringComparison.OrdinalIgnoreCase) 
                        && sn.Actions.Any(ac => ac.Name.Equals(action, StringComparison.OrdinalIgnoreCase)));
            return ns;
        }

        

        public async Task<VstsSecurityNamespaceCollection> GetAllNamespacesAsync() 
        {
            var path = "_apis/securitynamespaces?api-version=6.0";
            var namespaces = await GetAzureDevOpsDefaultUri()
                .GetRestAsync<VstsSecurityNamespaceCollection>(path, await GetBearerTokenAsync());

            return namespaces;
        }

        public async Task<string> GenerateEnumerationsAsync(string codeNamespace)
        {
            var sb = new StringBuilder();
            var all = await GetAllNamespacesAsync();

            var distincts = new List<string>();
            var safeName = new Func<string, string>(name => 
            {
                var alterName = $"{name.Replace(" ", string.Empty)}{(distincts.Contains(name) ? "Ex" : string.Empty)}";
                distincts.Add(name);
                return alterName;
            });

            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.ComponentModel;");
            sb.AppendLine("using System.Text;");
            sb.AppendLine("using System.Runtime.InteropServices;");
            sb.AppendLine();
            sb.AppendLine($"namespace {codeNamespace}");
            sb.AppendLine("{");

            foreach (var item in all.Value)
            {
                sb.AppendLine($"\t[Guid(\"{item.NamespaceId}\")]");
                sb.AppendLine($"\t[Description(\"{item.DisplayName}\")]");
                sb.AppendLine($"\tpublic enum {safeName(item.Name)}");
                sb.AppendLine("\t{");
                for(var i =0; i < item.Actions.Length; ++ i)
                {
                    var action = item.Actions[i];
                    sb.AppendLine($"\t\t[Description(\"{action.DisplayName}\")]");
                    sb.AppendLine($"\t\t[DefaultValue({action.Bit})]");
                    sb.AppendLine($"\t\t{action.Name}{(i + 1 == item.Actions.Length ? string.Empty : ",")}");
                }                
                sb.AppendLine("\t}");
                sb.AppendLine();
            }
            sb.AppendLine("}");

            return sb.ToString();
        }

        #region Example methods
        public async Task<string> CreateAcrConnectionAsync(
            string projectName, string acrName, string name, string description,
            string subscriptionId, string subscriptionName, string resourceGroup,
            string clientId, string secret, string tenantId)
        {
            var response = await GetAzureDevOpsDefaultUri()
                .PostRestAsync(
                $"{projectName}/_apis/serviceendpoint/endpoints?api-version=5.1-preview.2",
                new
                {
                    name,
                    description,
                    type = "dockerregistry",
                    url = $"https://{acrName}.azurecr.io",
                    isShared = false,
                    owner = "library",
                    data = new
                    {
                        registryId = $"/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/Microsoft.ContainerRegistry/registries/{acrName}",
                        registrytype = "ACR",
                        subscriptionId,
                        subscriptionName
                    },
                    authorization = new
                    {
                        scheme = "ServicePrincipal",
                        parameters = new
                        {
                            loginServer = $"{acrName}.azurecr.io",
                            servicePrincipalId = clientId,
                            tenantId,
                            serviceprincipalkey = secret
                        }
                    }
                },
                await GetBearerTokenAsync());
            return response;
        }

        public async Task<string> CreateApprovalPolicyAsync(
            string projectName, Guid groupId, long envId,
            string instruction = "Please approve the Deployment")
        {
            var response = await GetAzureDevOpsDefaultUri()
                .PostRestAsync(
                $"{projectName}/_apis/pipelines/checks/configurations?api-version=5.2-preview.1",
                new
                {
                    timeout = 43200,
                    type = new
                    {
                        name = "Approval"
                    },
                    settings = new
                    {
                        executionOrder = 1,
                        instructions = instruction,
                        blockedApprovers = new List<object> { },
                        minRequiredApprovers = 0,
                        requesterCannotBeApprover = false,
                        approvers = new List<object>
                        {
                            new
                            {
                                id = groupId
                            }
                        }
                    },
                    resource = new
                    {
                        type = "environment",
                        id = envId.ToString()
                    }
                }, await GetBearerTokenAsync());
            return response;
        }

        public async Task<IEnumerable<VstsGroup>> ListGroupsAsync()
        {
            var groups = await GetAzureDevOpsVsspUri()
                .GetRestAsync<GroupCollection>(
                $"_apis/graph/groups?api-version=5.1-preview.1",
                await GetBearerTokenAsync());
            return groups.Value;
        }

        public async Task<string> CreateKubernetesResourceAsync(
            string projectName, long environmentId, Guid endpointId,
            string kubernetesNamespace, string kubernetesClusterName)
        {
            var link = await GetAzureDevOpsDefaultUri()
                            .PostRestAsync(
                            $"{projectName}/_apis/distributedtask/environments/{environmentId}/providers/kubernetes?api-version=5.0-preview.1",
                            new
                            {
                                name = kubernetesNamespace,
                                @namespace = kubernetesNamespace,
                                clusterName = kubernetesClusterName,
                                serviceEndpointId = endpointId
                            },
                            await GetBearerTokenAsync());
            return link;
        }

        public async Task<Endpoint> CreateKubernetesEndpointAsync(
            Guid projectId, string projectName,
            string endpointName, string endpointDescription,
            string clusterApiUri,
            string serviceAccountCertificate, string apiToken)
        {
            var ep = await GetAzureDevOpsDefaultUri()
                .PostRestAsync<Endpoint>(
                $"{projectName}/_apis/serviceendpoint/endpoints?api-version=6.0-preview.4",
                new
                {
                    authorization = new
                    {
                        parameters = new
                        {
                            serviceAccountCertificate,
                            isCreatedFromSecretYaml = true,
                            apitoken = apiToken
                        },
                        scheme = "Token"
                    },
                    data = new
                    {
                        authorizationType = "ServiceAccount"
                    },
                    isShared = false,
                    name = endpointName,
                    owner = "library",
                    type = "kubernetes",
                    url = clusterApiUri,
                    description = endpointDescription,
                    serviceEndpointProjectReferences = new List<Object>
                    {
                        new
                        {
                            description = endpointDescription,
                            name =  endpointName,
                            projectReference = new
                            {
                                id =  projectId,
                                name =  projectName
                            }
                        }
                    }
                },
                await GetBearerTokenAsync());
            return ep;
        }

        public async Task<PipelineEnvironment> CreateEnvironmentAsync(
            string project, string envName, string envDesc)
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

        public async Task<EndpointCollection> ListEndpointsAsync(string project)
        {
            var path = $"{project}/_apis/serviceendpoint/endpoints?api-version=5.1-preview.2";
            var types = await GetAzureDevOpsDefaultUri()
                .GetRestAsync<EndpointCollection>(path, await GetBearerTokenAsync());

            return types;
        }

        public async Task<string> GetEndpointTypesAsync()
        {
            var path = "_apis/serviceendpoint/types?api-version=5.1-preview.1";
            var types = await GetAzureDevOpsDefaultUri()
                .GetRestJsonAsync(path, await GetBearerTokenAsync());

            return types;
        }

        public async Task<ProjectCollection> GetProjectsAsync()
        {
            var path = "_apis/projects?stateFilter=All&api-version=1.0";
            var projects = await GetAzureDevOpsDefaultUri()
                .GetRestAsync<ProjectCollection>(path, await GetBearerTokenAsync());

            return projects;
        }

        public async Task<PipelineEnvironmentCollection> GetEnvAsync(string project)
        {
            var envs = await GetAzureDevOpsDefaultUri()
                .GetRestAsync<PipelineEnvironmentCollection>(
                $"{project}/_apis/distributedtask/environments",
                await GetBearerTokenAsync());

            return envs;
        }

        #endregion
    }
}
