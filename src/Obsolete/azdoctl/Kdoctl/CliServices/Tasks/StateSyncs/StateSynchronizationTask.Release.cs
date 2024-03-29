﻿

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
        protected async Task EnsureReleaseFoldersAsync(ProjectManifest manifest,
           Kdoctl.CliServices.AzDoServices.Dtos.Project project)
        {
            if (manifest.ReleaseFolders != null && manifest.ReleaseFolders.Any())
            {
                var releaseService = GetReleaseService();
                var releasePaths = await releaseService.ListFoldersAsync(project.Id);
                foreach (var rp in manifest.ReleaseFolders)
                {
                    var existingItem = releasePaths.Value
                        .FirstOrDefault(p => p.Path.Replace("\\", "/").Equals(rp.Path, StringComparison.OrdinalIgnoreCase));

                    if (existingItem == null)
                    {
                        existingItem = await releaseService.CreateFolderAsync(project.Id, rp.Path);
                    }
                    using var op  = Insights.BeginOperation($"Creating permissions {rp.Path}...", "ReleaseFolderPermissions");
                    await ProvisionReleasePathPermissionsAsync( project, rp, existingItem);                    
                }
            }
        }

        protected async Task ProvisionReleasePathPermissionsAsync(             
             Kdoctl.CliServices.AzDoServices.Dtos.Project project, PipelineFolder rp, VstsFolder existingItem)
        {
            if (existingItem != null)
            {
                var secService = GetSecurityNamespaceService();
                var buildNamespace = await secService.GetNamespaceAsync(SecurityNamespaceConstants.ReleaseManagement,
                    ReleaseManagementEx.AdministerReleasePermissions.ToString());
                var namespaceId = buildNamespace.NamespaceId;
                var aclDictioanry = new Dictionary<string, VstsAcesDictionaryEntry>();
                await CreateAclsAsync( typeof(ReleaseManagementEx), rp.Permissions, aclDictioanry);

                if (aclDictioanry.Count > 0)
                {
                    var fpath = rp.Path.TrimStart("\\/".ToCharArray()).Replace("\\", "/");
                    var token = $"{project.Id}/{fpath}";
                    var aclService = GetAclListService();
                    await aclService.SetAclsAsync(namespaceId, token, aclDictioanry, false);
                }
            }
        }
    }
}
