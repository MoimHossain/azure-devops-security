
using Kdoctl.CliServices.Controllers.Abstract;
using Kdoctl.Schema;
using System;
using System.Threading.Tasks;

namespace Kdoctl.CliServices.Controllers.Project
{
    public class ProjectController : ControllerBase<ProjectManifest>
    {
        public ProjectController(ProjectManifest resourceManifest) : base(resourceManifest)
        {
        }

        public override Task CreateAsync()
        {
            throw new NotImplementedException();
        }

        public override Task DeleteAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<ProjectManifest> GetAsync()
        {
            throw new NotImplementedException();
        }

        public override Task UpdateAsync()
        {
            throw new NotImplementedException();
        }
    }
}
