
using Cielo.Azdo;
using Cielo.Manifests;
using Cielo.ResourceManagers.Abstract;
using Cielo.ResourceManagers.ResourceStates;
using Microsoft.Extensions.DependencyInjection;

namespace Cielo.ResourceManagers
{
    public class ProjectResourceManager : ResourceManagerBase
    {
        private readonly ProjectService projectService;

        public ProjectResourceManager(IServiceProvider serviceProvider, string rawManifest) 
            : base(serviceProvider, rawManifest) 
        {
            this.projectService = serviceProvider.GetRequiredService<ProjectService>();
        }

        protected override async Task<ResourceState> GetAsync()
        {
            var state = new ResourceState();
            var projectName = ProjectManifest.Metadata.Name;

            var project = await projectService.GetProjectByIdOrNameAsync(projectName);
            state.Exists = (project != null);

            if(project != null)
            {
                state.Properties.Add("Project Id", project.Id);
                state.Properties.Add("Project Name", project.Name);
            }
            else
            {
                state.Properties.Add("Project Name", projectName);
            }

            return state;
        }

        protected override Type GetResourceType()
        {
            return typeof(ProjectManifest);
        }

        private ProjectManifest ProjectManifest { get { return (ProjectManifest)this.Manifest; } }
    }
}
