
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
                Context.CurrentProject = project;

                state.AddProperty("Project Id", project.Id);
                state.AddProperty("Project Name", project.Name);

                var properties = await projectService.GetProjectPropertiesAsync(project.Id);
                if (properties != null && properties.Value != null )
                {
                    foreach( var property in properties.Value.Where(v=> !v.Name.Equals("System.MSPROJ", StringComparison.OrdinalIgnoreCase)))
                    {
                        state.AddProperty(property.Name, property.Value);
                    }
                }
            }
            else
            {
                state.AddProperty("Project Name", projectName);
            }
            return state;
        }

        protected override async Task<ResourceState> CreateAsync()
        {
            await Task.CompletedTask;
            return new ResourceState() { Exists = true };
        }

        protected override async Task<ResourceState?> UpdateAsync()
        {
            await Task.CompletedTask;
            return new ResourceState() { Exists = true, Changed = false };
        }

        protected override Type GetResourceType()
        {
            return typeof(ProjectManifest);
        }

        protected override void SetContextData()
        {
            this.Context.CurrentProjectManifest = this.ProjectManifest;
        }

        private ProjectManifest ProjectManifest { get { return (ProjectManifest)this.Manifest; } }
    }
}
