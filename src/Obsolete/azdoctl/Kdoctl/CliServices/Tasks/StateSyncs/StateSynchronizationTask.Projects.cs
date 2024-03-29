﻿

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
            var project = await projectService.GetProjectByIdOrNameAsync(manifest.Metadata.Name);
            
            if (project == null)
            {
                using var op = Insights.BeginOperation("Creating project, reading Process templates...", "Project");
                var templates = await projectService.ListProcessAsync();
                var tempalte = templates.Value.FirstOrDefault(t => t.Name.Equals(manifest.Template.Name, StringComparison.InvariantCulture));
                if (tempalte == null)
                {
                    op.EndWithFailure($"Process template {manifest.Template.Name} is not valid! Good example: Agile, CMMI, Basic, Scrum etc.");
                    throw new InvalidOperationException($"Process template {manifest.Template.Name} is not valid! Good example: Agile, CMMI, Basic, Scrum etc.");
                }

                await projectService.CreateProjectAsync(
                    manifest.Metadata.Name, tempalte,
                    manifest.Template.SourceControlType, manifest.Metadata.Description);
                projectCreatedJIT = true;                
                while (project == null)
                {
                    project = await projectService.GetProjectByIdOrNameAsync(manifest.Metadata.Name);                    
                }                
            }

            await projectService.UpdateRetentionAsync(project.Id, new ProjectRetentionSetting
            {
                ArtifactsRetention = new UpdateRetentionSettingSchema { Value = 15 },
                PullRequestRunRetention = new UpdateRetentionSettingSchema { Value = 12 },
                RetainRunsPerProtectedBranch = new UpdateRetentionSettingSchema { Value = 4 },
                RunRetention = new UpdateRetentionSettingSchema { Value = 12 },
            });

            return new Tuple<Kdoctl.CliServices.AzDoServices.Dtos.Project, bool>(project, projectCreatedJIT);
        }
    }
}
