using Kdoctl.CliServices.Abstract;
using Kdoctl.CliServices.AzDoServices.Dtos;
using Kdoctl.CliServices.Supports;
using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace Kdoctl.CliServices.AzDoServices
{
    public class ProjectService : RestServiceBase
    {
        private readonly TeamHttpClient teamClient;

        public ProjectService(TeamHttpClient teamClient, string adoUrl, string pat)
            : base(adoUrl, pat)
        {
            this.teamClient = teamClient;
        }

        public async Task<WebApiTeam> CreateTeamAsync(WebApiTeam team, Guid projectId)
        {
            return await teamClient.CreateTeamAsync(team, projectId.ToString());
        }

        public async Task<VstsTeamCollection> GetTeamsAsync()
        {
            var path = "_apis/teams?api-version=6.1-preview.3";
            var teams = await CoreApi()
                .GetRestAsync<VstsTeamCollection>(path, await GetBearerTokenAsync());

            return teams;
        }

        public async Task<ProjectCollection> GetProjectsAsync()
        {
            var path = "_apis/projects?stateFilter=All&api-version=1.0";
            var projects = await CoreApi()
                .GetRestAsync<ProjectCollection>(path, await GetBearerTokenAsync());

            return projects;
        }

        public async Task<ProcessTemplateCollection> ListProcessAsync()
        {
            var path = "_apis/process/processes?api-version=6.1-preview.1";
            var processes = await CoreApi()
                .GetRestAsync<ProcessTemplateCollection>(path, await GetBearerTokenAsync());

            return processes;
        }

        public async Task<string> CreateProjectAsync(
            string name, ProcessTemplate tempalte, 
            string sourceControlType, string description)
        {
            var response = await CoreApi()
                .PostRestAsync(
                $"_apis/projects?api-version=6.1-preview.4",
                new
                {
                    Name = name,
                    Description = description,
                    capabilities = new
                    {
                        versioncontrol = new
                        {
                            sourceControlType
                        },
                        processTemplate = new
                        {
                            templateTypeId = tempalte.Id
                        }
                    }
                },
                await GetBearerTokenAsync());
            return response;
        }
    }
}
