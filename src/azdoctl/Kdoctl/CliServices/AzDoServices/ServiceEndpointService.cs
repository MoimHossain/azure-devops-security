
using Kdoctl.CliServices.Abstract;
using Kdoctl.CliServices.AzDoServices.Dtos;
using Kdoctl.CliServices.Supports;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace Kdoctl.CliServices.AzDoServices
{
    public class ServiceEndpointService : RestServiceBase
    {
        public ServiceEndpointService(IHttpClientFactory clientFactory)
            : base(clientFactory)
        {

        }

        public async Task<VstsServiceEndpointCollection> ListServiceEndpointsAsync(Guid projectId)
        {
            var requestPath = $"{projectId}/_apis/serviceendpoint/endpoints?api-version=6.1-preview.4";
            var endpoints = await CoreApi()
                .GetRestAsync<VstsServiceEndpointCollection>(requestPath);

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
            var response = await CoreApi()
                .PutRestAsync(
                $"_apis/securityroles/scopes/distributedtask.serviceendpointrole/roleassignments/resources/{projectId}_{endpointId}?api-version=5.0-preview.1",
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

        public async Task<VstsServiceEndpointAccessControlCollection> SetPermissionCollectionLevelAsync(
            Guid endpointId,
            Guid localId)
        {
            var response = await CoreApi()
                .PutRestAsync(
                $"_apis/securityroles/scopes/distributedtask.serviceendpointrole/roleassignments/resources/collection_{endpointId}?api-version=5.0-preview.1",
                new List<object>
                {
                    new
                    {
                        roleName = ServiceEndpointPermissions.Administrator.ToString(), // only admin supported for this level
                        userId = localId
                    }
                });
            return JsonConvert.DeserializeObject<VstsServiceEndpointAccessControlCollection>(response);
        }

        public async Task<string> CreateKubernetesResourceAsync(
            string projectName, long environmentId, Guid endpointId,
            string kubernetesNamespace, string kubernetesClusterName)
        {
            var link = await CoreApi()
                            .PostRestAsync(
                            $"{projectName}/_apis/distributedtask/environments/{environmentId}/providers/kubernetes?api-version=5.0-preview.1",
                            new
                            {
                                name = kubernetesNamespace,
                                @namespace = kubernetesNamespace,
                                clusterName = kubernetesClusterName,
                                serviceEndpointId = endpointId
                            });
            return link;
        }

        public async Task<Endpoint> CreateKubernetesEndpointAsync(
            Guid projectId, string projectName,
            string endpointName, string endpointDescription,
            string clusterApiUri,
            string serviceAccountCertificate, string apiToken)
        {
            var ep = await CoreApi()
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
                        authorizationType = "ServiceAccount",
                        acceptUntrustedCerts = true
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
                });
            return ep;
        }


        public async Task<Endpoint> UpdateKubernetesEndpointAsync(
            Guid endpointId,
            Guid projectId, string projectName,
            string endpointName, string endpointDescription,
            string clusterApiUri,
            string serviceAccountCertificate, string apiToken)
        {
            var ep = await CoreApi()
                .PutRestAsync(
                $"{projectName}/_apis/serviceendpoint/endpoints/{endpointId}?api-version=6.0-preview.4",
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
                        authorizationType = "ServiceAccount",
                        acceptUntrustedCerts = true
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
                });

            var endpoint = JsonConvert.DeserializeObject<Endpoint>(ep);
            return endpoint;
        }
    }
}