

using Kdoctl.CliServices.AzDoServices;
using Kdoctl.CliServices.AzDoServices.Dtos;
using Kdoctl.CliServices.Constants;
using Kdoctl.CliServices.K8sServices;
using Kdoctl.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kdoctl.CliServices
{
    public partial class StateSynchronizationTask
    {
        protected async Task<List<Tuple<k8s.Models.V1ServiceAccount>>> EnsureServiceEndpointExistsAsync(
            ProjectManifest manifest,
            ProjectService projectService,            
            Tuple<Kdoctl.CliServices.AzDoServices.Dtos.Project, bool> outcome)
        {
            List<Tuple<k8s.Models.V1ServiceAccount>> producedK8sInfo = new List<Tuple<k8s.Models.V1ServiceAccount>>();
            if (manifest.ServiceEndpoints != null)
            {
                foreach (var seManifest in manifest.ServiceEndpoints)
                {
                    if (!string.IsNullOrWhiteSpace(seManifest.Name))
                    {
                        if (seManifest.Kind == ServiceEndpointKind.Kubernetes &&
                            seManifest.ClusterInfo != null)
                        {
                            // only supporting Kubernetes for now
                            if (seManifest.ClusterInfo.Namespace != null &&
                                seManifest.ClusterInfo.ServiceAccount != null &&
                                seManifest.ClusterInfo.Namespace.Metadata != null &&
                                seManifest.ClusterInfo.ServiceAccount.Spec != null &&
                                seManifest.ClusterInfo.ServiceAccount.Spec.Metadata != null &&
                                seManifest.ClusterInfo.ServiceAccount.Binding != null &&
                                seManifest.ClusterInfo.ServiceAccount.Role != null &&
                                !string.IsNullOrWhiteSpace(seManifest.ClusterInfo.Namespace.Metadata.Name) &&
                                !string.IsNullOrWhiteSpace(seManifest.ClusterInfo.ServiceAccount.Spec.Metadata.Name))
                            {
                                //Logger.StatusBegin($"Creating Service endpoint '{seManifest.Name}' to deploy in Kubernetes namespace..");

                                await K8sService.Cluster
                                    .EnsureNamespaceExistsAsync(seManifest.ClusterInfo, Logger);
                                var sa = await K8sService.Cluster
                                    .EnsureServiceAccountExists(seManifest.ClusterInfo, Logger);
                                if (sa != null)
                                {
                                    var secret = await K8sService.Cluster.GetSecretAsync(sa);

                                    await EnsureServiceEndpointForKubernetesAsync(manifest, seManifest,  projectService, secret, outcome);

                                    producedK8sInfo.Add(new Tuple<k8s.Models.V1ServiceAccount>(sa));
                                }
                            }
                        }
                    }
                }
            }
            return producedK8sInfo;
        }

        protected async Task EnsureServiceEndpointForKubernetesAsync(
            ProjectManifest manifest,
            ServiceEndpointManifest seManifest,
            
            ProjectService projectService,
            k8s.Models.V1Secret secret,
            Tuple<Kdoctl.CliServices.AzDoServices.Dtos.Project, bool> outcome)
        {
            var seService = GetServiceEndpointService();
            var project = outcome.Item1;

            var eps = await seService.ListServiceEndpointsAsync(project.Id);
            if (eps != null && eps.Value != null)
            {
                var endpoint = eps.Value.FirstOrDefault(ep => ep.Name.Equals(seManifest.Name, StringComparison.OrdinalIgnoreCase));
                if (endpoint == null)
                {
                    Logger.StatusBegin($"Creating Kubernetes Service Endpoint '{seManifest.Name}' ...");
                    var newEndpoint = await seService.CreateKubernetesEndpointAsync(
                        project.Id, project.Name,
                        seManifest.Name, seManifest.Description,
                        K8sService.Cluster.GetClusterAPIUri().ToString(),
                        Convert.ToBase64String(secret.Data["ca.crt"]),
                        Convert.ToBase64String(secret.Data["token"]));

                    if (newEndpoint != null) { Logger.StatusEndSuccess("Succeed"); } else { Logger.StatusEndFailed("Failed"); }
                }
                else
                {
                    Logger.StatusBegin($"Updating Kubernetes Service Endpoint '{seManifest.Name}' ...");
                    var updatedEndpoint = await seService.UpdateKubernetesEndpointAsync(
                         endpoint.Id,
                         project.Id, project.Name,
                         seManifest.Name, seManifest.Description,
                         K8sService.Cluster.GetClusterAPIUri().ToString(),
                         Convert.ToBase64String(secret.Data["ca.crt"]),
                         Convert.ToBase64String(secret.Data["token"]));


                    if (updatedEndpoint != null) { Logger.StatusEndSuccess("Succeed"); } else { Logger.StatusEndFailed("Failed"); }
                }
            }
        }
    }
}
