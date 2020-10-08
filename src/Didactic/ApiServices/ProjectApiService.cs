

using Didactic.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waddle.Constants;
using Waddle.Dtos;
using Waddle.Supports;

namespace Didactic.ApiServices
{
    public class ProjectApiService : BaseApiService
    {
        public ProjectApiService(string orgUri, string pat) : base(orgUri, pat)
        {
        }

        protected async override Task ExecuteCoreAsync(BaseSchema baseSchema, string manifestContent)
        {
            var manifest = Deserialize<ProjectManifest>(manifestContent);
            var factory = base.Factory;
            var projectService = factory.GetProjectService();
            var repoService = factory.GetRepositoryService();
            var templates = await projectService.ListProcessAsync();

            if (manifest.Validate())
            {
                var tempalte = templates.Value.FirstOrDefault(t => t.Name.Equals(manifest.Template.Name, StringComparison.InvariantCulture));
                if (tempalte == null)
                {
                    throw new ArgumentOutOfRangeException($"Process template {manifest.Template.Name} is not valid! Good example: Agile, CMMI, Basic, Scrum etc.");
                }

                var projects = await projectService.GetProjectsAsync();
                var project = projects.Value.FirstOrDefault(p => p.Name.Equals(manifest.Metadata.Name, 
                    StringComparison.OrdinalIgnoreCase));

                if (project == null)
                {
                    await projectService.CreateProjectAsync(
                        manifest.Metadata.Name, tempalte,
                        manifest.Template.SourceControlType, manifest.Metadata.Description);
                    await Task.Delay(5000);
                    project = projects.Value.FirstOrDefault(p => p.Name.Equals(manifest.Metadata.Name,
                        StringComparison.OrdinalIgnoreCase));
                }

                if (project != null && manifest.Repositories != null && manifest.Repositories.Any())
                {
                    foreach(var repo in manifest.Repositories)
                    {
                        if(!string.IsNullOrWhiteSpace(repo.Name))
                        {
                            var reposCollection = await repoService.GetRepositoryListAsync(project.Id);
                            var repository = reposCollection
                                .FirstOrDefault(r => r.Name.Equals(repo.Name, StringComparison.OrdinalIgnoreCase));

                            if (repository == null)
                            {
                                repository = await repoService.CreateAsync(project.Id, repo.Name);
                            }

                            if(repository != null && repo.Permissions != null && repo.Permissions.Any())
                            {
                                var secService = factory.GetSecurityNamespaceService();
                                var gitNamespace = await secService.GetNamespaceAsync(SecurityNamespaceConstants.Git_Repositories);
                                var gitSecurityNamespaceId = gitNamespace.NamespaceId;
                                var aclDictioanry = new Dictionary<string, VstsAcesDictionaryEntry>();

                                foreach (var permission in repo.Permissions)
                                {
                                    if(permission.Allowed != null && permission.Allowed.Any())
                                    {                                        
                                        var groups = await factory.GetGrouoService().ListGroupsAsync();
                                        var group = groups.Value
                                            .FirstOrDefault(g => 
                                            g.Origin.ToString().Equals(permission.Origin, StringComparison.OrdinalIgnoreCase) && 
                                            g.PrincipalName.Contains(permission.Group, StringComparison.OrdinalIgnoreCase));

                                        if (group != null)
                                        {
                                            var bitMask = 0;
                                            foreach(var permissionItem in permission.Allowed)
                                            {
                                                bitMask |= EnumSupport.GetBitMaskValue(typeof(GitRepositories), permissionItem);
                                            }
                                            aclDictioanry.Add(group.Sid, new VstsAcesDictionaryEntry
                                            {
                                                Descriptor = group.Sid,
                                                Allow = bitMask,
                                                Deny = 0
                                            });
                                        }
                                    }
                                }

                                if (aclDictioanry.Count > 0)
                                {
                                    var repositorySecurityToken = $"repoV2/{project.Id}/{repository.Id}";
                                    var aclService = factory.GetAclListService();
                                    await aclService.SetAclsAsync(gitSecurityNamespaceId, repositorySecurityToken, aclDictioanry, false);
                                }
                            }
                        }
                    }
                }
            }

            await Task.CompletedTask;
        }
    }
}
