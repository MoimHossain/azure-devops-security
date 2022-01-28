

using Kdoctl.CliOptions;
using Kdoctl.CliServices.Supports;
using Kdoctl.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kdoctl.CliServices.Tasks
{
    public partial class ExportTask : TaskBase
    {
        private readonly ExportOptions opts;
        private readonly ExportFileSystem fs;

        public ExportTask(IServiceProvider services, ExportOptions opts) : base(services)
        {
            this.opts = opts;
            this.fs = new ExportFileSystem(opts, base.Insights);
        }

        protected async override Task ExecuteCoreAsync()
        {
            var projectService = GetProjectService();
            var projectCollection = await projectService.ListProjectsAsync();

            if (projectCollection != null && projectCollection.Value != null)
            {
                foreach (var project in projectCollection.Value)
                {
                    if(opts.Resources.Contains(ManifestKind.Permission))
                    {
                        await ExportProjectPermissionsAsync(project);
                    }
                }
            }
        }

        protected override Task ExecuteCoreAsync(BaseSchema baseSchema, string manifest, string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
