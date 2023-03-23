using Cielo.Azdo;
using Cielo.Azdo.Dtos;
using Cielo.Manifests;
using Cielo.ResourceManagers.Abstract;
using Cielo.ResourceManagers.ResourceStates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cielo.ResourceManagers
{
    public class TeamResourceManager : ResourceManagerBase
    {
        private readonly TeamService teamService;

        public TeamResourceManager(IServiceProvider serviceProvider, string rawManifest) 
            : base(serviceProvider, rawManifest)
        {
            this.teamService = serviceProvider.GetRequiredService<TeamService>();
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

        private async Task UpdateAdminsAsync(VstsTeam team, ResourceState state, Project project)
        {
            await Task.CompletedTask;
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

                await DiscoverAdminsAsync(team, state, project);
                await DiscoverMembershipAsync(team, state, project);
            }
            return state;
        }

        private async Task DiscoverAdminsAsync(VstsTeam team, ResourceState state, Project project)
        {
            var admins = await teamService.GetTeamAdminsAsync(project.Id, team.Id);
        
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
