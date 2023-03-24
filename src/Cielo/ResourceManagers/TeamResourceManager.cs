

using Cielo.Azdo;
using Cielo.Azdo.Dtos;
using Cielo.Manifests;
using Cielo.ResourceManagers.Abstract;
using Cielo.ResourceManagers.ResourceStates;
using Microsoft.Extensions.DependencyInjection;
using static Cielo.Azdo.SecurityNamespaceService;

namespace Cielo.ResourceManagers
{
    public class TeamResourceManager : ResourceManagerBase
    {
        private readonly TeamService teamService;
        private readonly SecurityNamespaceService sercurityNamespaceService;
        private readonly GraphService graphService;
        private AclService aclService;

        public TeamResourceManager(IServiceProvider serviceProvider, string rawManifest) 
            : base(serviceProvider, rawManifest)
        {
            this.teamService = serviceProvider.GetRequiredService<TeamService>();
            this.sercurityNamespaceService = serviceProvider.GetRequiredService<SecurityNamespaceService>();
            this.graphService = serviceProvider.GetRequiredService<GraphService>();
            this.aclService = serviceProvider.GetRequiredService<AclService>();
        }

        protected async override Task<ResourceState> CreateAsync()
        {
            var project = Context.CurrentProject;
            var teamName = this.TeamManifest.Metadata.Name;
            var description = this.TeamManifest.Metadata.Description;
            if (project == null)
            {
                var errorState = new ResourceState();
                errorState.AddError($"Required Project context is missing for resource {TeamManifest.Kind}:{teamName}");
                return errorState;
            }

            var state = new ResourceState();
            if (string.IsNullOrEmpty(teamName) || this.TeamManifest == null)
            {
                state.AddError("Team name is empty");
                return state;
            }

            var team = await teamService.CreateTeamAsync(
                new Microsoft.TeamFoundation.Core.WebApi.WebApiTeam 
                {
                    Name = teamName,
                    Description = TeamManifest.Metadata.Description,
                    ProjectId = project.Id,
                    ProjectName = project.Name
                }, project.Id);

            if (team != null)
            {
                state.AddProperty("IdentityUrl", team.IdentityUrl);

                await UpdateAdminsAsync(team, state, project);
                await UpdateMembersAsync(team, state, project);
            }
            return state;
        }

        private async Task UpdateMembersAsync(VstsTeam team, ResourceState state, Project project)
        {
            await Task.CompletedTask;
        }

        protected async override Task<ResourceState> GetAsync()
        {
            var project = Context.CurrentProject;
            var teamName = TeamManifest.Metadata.Name;
            if (project == null)
            {
                var errorState = new ResourceState();
                errorState.AddError($"Required Project context is missing for resource {TeamManifest.Kind}:{teamName}");
                return errorState;
            }

            var state = new ResourceState();
            if (string.IsNullOrEmpty(teamName) || TeamManifest == null)
            {
                state.AddError("Team name is empty");
                return state;
            }

            var team = await teamService.GetTeamByNameAsync(project.Id, teamName);
            state.AddProperty("Name", teamName);
            state.AddProperty("Description", TeamManifest.Metadata.Description);

            if (team == null)
            {
                state.Exists = false;
            }
            else
            {
                state.Exists = true;
                if (TeamManifest.Admins != null)
                {
                    await DiscoverAdminsAsync(team, state, project, TeamManifest.Admins);
                }
               
                await DiscoverMembershipAsync(team, state, project);
            }
            return state;
        }

        private async Task DiscoverAdminsAsync(
            VstsTeam team, ResourceState state,
            Project project, List<TeamManifest.UserReference> expectedAdmins)
        {
            var currentAdmins = await teamService.GetTeamAdminsAsync(project.Id, team.Id);

            if (currentAdmins != null)
            {
                foreach (var expectedAdmin in expectedAdmins)
                {
                    var exists = currentAdmins.Exists(ca => ca.UniqueName.Equals(expectedAdmin.Principal, StringComparison.OrdinalIgnoreCase));
                    if (!exists)
                    {
                        state.AddProperty($"{team.Name} Admins", $"{expectedAdmin.Principal} will be added as Admin", true);
                    }
                }
            }
        }

        private async Task UpdateAdminsAsync(VstsTeam team, ResourceState state, Project project)
        {
            var expectedAdmins = TeamManifest.Admins;
            if(expectedAdmins != null && expectedAdmins.Count > 0)
            {
                var token = $"{project.Id}\\{team.Id}";
                var securityNamespace = await sercurityNamespaceService.GetNamespaceAsync(SecurityNamespaceConstants.Identity);
                var secNamespaceId = securityNamespace.NamespaceId;
                var aclDictioanry = new Dictionary<string, VstsAcesDictionaryEntry>();

                var currentAdmins = await teamService.GetTeamAdminsAsync(project.Id, team.Id);

                if (currentAdmins != null)
                {
                    foreach (var expectedAdmin in expectedAdmins)
                    {
                        var loadedUserObject = await graphService.GetUserByPrincipalNameAsync(expectedAdmin.Principal);
                        if (loadedUserObject != null)
                        {
                            var securityDescriptor = graphService.GetSecurityDescriptorForUser(loadedUserObject);
                            aclDictioanry.Add(securityDescriptor, new VstsAcesDictionaryEntry
                            {
                                Allow = 31,
                                Deny = 0,
                                Descriptor = securityDescriptor
                            });
                        }
                        else
                        {
                            state.AddError($"Failed to find user {expectedAdmin.Principal}");
                        }
                    }

                    var result = await aclService.SetAclsAsync(secNamespaceId, token, aclDictioanry, false);
                    if(!result)
                    {
                        state.AddError($"Couldn't update Tean Admins");
                    }
                }
            }
        }

        private async Task DiscoverMembershipAsync(VstsTeam team, ResourceState state, Project project)
        {
            //
        }

        protected async override Task<ResourceState?> UpdateAsync()
        {
            var project = Context.CurrentProject;
            var teamName = TeamManifest.Metadata.Name;
            if (project == null)
            {
                var errorState = new ResourceState();
                errorState.AddError($"Required Project context is missing for resource {TeamManifest.Kind}:{teamName}");
                return errorState;
            }

            var state = new ResourceState();
            if (string.IsNullOrEmpty(teamName) || TeamManifest == null )
            {
                state.AddError("Team name is empty");
                return state;
            }

            var team = await teamService.GetTeamByNameAsync(project.Id, teamName);

            if (team != null)
            {
                await UpdateAdminsAsync(team, state, project);
                await UpdateMembersAsync(team, state, project);
            }

            return state;
        }

        protected override Type GetResourceType()
        {
            return typeof(TeamManifest);
        }

        private TeamManifest TeamManifest { get { return (TeamManifest)this.Manifest; } }
    }
}
