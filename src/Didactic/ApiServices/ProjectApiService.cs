

using Didactic.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var templates = await projectService.ListProcessAsync();

            if (manifest.Validate())
            {
                var tempalte = templates.Value.FirstOrDefault(t => t.Name.Equals(manifest.Template.Name, StringComparison.InvariantCulture));
                if (tempalte == null)
                {
                    throw new ArgumentOutOfRangeException($"Process template {manifest.Template.Name} is not valid! Good example: Agile, CMMI, Basic, Scrum etc.");
                }

                var projects = await projectService.GetProjectsAsync();

                if(!projects.Value.Any(p=> p.Name.Equals(manifest.Metadata.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    await projectService.CreateProjectAsync(
                        manifest.Metadata.Name, tempalte,
                        manifest.Template.SourceControlType, manifest.Metadata.Description);
                }
            }

            await Task.CompletedTask;
        }
    }
}
