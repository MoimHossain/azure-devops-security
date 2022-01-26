

using Kdoctl.CliOptions;
using Kdoctl.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kdoctl.CliServices.Tasks
{
    public class WorkItemMigrationTask : MigrationTaskBase
    {
        private readonly WorkItemMigrateOptions options;

        public WorkItemMigrationTask(IServiceProvider services, WorkItemMigrateOptions options) : base(services)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        protected async override Task ExecuteCoreAsync()
        {
            await Task.CompletedTask;
            Logger.Message("GAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
        }

        protected override Task ExecuteCoreAsync(BaseSchema baseSchema, string manifest, string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
