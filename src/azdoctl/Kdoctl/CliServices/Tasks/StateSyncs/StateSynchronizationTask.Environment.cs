

using Kdoctl.CliServices.AzDoServices;
using Kdoctl.CliServices.AzDoServices.Dtos;
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
        protected async Task ProvisionEnvironmentPermissionsAsync(

            Kdoctl.CliServices.AzDoServices.Dtos.Project project,
            PipelineEnvironmentService peService,
            EnvironmentManifest pe, PipelineEnvironment envObject)
        {
            if (envObject != null && pe.Permissions != null && pe.Permissions.Any())
            {
                foreach (var permissionObject in pe.Permissions)
                {
                    using var op = Insights.BeginOperation($"Configuring Environment ({pe.Name}) permissions: AAD object ({permissionObject.Group}) ...", "Envrionment");
                    var group = await GetGroupByNameAsync(
                        permissionObject.Origin, permissionObject.Group, permissionObject.Id);
                    if (group != null)
                    {
                        var legacyIdentity = await GetGraphService()
                            .GetLegacyIdentitiesBySidAsync(group.Sid);
                        if (legacyIdentity != null && legacyIdentity.Value.Any())
                        {
                            var localId = legacyIdentity.Value.First().Id;
                            foreach (var role in permissionObject.Roles)
                            {
                                await peService.SetPermissionAsync(project.Id, envObject.Id, localId, role);
                            }
                        }
                    }
                    else
                    {
                        op.EndWithFailure("Failed (Not found in AAD)");
                    }
                }
            }
        }
        protected async Task ProvisionEnvironmentAsync(
            Kdoctl.CliServices.AzDoServices.Dtos.Project project,
            PipelineEnvironmentService peService, EnvironmentManifest pe,
            string k8sNamespace, string k8sClusterName)
        {
            var seService = GetServiceEndpointService();
            var peColl = await peService.ListEnvironmentsAsync(project.Id);
            if (peColl != null)
            {
                var envObject = peColl.Value
                     .FirstOrDefault(penv => penv.Name.Equals(pe.Name, StringComparison.OrdinalIgnoreCase));
                if (envObject == null)
                {
                    envObject = await peService.CreateEnvironmentAsync(project.Id, pe.Name, pe.Description);
                    if (!string.IsNullOrWhiteSpace(pe.ServiceEndpointReference))
                    {
                        var seColl = await seService.ListServiceEndpointsAsync(project.Id);
                        if (seColl != null && seColl.Value != null)
                        {
                            var foundSe = seColl.Value
                                .FirstOrDefault(s => s.Name
                                .Equals(pe.ServiceEndpointReference, StringComparison.OrdinalIgnoreCase));

                            if (foundSe != null)
                            {
                                await seService.CreateKubernetesResourceAsync(
                                    project.Id.ToString(),
                                    envObject.Id,
                                    foundSe.Id, k8sNamespace, k8sClusterName);
                            }
                        }
                    }
                }
                await ProvisionEnvironmentPermissionsAsync(project, peService, pe, envObject);
            }
        }
        protected async Task EnsureEnvironmentExistsAsync(
            ProjectManifest manifest,

            Kdoctl.CliServices.AzDoServices.Dtos.Project project,
            List<Tuple<k8s.Models.V1ServiceAccount>> k8sOutcome)
        {
            if (project != null && manifest.Environments != null && manifest.Environments.Any())
            {
                var peService = GetPipelineEnvironmentService();
                foreach (var pe in manifest.Environments)
                {
                    if (pe != null && !string.IsNullOrWhiteSpace(pe.Name) && k8sOutcome != null && k8sOutcome.Count > 0)
                    {
                        var item = k8sOutcome.FirstOrDefault().Item1;
                        if (item != null)
                        {
                            await ProvisionEnvironmentAsync(project, peService, pe,
                                item.Metadata.NamespaceProperty, item.Metadata.NamespaceProperty);
                        }
                    }
                }
            }
        }
    }
}
