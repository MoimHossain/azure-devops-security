

using Kdoctl.Schema;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kdoctl.CliServices
{
    public abstract class MigrationTaskBase<MigrationItem> : TaskBase 
        where MigrationItem: class
    {
        public MigrationTaskBase(IServiceProvider services) : base(services)
        {
                        
        }
        protected async override Task ExecuteCoreAsync()
        {            
            await PrepareMigrationAsync();

            foreach(var migrationItem in await ListMigrationItemsAsync())
            {   
                await MapMigrationFieldsAsync(migrationItem);
                await EnrichMigrationFieldsAsync(migrationItem);
                await MigrateItemAsync(migrationItem);
            }
        }



        protected abstract Task MigrateItemAsync(MigrationItem migrationItem);
        protected abstract Task<IEnumerable<MigrationItem>> ListMigrationItemsAsync();
        protected async virtual Task EnrichMigrationFieldsAsync(MigrationItem migrationItem)
        {
            await Task.CompletedTask;
        }
        protected async virtual Task MapMigrationFieldsAsync(MigrationItem migrationItem)
        {
            await Task.CompletedTask;
        }
        protected override Task ExecuteCoreAsync(BaseSchema baseSchema, string manifest, string filePath)
        {
            throw new NotSupportedException("This version is not used in migration tasks.");
        }
        protected async virtual Task PrepareMigrationAsync()
        {
            await Task.CompletedTask;
        }
    }
}
