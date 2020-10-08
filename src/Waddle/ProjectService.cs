using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Waddle.Dtos;
using Waddle.Supports;

namespace Waddle
{
    public class ProjectService : RestServiceBase
    {
        public ProjectService(string adoUrl, string pat)
            : base(adoUrl, pat)
        {

        }

        public async Task<ProjectCollection> GetProjectsAsync()
        {
            var path = "_apis/projects?stateFilter=All&api-version=1.0";
            var projects = await GetAzureDevOpsDefaultUri()
                .GetRestAsync<ProjectCollection>(path, await GetBearerTokenAsync());

            return projects;
        }

        public async Task<ProcessTemplateCollection> ListProcessAsync()
        {
            var path = "_apis/process/processes?api-version=6.1-preview.1";
            var processes = await GetAzureDevOpsDefaultUri()
                .GetRestAsync<ProcessTemplateCollection>(path, await GetBearerTokenAsync());

            return processes;
        }

        public async Task<string> CreateProjectAsync(
            string name, ProcessTemplate tempalte, 
            string sourceControlType, string description)
        {
            var response = await GetAzureDevOpsDefaultUri()
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
                            sourceControlType = sourceControlType
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
