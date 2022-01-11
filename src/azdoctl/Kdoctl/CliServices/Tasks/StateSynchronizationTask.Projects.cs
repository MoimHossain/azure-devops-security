

using Kdoctl.CliServices.AzDoServices;
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
        protected async Task<Tuple<Kdoctl.CliServices.AzDoServices.Dtos.Project, bool>> EnsureProjectExistsAsync(
            ProjectManifest manifest, ProjectService projectService)
        {
            var projectCreatedJIT = false;
            var projects = await projectService.ListProjectsAsync();
            var project = projects.Value.FirstOrDefault(p => p.Name.Equals(manifest.Metadata.Name,
                StringComparison.OrdinalIgnoreCase));

            if (project == null)
            {
                Logger.StatusBegin("Creating project, reading Process templates...");
                var templates = await projectService.ListProcessAsync();
                var tempalte = templates.Value.FirstOrDefault(t => t.Name.Equals(manifest.Template.Name, StringComparison.InvariantCulture));
                if (tempalte == null)
                {
                    Logger.StatusEndFailed("Failed");
                    throw new InvalidOperationException($"Process template {manifest.Template.Name} is not valid! Good example: Agile, CMMI, Basic, Scrum etc.");
                }
                else
                {
                    Logger.StatusEndSuccess("Succeed");
                }

                await projectService.CreateProjectAsync(
                    manifest.Metadata.Name, tempalte,
                    manifest.Template.SourceControlType, manifest.Metadata.Description);
                projectCreatedJIT = true;
                Logger.StatusBegin("Waiting on project creation...");
                while (project == null)
                {
                    projects = await projectService.ListProjectsAsync();
                    project = projects.Value.FirstOrDefault(p => p.Name.Equals(manifest.Metadata.Name,
                                                StringComparison.OrdinalIgnoreCase));
                }
                Logger.StatusEndSuccess("Succeed");
            }
            else
            {
                Logger.Message($"{project.Name} already exists...");
            }

            //await projectService.UpdateRetentionAsync(project.Id, new ProjectRetentionSetting 
            //{
            //    ArtifactsRetention = new UpdateRetentionSettingSchema { Value = 15 },
            //    PullRequestRunRetention = new UpdateRetentionSettingSchema { Value = 12 },
            //    RetainRunsPerProtectedBranch = new UpdateRetentionSettingSchema { Value = 4 },
            //    RunRetention = new UpdateRetentionSettingSchema { Value = 12 },
            //});

            return new Tuple<Kdoctl.CliServices.AzDoServices.Dtos.Project, bool>(project, projectCreatedJIT);
        }
    }
}
