


using Kdoctl.CliServices.Abstract;
using Kdoctl.CliServices.AzDoServices.Dtos;
using Kdoctl.CliServices.Supports;
using Kdoctl.Schema;
using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Net.Http;
using System.Threading.Tasks;


namespace Kdoctl.CliServices.AzDoServices
{
    public class ProjectService : RestServiceBase
    {
        private readonly TeamHttpClient teamClient;

        public ProjectService(TeamHttpClient teamClient, IHttpClientFactory clientFactory) : base(clientFactory)
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
                .GetRestAsync<VstsTeamCollection>(path);

            return teams;
        }

        public async Task<string> UpdateRetentionAsync(Guid projectId, ProjectRetentionSetting settings)
        {
            var response = await CoreApi()
               .PatchRestAsync(
               $"{projectId}/_apis/build/retention?api-version=6.0-preview.1",
               settings);

            return response;
        }

        public async Task<ProjectCollection> ListProjectsAsync()
        {
            var path = "_apis/projects?stateFilter=All&api-version=1.0";
            var projects = await CoreApi()
                .GetRestAsync<ProjectCollection>(path);

            return projects;
        }

        public async Task<ProcessTemplateCollection> ListProcessAsync()
        {
            var path = "_apis/process/processes?api-version=6.1-preview.1";
            var processes = await CoreApi()
                .GetRestAsync<ProcessTemplateCollection>(path);
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
                });
            return response;
        }
    }
}
