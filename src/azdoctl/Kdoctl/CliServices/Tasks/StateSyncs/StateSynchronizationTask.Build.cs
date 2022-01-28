

using Kdoctl.CliServices.AzDoServices;
using Kdoctl.CliServices.AzDoServices.Dtos;
using Kdoctl.CliServices.Constants;
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
        protected async Task EnsureBuildFoldersAsync(
            ProjectManifest manifest,            
            Kdoctl.CliServices.AzDoServices.Dtos.Project project)
        {
            if (manifest.BuildFolders != null && manifest.BuildFolders.Any())
            {
                var buildService = this.GetBuildService();
                var buildPaths = await buildService.ListFoldersAsync(project.Id);
                foreach (var bp in manifest.BuildFolders)
                {
                    var existingItem = buildPaths.Value
                        .FirstOrDefault(p => p.Path.Replace("\\", "/").Equals(bp.Path, StringComparison.OrdinalIgnoreCase));

                    if (existingItem == null)
                    {
                        existingItem = await buildService.CreateFolderAsync(project.Id, bp.Path);
                    }
                    using var op = Insights.BeginOperation($"Creating permissions {bp.Path}...", "Build-Folder-Permissions");
                    await ProvisionBuildPathPermissionsAsync( project, bp, existingItem);                    
                }
            }
        }

        protected async Task ProvisionBuildPathPermissionsAsync(
            
            Kdoctl.CliServices.AzDoServices.Dtos.Project project,
            PipelineFolder bp,
            VstsFolder existingItem)
        {
            if (existingItem != null)
            {
                var secService = GetSecurityNamespaceService();
                var buildNamespace = await secService.GetNamespaceAsync(SecurityNamespaceConstants.Build);
                var namespaceId = buildNamespace.NamespaceId;
                var aclDictioanry = new Dictionary<string, VstsAcesDictionaryEntry>();
                await CreateAclsAsync( typeof(Build), bp.Permissions, aclDictioanry);

                if (aclDictioanry.Count > 0)
                {
                    var fpath = bp.Path.TrimStart("\\/".ToCharArray()).Replace("\\", "/");
                    var token = $"{project.Id}/{fpath}";
                    var aclService = GetAclListService();
                    await aclService.SetAclsAsync(namespaceId, token, aclDictioanry, false);
                }
            }
        }
    }
}
